using AlpimiAPI.Entities.EUser.Commands;
using AlpimiAPI.Entities.EUser.DTO;
using AlpimiAPI.Entities.EUser.Queries;
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

        [AllowAnonymous]
        [HttpPost]
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
        public async Task<ActionResult<User>> GetOne([FromRoute] Guid id)
        {
            var query = new GetUserQuery(id);
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
        public async Task<ActionResult<User>> GetOneByLogin([FromRoute] string login)
        {
            var query = new GetUserByLoginQuery(login);
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

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var command = new DeleteUserCommand(id);
            try
            {
                await _mediator.Send(command);

                return NoContent();
            }
            catch (HttpRequestException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<User>> Patch(
            [FromBody] UpdateUserDTO request,
            [FromRoute] Guid id
        )
        {
            var command = new UpdateUserCommand(id, request.Login, request.CustomURL);
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
            catch (Exception)
            {
                return BadRequest("TODO make a message");
            }
        }
    }
}
