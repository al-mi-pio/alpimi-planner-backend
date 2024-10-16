using AlpimiAPI.Auth.DTO;
using AlpimiAPI.Auth.Queries;
using AlpimiAPI.Breed.Queries;
using AlpimiAPI.Dog.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<String>> Login([FromBody] DTO.LoginDTO request)
        {
            var query = new LoginQuery(request.Login, request.Password);
            string res = await _mediator.Send(query);
            //TODO add exceptions
            return Ok(res);
        }
    }
}
