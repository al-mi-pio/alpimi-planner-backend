using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELesson.Queries;
using AlpimiAPI.Entities.EStudent.Commands;
using AlpimiAPI.Entities.EStudent.DTO;
using AlpimiAPI.Entities.EStudent.Queries;
using AlpimiAPI.Entities.ESubgroup;
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

namespace AlpimiAPI.Entities.EStudent
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiErrorResponse), 429)]
    public class StudentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public StudentController(
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
        /// Creates a Student
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
            [FromBody] CreateStudentDTO request,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new CreateStudentCommand(Guid.NewGuid(), request, filteredId, privileges);
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
        /// Deletes a Student
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

            var command = new DeleteStudentCommand(id, filteredId, privileges);
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
        /// Updates a Student
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
        public async Task<ActionResult<ApiGetResponse<StudentDTO>>> Patch(
            [FromBody] UpdateStudentDTO request,
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new UpdateStudentCommand(id, request, filteredId, privileges);
            try
            {
                Student? result = await _mediator.Send(command);
                if (result == null)
                {
                    return NotFound(
                        new ApiErrorResponse(
                            404,
                            [new ErrorObject(_str["notFound", _strFields["Student"]])]
                        )
                    );
                }

                var allSubgroupsQuery = new GetAllSubgroupsQuery(
                    id,
                    filteredId,
                    privileges,
                    new PaginationParams(int.MaxValue, 0, "Id", "ASC")
                );
                (IEnumerable<Subgroup>?, int) subgroups = await _mediator.Send(allSubgroupsQuery);

                var response = new ApiGetResponse<StudentDTO>(
                    DataTrimmer.Trim(result, subgroups.Item1!)
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
        /// Gets all Students by GroupId, ScheduleId or SubgroupId
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpGet]
        [EnableRateLimiting("Regular")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<ActionResult<ApiGetAllResponse<IEnumerable<StudentDTO>>>> GetAll(
            [FromHeader] string Authorization,
            [FromQuery] Guid id,
            [FromQuery] int perPage = Configuration.perPage,
            [FromQuery] int page = Configuration.page,
            [FromQuery] string sortBy = Configuration.sortBy,
            [FromQuery] string sortOrder = Configuration.sortOrder
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetAllStudentsQuery(
                id,
                filteredId,
                privileges,
                new PaginationParams(perPage, (page - 1) * perPage, sortBy, sortOrder)
            );
            try
            {
                (IEnumerable<Student>?, int) result = await _mediator.Send(query);

                var studentDTOs = new List<StudentDTO>();

                foreach (var student in result.Item1!)
                {
                    var allSubgroupsQuery = new GetAllSubgroupsQuery(
                        student.Id,
                        filteredId,
                        privileges,
                        new PaginationParams(int.MaxValue, 0, "Id", "ASC")
                    );

                    var subgroups = await _mediator.Send(allSubgroupsQuery);

                    studentDTOs.Add(DataTrimmer.Trim(student, subgroups.Item1!));
                }

                var response = new ApiGetAllResponse<IEnumerable<StudentDTO>>(
                    studentDTOs,
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
        /// Gets a Student
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
        public async Task<ActionResult<ApiGetResponse<StudentDTO>>> GetOne(
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetStudentQuery(id, filteredId, privileges);
            try
            {
                Student? result = await _mediator.Send(query);
                if (result == null)
                {
                    return NotFound(
                        new ApiErrorResponse(
                            404,
                            [new ErrorObject(_str["notFound", _strFields["Student"]])]
                        )
                    );
                }

                var allSubgroupsQuery = new GetAllSubgroupsQuery(
                    id,
                    filteredId,
                    privileges,
                    new PaginationParams(int.MaxValue, 0, "Id", "ASC")
                );
                (IEnumerable<Subgroup>?, int) subgroups = await _mediator.Send(allSubgroupsQuery);

                var response = new ApiGetResponse<StudentDTO>(
                    DataTrimmer.Trim(result, subgroups.Item1!)
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
