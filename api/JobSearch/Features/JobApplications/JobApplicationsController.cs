namespace JobSearch.Features.JobApplications
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [ApiController]
    [Authorize]
    [Route("api/jobApplications")]
    public class JobApplicationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public JobApplicationsController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets a list of jobApplications for the logged in user.
        /// </summary>
        /// <returns>A list of jobApplications for the logged in user.</returns>
        [HttpGet]
        public async Task<ActionResult<List<JobApplicationResponse>>> GetJobApplications()
        {
            var request = new GetJobApplications.Request();

            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Gets the jobApplication with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the jobApplication to return.</param>
        /// <returns>The jobApplication with the specified id, if it exists.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<List<JobApplicationResponse>>> GetJobApplication(int id)
        {
            var request = new GetJobApplication.Request();
            request.SetId(id);

            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Saves a jobApplication for the logged in user.
        /// </summary>
        /// <returns>The saved jobApplication.</returns>
        [HttpPost]
        public async Task<ActionResult<List<JobApplicationResponse>>> CreateJobApplication([FromBody]CreateJobApplication.Request request)
        {
            var response = await _mediator.Send(request);

            return Created($"{_configuration["Website:ApiUrl"]}/api/jobApplications/{response.Id}", response);
        }

        /// <summary>
        /// Updates the jobApplication with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the jobApplication to update.</param>
        /// <param name="request"></param>
        /// <returns>The updated jobApplication.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<List<JobApplicationResponse>>> UpdateJobApplication(int id, [FromBody]UpdateJobApplication.Request request)
        {
            request.SetId(id);

            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Deletes the jobApplication with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the jobApplication to delete.</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<JobApplicationResponse>>> DeleteJobApplication(int id)
        {
            var request = new DeleteJobApplication.Request();
            request.SetId(id);

            await _mediator.Send(request);

            return NoContent();
        }
    }
}
