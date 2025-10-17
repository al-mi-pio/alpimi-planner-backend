using AlpimiAPI.Entities.EClassroom.Commands;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.EClassroom.Queries;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EClassroom
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiErrorResponse), 429)]
    public class ClassroomController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public ClassroomController(
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
        /// Creates a Classroom
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
            [FromBody] CreateClassroomDTO request,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new CreateClassroomCommand(
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
        /// Deletes a Classroom
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

            var command = new DeleteClassroomCommand(id, filteredId, privileges);
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
        /// Updates a Classroom
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
        public async Task<ActionResult<ApiGetResponse<ClassroomDTO>>> Patch(
            [FromBody] UpdateClassroomDTO request,
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new UpdateClassroomCommand(id, request, filteredId, privileges);
            try
            {
                Classroom? result = await _mediator.Send(command);
                if (result == null)
                {
                    return NotFound(
                        new ApiErrorResponse(
                            404,
                            [new ErrorObject(_str["notFound", _strFields["Classroom"]])]
                        )
                    );
                }

                var allClassroomTypesQuery = new GetAllClassroomTypesQuery(
                    id,
                    filteredId,
                    privileges,
                    new PaginationParams(int.MaxValue, 0, "Id", "ASC")
                );
                (IEnumerable<ClassroomType>?, int) classroomTypes = await _mediator.Send(
                    allClassroomTypesQuery
                );

                var response = new ApiGetResponse<ClassroomDTO>(
                    DataTrimmer.Trim(result, classroomTypes.Item1!)
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
        /// Gets all Classrooms by ScheduleId or ClassroomTypeId
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpGet]
        [EnableRateLimiting("Regular")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<ActionResult<ApiGetAllResponse<IEnumerable<ClassroomDTO>>>> GetAll(
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

            var query = new GetAllClassroomsQuery(
                id,
                filteredId,
                privileges,
                new PaginationParams(perPage, (page - 1) * perPage, sortBy, sortOrder)
            );
            try
            {
                (IEnumerable<Classroom>?, int) result = await _mediator.Send(query);

                var classroomDTOs = new List<ClassroomDTO>();

                foreach (var classroom in result.Item1!)
                {
                    var allClassroomTypesQuery = new GetAllClassroomTypesQuery(
                        classroom.Id,
                        filteredId,
                        privileges,
                        new PaginationParams(int.MaxValue, 0, "Id", "ASC")
                    );

                    var classroomTypes = await _mediator.Send(allClassroomTypesQuery);

                    classroomDTOs.Add(DataTrimmer.Trim(classroom, classroomTypes.Item1!));
                }

                var response = new ApiGetAllResponse<IEnumerable<ClassroomDTO>>(
                    classroomDTOs,
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
        /// Gets a Classroom
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
        public async Task<ActionResult<ApiGetResponse<ClassroomDTO>>> GetOne(
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetClassroomQuery(id, filteredId, privileges);
            try
            {
                Classroom? result = await _mediator.Send(query);
                if (result == null)
                {
                    return NotFound(
                        new ApiErrorResponse(
                            404,
                            [new ErrorObject(_str["notFound", _strFields["Classroom"]])]
                        )
                    );
                }

                var allClassroomTypesQuery = new GetAllClassroomTypesQuery(
                    id,
                    filteredId,
                    privileges,
                    new PaginationParams(int.MaxValue, 0, "Id", "ASC")
                );
                (IEnumerable<ClassroomType>?, int) classroomTypes = await _mediator.Send(
                    allClassroomTypesQuery
                );

                var response = new ApiGetResponse<ClassroomDTO>(
                    DataTrimmer.Trim(result, classroomTypes.Item1!)
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
