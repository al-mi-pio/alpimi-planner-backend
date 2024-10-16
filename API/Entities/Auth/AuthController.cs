using AlpimiAPI.Breed.Queries;
using AlpimiAPI.Dog.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Auth
{
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<String>> Get()
        {
            return Ok(await _mediator.Send(new GetBreedsQuery()));
        }
    }
}
