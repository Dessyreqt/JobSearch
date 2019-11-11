namespace JobSearch.Features.Followups
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [ApiController]
    [Authorize]
    [Route("api/followups")]
    public class FollowupsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public FollowupsController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets a list of followups for the logged in user.
        /// </summary>
        /// <returns>A list of followups for the logged in user.</returns>
        [HttpGet]
        public async Task<ActionResult<List<FollowupResponse>>> GetFollowups()
        {
            var request = new GetFollowups.Request();

            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Gets the followup with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the followup to return.</param>
        /// <returns>The followup with the specified id, if it exists.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<List<FollowupResponse>>> GetFollowup(int id)
        {
            var request = new GetFollowup.Request();
            request.SetId(id);

            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Saves a followup for the logged in user.
        /// </summary>
        /// <returns>The saved followup.</returns>
        [HttpPost]
        public async Task<ActionResult<List<FollowupResponse>>> CreateFollowup([FromBody]CreateFollowup.Request request)
        {
            var response = await _mediator.Send(request);

            return Created($"{_configuration["Website:ApiUrl"]}/api/followups/{response.Id}", response);
        }

        /// <summary>
        /// Updates the followup with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the followup to update.</param>
        /// <param name="request"></param>
        /// <returns>The updated followup.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<List<FollowupResponse>>> UpdateFollowup(int id, [FromBody]UpdateFollowup.Request request)
        {
            request.SetId(id);

            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Deletes the followup with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the followup to delete.</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<FollowupResponse>>> DeleteFollowup(int id)
        {
            var request = new DeleteFollowup.Request();
            request.SetId(id);

            await _mediator.Send(request);

            return NoContent();
        }
    }
}
