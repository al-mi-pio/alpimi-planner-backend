using System;
using System.Security.Cryptography;
using System.Text;
using AlpimiAPI.User.Commands;
using AlpimiAPI.User.DTO;
using AlpimiAPI.User.Queries;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace AlpimiAPI.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<Guid>> Post([FromBody] CreateUserDTO request)
        {
            var passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(request.Password));

            var command = new CreateUserCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                request.Login,
                request.CustomURL,
                Convert.ToHexString(passwordHash)
            );

            var res = await _mediator.Send(command);

            return Ok(res);
        }

        [HttpGet("{id}")]
        [Authorize]
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
            catch (Exception ex)
            {
                return BadRequest("TODO make a message");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
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
                return BadRequest("TODO make a message");
            }
        }

        [HttpPatch("{id}")]
        [Authorize]
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
            catch (Exception ex)
            {
                return BadRequest("TODO make a message");
            }
        }
    }
}
