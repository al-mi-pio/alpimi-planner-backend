using AlpimiAPI.Entities.EAuth.Queries;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EAuth
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Returns a JWT token
        /// </summary>
        /// <remarks>
        /// To authenticate API requests provide given token inside the header of Authorization with the prefix Bearer
        /// </remarks>

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Login([FromBody] DTO.LoginDTO request)
        {
            var query = new LoginQuery(request.Login, request.Password);
            try
            {
                string result = await _mediator.Send(query);
                if (result == null)
                {
                    return NotFound();
                }
                var response = new ApiGetResponse<String>(result, DateTime.UtcNow);
                return Ok(response);
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
