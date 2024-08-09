using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AlpimiAPI.Dog.Commands;
using AlpimiAPI.Dog.Queries;
using AlpimiAPI.Dog.Requests;

namespace AlpimiAPI.Dog
{
    [Route("api/[controller]")]
    [ApiController]
    public class DogController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DogController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateDogRequest request)
        {
            var command = new CreateDogCommand(request.Name, request.BirthDate, request.BreedId);
            var res = await _mediator.Send(command);

            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Dog>> GetOne([FromRoute] int id)
        {
            var query = new GetDogQuery(id);
            var res = await _mediator.Send(query);

            if(res is null) 
            {
                return NotFound();
            }

            return Ok(res);
        }
    }
}
