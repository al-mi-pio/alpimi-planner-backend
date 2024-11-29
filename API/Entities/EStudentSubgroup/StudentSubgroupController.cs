using AlpimiAPI.Entities.EStudentSubgroup.Commands;
using AlpimiAPI.Entities.EStudentSubgroup.DTO;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EStudentSubgroup
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiErrorResponse), 429)]
    [EnableRateLimiting("FixedWindow")]
    public class StudentSubgroupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<Errors> _str;

        public StudentSubgroupController(IMediator mediator, IStringLocalizer<Errors> str)
        {
            _mediator = mediator;
            _str = str;
        }

        /// <summary>
        /// Creates a StudentSubgroup
        /// </summary>
        /// <remarks>
        /// - JWT token is required
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<ActionResult<ApiGetResponse<Guid>>> Post(
            [FromBody] CreateStudentSubgroupDTO request,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new CreateStudentSubgroupCommand(request, filteredId, privileges);
            try
            {
                await _mediator.Send(command);

                return NoContent();
            }
            catch (ApiErrorException ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.errors));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiErrorResponse(400, [new ErrorObject(_str["unknownError", ex])])
                );
            }
        }

        /// <summary>
        /// Deletes a StudentSubgroup
        /// </summary>
        /// <remarks>
        /// - JWT is required
        /// </remarks>
        [HttpDelete("{studentId}/{subgroupId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        public async Task<ActionResult> Delete(
            [FromRoute] Guid studentId,
            [FromRoute] Guid subgroupId,
            [FromHeader] string Authorization
        )
        {
            Guid filteredId = Privileges.GetUserIdFromToken(Authorization);
            string privileges = Privileges.GetUserRoleFromToken(Authorization);

            var command = new DeleteStudentSubgroupCommand(
                studentId,
                subgroupId,
                filteredId,
                privileges
            );
            try
            {
                await _mediator.Send(command);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiErrorResponse(400, [new ErrorObject(_str["unknownError", ex])])
                );
            }
        }
    }
}
