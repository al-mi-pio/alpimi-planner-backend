using AlpimiAPI.Entities.ETeacher.Commands;
using AlpimiAPI.Entities.ETeacher.DTO;
using AlpimiAPI.Entities.ETeacher.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiAPI.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ETeacher
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiErrorResponse), 429)]
    [EnableRateLimiting("FixedWindow")]
    public class TeacherController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<Errors> _str;

        public TeacherController(IMediator mediator, IStringLocalizer<Errors> str)
        {
            _mediator = mediator;
            _str = str;
        }

        /// <summary>
        /// Creates a Teacher
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<ActionResult<ApiGetResponse<Guid>>> Post(
            [FromBody] CreateTeacherDTO request,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new CreateTeacherCommand(Guid.NewGuid(), request, filteredId, privileges);
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
        /// Deletes a Teacher
        /// </summary>
        /// <remarks>
        /// - JWT is required
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<ActionResult> Delete(
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new DeleteTeacherCommand(id, filteredId, privileges);
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
        /// Updates a Teacher
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<ActionResult<ApiGetResponse<TeacherDTO>>> Patch(
            [FromBody] UpdateTeacherDTO request,
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new UpdateTeacherCommand(id, request, filteredId, privileges);
            try
            {
                Teacher? result = await _mediator.Send(command);
                if (result == null)
                {
                    return NotFound(
                        new ApiErrorResponse(404, [new ErrorObject(_str["notFound", "Teacher"])])
                    );
                }
                var response = new ApiGetResponse<TeacherDTO>(DataTrimmer.Trim(result));
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
        /// Gets all Teachers by ScheduleId
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<ActionResult<ApiGetAllResponse<IEnumerable<TeacherDTO>>>> GetAll(
            [FromHeader] string Authorization,
            [FromQuery] Guid scheduleId,
            [FromQuery] int perPage = PaginationSettings.perPage,
            [FromQuery] int page = PaginationSettings.page,
            [FromQuery] string sortBy = PaginationSettings.sortBy,
            [FromQuery] string sortOrder = PaginationSettings.sortOrder
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetAllTeachersByScheduleQuery(
                scheduleId,
                filteredId,
                privileges,
                new PaginationParams(perPage, (page - 1) * perPage, sortBy, sortOrder)
            );
            try
            {
                (IEnumerable<Teacher>?, int) result = await _mediator.Send(query);
                var response = new ApiGetAllResponse<IEnumerable<TeacherDTO>>(
                    result.Item1!.Select(DataTrimmer.Trim),
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
        /// Gets a Teacher
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<ActionResult<ApiGetResponse<TeacherDTO>>> GetOne(
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetTeacherQuery(id, filteredId, privileges);
            try
            {
                Teacher? result = await _mediator.Send(query);
                if (result == null)
                {
                    return NotFound(
                        new ApiErrorResponse(404, [new ErrorObject(_str["notFound", "Teacher"])])
                    );
                }
                var response = new ApiGetResponse<TeacherDTO>(DataTrimmer.Trim(result));

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
