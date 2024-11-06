using AlpimiAPI.Entities.EAuth.Queries;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
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
        /// Provide a valid token inside the Authorization header with the 'Bearer' prefix
        /// </remarks>

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiGetResponse<string>>> Login(
            [FromBody] DTO.LoginDTO request
        )
        {
            var query = new LoginQuery(request.Login, request.Password);
            try
            {
                string result = await _mediator.Send(query);
                if (result == null)
                {
                    return NotFound();
                }
                var response = new ApiGetResponse<String>(result);
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

        /// <summary>
        /// Refreshes a JWT token
        /// </summary>
        /// <remarks>
        /// Takes a valid token from Authorization header and returns another one with a new expiration date
        ///
        /// - JWT token is required
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<ApiGetResponse<string>> Refresh([FromHeader] string Authorization)
        {
            var query = new RefreshTokenQuery(
                Privileges.GetUserLoginFromToken(Authorization),
                Privileges.GetUserIdFromToken(Authorization),
                Privileges.GetUserRoleFromToken(Authorization)
            );
            var refreshTokenHandler = new RefreshTokenHandler();
            string result = refreshTokenHandler.Handle(query, new CancellationToken());

            var response = new ApiGetResponse<String>(result);
            return Ok(response);
        }
    }
}
