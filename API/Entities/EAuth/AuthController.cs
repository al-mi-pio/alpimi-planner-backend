using AlpimiAPI.Entities.EAuth.Queries;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using alpimi_planner_backend.API.Locales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EAuth
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly IStringLocalizer<Errors> _str;

        public AuthController(IMediator mediator, IStringLocalizer<Errors> str)
        {
            _mediator = mediator;
            _str = str;
        }

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
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<ActionResult<ApiGetResponse<string>>> Login(
            [FromBody] DTO.LoginDTO request
        )
        {
            var query = new LoginQuery(request.Login, request.Password);
            try
            {
                string result = await _mediator.Send(query);
                var response = new ApiGetResponse<String>(result);
                return Ok(response);
            }
            catch (ApiErrorException ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.errors));
            }
            catch (Exception)
            {
                return BadRequest(
                    new ApiErrorResponse(400, [new ErrorObject(_str["unknownError"])])
                );
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
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
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
