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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        [HttpGet("{id}")]
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

        [HttpGet("byLogin/{login}")]
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
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

        [HttpPatch("{id}")]
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
