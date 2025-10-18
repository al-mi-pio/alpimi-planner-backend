using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELesson.Queries;
using AlpimiAPI.Entities.ESubgroup.Commands;
using AlpimiAPI.Entities.ESubgroup.DTO;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ESubgroup
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiErrorResponse), 429)]
    public class SubgroupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public SubgroupController(
            IMediator mediator,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _mediator = mediator;
            _str = str;
            _strFields = strFields;
        }

        /// <summary>
        /// Creates a Subgroup
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpPost]
        [EnableRateLimiting("Moderate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<ActionResult<ApiGetResponse<Guid>>> Post(
            [FromBody] CreateSubgroupDTO request,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new CreateSubgroupCommand(
                Guid.NewGuid(),
                request,
                filteredId,
                privileges
            );
            try
            {
                var result = await _mediator.Send(command);

                var response = new ApiGetResponse<Guid>(result);
                return Ok(response);
            }
            catch (ApiErrorException ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.errors));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiErrorResponse(400, [new ErrorObject(_str["unknownError", ex])])
                );
            }
        }

        /// <summary>
        /// Deletes a Subgroup
        /// </summary>
        /// <remarks>
        /// - JWT is required
        /// </remarks>
        [HttpDelete("{id}")]
        [EnableRateLimiting("Moderate")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<ActionResult> Delete(
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new DeleteSubgroupCommand(id, filteredId, privileges);
            try
            {
                await _mediator.Send(command);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiErrorResponse(400, [new ErrorObject(_str["unknownError", ex])])
                );
            }
        }

        /// <summary>
        /// Updates a Subgroup
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpPatch("{id}")]
        [EnableRateLimiting("Moderate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<ActionResult<ApiGetResponse<SubgroupDTO>>> Patch(
            [FromBody] UpdateSubgroupDTO request,
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new UpdateSubgroupCommand(id, request, filteredId, privileges);
            try
            {
                Subgroup? result = await _mediator.Send(command);
                if (result == null)
                {
                    return NotFound(
                        new ApiErrorResponse(
                            404,
                            [new ErrorObject(_str["notFound", _strFields["Subgroup"]])]
                        )
                    );
                }

                var allLessonsQuery = new GetAllLessonsQuery(
                    id,
                    filteredId,
                    privileges,
                    new PaginationParams(int.MaxValue, 0, "Id", "ASC")
                );
                (IEnumerable<Lesson>?, int) lessons = await _mediator.Send(allLessonsQuery);

                var response = new ApiGetResponse<SubgroupDTO>(
                    DataTrimmer.Trim(result, lessons.Item1!)
                );
                return Ok(response);
            }
            catch (ApiErrorException ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.errors));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiErrorResponse(400, [new ErrorObject(_str["unknownError", ex])])
                );
            }
        }

        /// <summary>
        /// Gets all Subgroups by GroupId, StudentId or ScheduleId
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet]
        [EnableRateLimiting("Regular")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<ActionResult<ApiGetAllResponse<IEnumerable<SubgroupDTO>>>> GetAll(
            [FromHeader] string? Authorization,
            [FromQuery] Guid id,
            [FromQuery] int perPage = Configuration.perPage,
            [FromQuery] int page = Configuration.page,
            [FromQuery] string sortBy = Configuration.sortBy,
            [FromQuery] string sortOrder = Configuration.sortOrder
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetAllSubgroupsQuery(
                id,
                filteredId,
                privileges,
                new PaginationParams(perPage, (page - 1) * perPage, sortBy, sortOrder)
            );
            try
            {
                (IEnumerable<Subgroup>?, int) result = await _mediator.Send(query);

                var subgroupDTOs = new List<SubgroupDTO>();

                foreach (var subgroup in result.Item1!)
                {
                    var allLessonsQuery = new GetAllLessonsQuery(
                        subgroup.Id,
                        filteredId,
                        privileges,
                        new PaginationParams(int.MaxValue, 0, "Id", "ASC")
                    );

                    var lessons = await _mediator.Send(allLessonsQuery);

                    subgroupDTOs.Add(DataTrimmer.Trim(subgroup, lessons.Item1!));
                }

                var response = new ApiGetAllResponse<IEnumerable<SubgroupDTO>>(
                    subgroupDTOs,
                    new Pagination(result.Item2, perPage, page, sortBy, sortOrder)
                );
                return Ok(response);
            }
            catch (ApiErrorException ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.errors));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiErrorResponse(400, [new ErrorObject(_str["unknownError", ex])])
                );
            }
        }

        /// <summary>
        /// Gets a Subgroup
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpGet("{id}")]
        [EnableRateLimiting("Regular")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<ActionResult<ApiGetResponse<SubgroupDTO>>> GetOne(
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetSubgroupQuery(id, filteredId, privileges);

            try
            {
                Subgroup? result = await _mediator.Send(query);
                if (result == null)
                {
                    return NotFound(
                        new ApiErrorResponse(
                            404,
                            [new ErrorObject(_str["notFound", _strFields["Subgroup"]])]
                        )
                    );
                }

                var allLessonsQuery = new GetAllLessonsQuery(
                    id,
                    filteredId,
                    privileges,
                    new PaginationParams(int.MaxValue, 0, "Id", "ASC")
                );
                (IEnumerable<Lesson>?, int) lessons = await _mediator.Send(allLessonsQuery);

                var response = new ApiGetResponse<SubgroupDTO>(
                    DataTrimmer.Trim(result, lessons.Item1!)
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiErrorResponse(400, [new ErrorObject(_str["unknownError", ex])])
                );
            }
        }
    }
}
