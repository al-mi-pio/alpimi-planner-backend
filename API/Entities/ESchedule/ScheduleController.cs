using System.Net;
using AlpimiAPI.Entities.ESchedule.Commands;
using AlpimiAPI.Entities.ESchedule.DTO;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Commands;
using AlpimiAPI.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            Guid UserID = Privileges.GetUserIDFromToken(Authorization);

            var command = new CreateScheduleCommand(
                Guid.NewGuid(),
                UserID,
                request.Name,
                request.SchoolHour
            );
            try
            {
                var res = await _mediator.Send(command);
                return Ok(res);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
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
            Guid filteredID = Privileges.GetUserIDFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetScheduleQuery(id, filteredID, privileges);
            try
            {
                Schedule? res = await _mediator.Send(query);

                if (res is null)
                {
                    return NotFound();
                }

                return Ok(res);
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
        public async Task<ActionResult<User>> GetOneByName(
            [FromRoute] string name,
            [FromHeader] string Authorization
        )
        {
            Guid filteredID = Privileges.GetUserIDFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetScheduleByNameQuery(name, filteredID, privileges);
            try
            {
                Schedule? res = await _mediator.Send(query);

                return Ok(res);
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
            Guid filteredID = Privileges.GetUserIDFromToken(Authorization);
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
        public async Task<ActionResult<User>> Patch(
            [FromBody] UpdateScheduleDTO request,
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredID = Privileges.GetUserIDFromToken(Authorization);
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
                Schedule? res = await _mediator.Send(command);
                if (res is null)
                {
                    return NotFound();
                }
                return Ok(res);
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
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<User>> GetAll([FromHeader] string Authorization)
        {
            Guid filteredID = Privileges.GetUserIDFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetSchedulesQuery(filteredID, privileges);
            try
            {
                IEnumerable<Schedule>? res = await _mediator.Send(query);

                return Ok(res);
            }
            catch (Exception)
            {
                return BadRequest("TODO make a message");
            }
        }
    }
}
