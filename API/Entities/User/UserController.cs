using System;
using AlpimiAPI.User.Commands;
using AlpimiAPI.User.DTO;
using AlpimiAPI.User.Queries;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateUserDTO request)
        {
            var command = new CreateUserCommand(Guid.NewGuid(), request.Login, request.CustomURL);
            var res = await _mediator.Send(command);

            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetOne([FromRoute] Guid id)
        {
            var query = new GetUserQuery(id);
            User res = await _mediator.Send(query);

            if (res is null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var command = new DeleteUserCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<User>> Patch(
            [FromBody] UpdateUserDTO request,
            [FromRoute] Guid id
        )
        {
            var command = new UpdateUserCommand(id, request.Login, request.CustomURL);
            User res = await _mediator.Send(command);
            if (res is null)
            {
                return NotFound();
            }
            return Ok(res);
        }
    }
}
