using MediatR;
using Microsoft.AspNetCore.Mvc;
using AlpimiAPI.Breed.Commands;
using AlpimiAPI.Breed.Queries;
using AlpimiAPI.Breed.Requests;

namespace AlpimiAPI.Breed
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreedController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BreedController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateBreedRequest request)
        {
            var command = new CreateBreedCommand(request.Name, request.CountryOrigin);
            var res = await _mediator.Send(command);

            return Ok(res);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Breed>>> GetAll()
        {
            return Ok(await _mediator.Send(new GetBreedsQuery()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Breed>> GetOne([FromRoute]int id)
        {
            var query = new GetBreedQuery(id);
            Breed res = await _mediator.Send(query);

            if(res is null)
            {
                return NotFound();
            }

            return Ok(res);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var command = new DeleteBreedCommand(id);
            await _mediator.Send(command);
            
            return NoContent();
        }
    }
}
