using System.Net;
using AlpimiAPI.Entities.EUser.Commands;
using AlpimiAPI.Entities.EUser.DTO;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiAPI.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EUser
{
    /// <summary>
    ///
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        ///
        public UserController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Creates a User
        /// </summary>
        /// <remarks>
        /// Admin role is required
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Guid>> Post([FromBody] CreateUserDTO request)
        {
            var command = new CreateUserCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                request.Login,
                request.CustomURL,
                request.Password
            );
            try
            {
                var res = await _mediator.Send(command);
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
        /// Gets a user by Id
        /// </summary>
        /// <remarks>
        /// JWT token is required
        /// </remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<User>> GetOne(
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredID = Privileges.GetUserIDFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetUserQuery(id, filteredID, privileges);
            try
            {
                User? res = await _mediator.Send(query);

                if (res is null)
                {
                    return NotFound();
                }

                return Ok(res);
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return BadRequest("TODO make a message");
            }
        }

        /// <summary>
        /// Gets a user by Login
        /// </summary>
        /// <remarks>
        /// JWT token is required
        /// </remarks>
        [HttpGet("byLogin/{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<User>> GetOneByLogin(
            [FromRoute] string login,
            [FromHeader] string Authorization
        )
        {
            Guid filteredID = Privileges.GetUserIDFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var query = new GetUserByLoginQuery(login, filteredID, privileges);
            try
            {
                User? res = await _mediator.Send(query);

                return Ok(res);
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return BadRequest("TODO make a message");
            }
        }

        /// <summary>
        /// Deletes a User
        /// </summary>
        /// <remarks>
        /// Admin role is required
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Delete(
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            var command = new DeleteUserCommand(id);
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
        /// Updates a user
        /// </summary>
        /// <remarks>
        /// JWT token is required
        /// </remarks>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<User>> Patch(
            [FromBody] UpdateUserDTO request,
            [FromRoute] Guid id,
            [FromHeader] string Authorization
        )
        {
            Guid filteredID = Privileges.GetUserIDFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new UpdateUserCommand(
                id,
                request.Login,
                request.CustomURL,
                filteredID,
                privileges
            );
            try
            {
                User? res = await _mediator.Send(command);
                if (res is null)
                {
                    return NotFound();
                }
                return Ok(res);
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
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
    }
}
