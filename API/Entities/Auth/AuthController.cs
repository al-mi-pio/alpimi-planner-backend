using AlpimiAPI.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<String>> Login([FromBody] DTO.LoginDTO request)
        {
            var query = new LoginQuery(request.Login, request.Password);
            try
            {
                string res = await _mediator.Send(query);
                return Ok(res);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("TODO make a message");
            }
        }
    }
}
