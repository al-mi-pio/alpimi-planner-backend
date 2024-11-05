using AlpimiAPI.Entities.ESchedule.Commands;
using AlpimiAPI.Entities.ESchedule.DTO;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using alpimi_planner_backend.API.Settings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace AlpimiAPI.Entities.ESchedule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScheduleController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Creates a Schedule
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> Post(
            [FromBody] CreateScheduleDTO request,
            [FromHeader] string Authorization
        )
        {
            Guid UserId = Privileges.GetUserIdFromToken(Authorization);

            var command = new CreateScheduleCommand(
                Guid.NewGuid(),
                UserId,
                request.Name,
                request.SchoolHour
            );
            try
            {
                var result = await _mediator.Send(command);
                var response = new ApiGetResponse<Guid>(result);
                return Ok(response);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest("TODO make a message");
            }
        }

        /// <summary>
        /// Gets a Schedule
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Schedule>> GetOne(
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredID = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetScheduleQuery(id, filteredID, privileges);
            try
            {
                Schedule? result = await _mediator.Send(query);
                if (result == null)
                {
                    return NotFound();
                }
                var response = new ApiGetResponse<Schedule>(result);

                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest("TODO make a message");
            }
        }

        /// <summary>
        /// Gets a Schedule by Name
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpGet("byName/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Schedule>> GetOneByName(
            [FromRoute] string name,
            [FromHeader] string Authorization
        )
        {
            Guid filteredID = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetScheduleByNameQuery(name, filteredID, privileges);
            try
            {
                Schedule? result = await _mediator.Send(query);
                if (result == null)
                {
                    return NotFound();
                }
                var response = new ApiGetResponse<Schedule>(result);
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest("TODO make a message");
            }
        }

        /// <summary>
        /// Deletes a Schedule
        /// </summary>
        /// <remarks>
        /// - JWT is required
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Delete(
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredID = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);
            var command = new DeleteScheduleCommand(id, filteredID, privileges);
            try
            {
                await _mediator.Send(command);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Updates a Schedule
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Schedule>> Patch(
            [FromBody] UpdateScheduleDTO request,
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredID = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new UpdateScheduleCommand(
                id,
                request.Name,
                request.SchoolHour,
                filteredID,
                privileges
            );
            try
            {
                Schedule? result = await _mediator.Send(command);
                if (result == null)
                {
                    return NotFound();
                }
                var response = new ApiGetResponse<Schedule>(result);
                return Ok(response);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest("TODO make a message");
            }
        }

        /// <summary>
        /// Gets all Schedule
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetAll(
            [FromHeader] string Authorization,
            [FromQuery] int perPage = PaginationSettings.perPage,
            [FromQuery] int page = PaginationSettings.page,
            [FromQuery] string sortBy = PaginationSettings.sortBy,
            [FromQuery] string sortOrder = PaginationSettings.sortOrder
        )
        {
            Guid filteredID = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetSchedulesQuery(
                filteredID,
                privileges,
                new PaginationParams(perPage, (page - 1) * perPage, sortBy, sortOrder)
            );
            try
            {
                (IEnumerable<Schedule>?, int) result = await _mediator.Send(query);
                var response = new ApiGetAllResponse<IEnumerable<Schedule>>(
                    result.Item1!,
                    new Pagination(result.Item2, perPage, page, sortBy, sortOrder)
                );
                return Ok(response);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest("TODO make a message");
            }
        }
    }
}
