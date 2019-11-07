namespace JobSearch.Features.Recruiters
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [ApiController]
    [Authorize]
    [Route("api/recruiters")]
    public class RecruitersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public RecruitersController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets a list of recruiters for the logged in user.
        /// </summary>
        /// <returns>A list of recruiters for the logged in user.</returns>
        [HttpGet]
        public async Task<ActionResult<List<RecruiterResponse>>> GetRecruiters()
        {
            var request = new GetRecruiters.Request();

            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Gets the recruiter with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the recruiter to return.</param>
        /// <returns>The recruiter with the specified id, if it exists.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<List<RecruiterResponse>>> GetRecruiter(int id)
        {
            var request = new GetRecruiter.Request { Id = id };

            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Saves a recruiter for the logged in user.
        /// </summary>
        /// <returns>The saved recruiter.</returns>
        [HttpPost]
        public async Task<ActionResult<List<RecruiterResponse>>> CreateRecruiter([FromBody]CreateRecruiter.Request request)
        {
            var response = await _mediator.Send(request);

            return Created($"{_configuration["Website:ApiUrl"]}/api/recruiters/{response.Id}", response);
        }

        /// <summary>
        /// Updates the recruiter with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the recruiter to update.</param>
        /// <param name="request"></param>
        /// <returns>The updated recruiter.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<List<RecruiterResponse>>> UpdateRecruiter(int id, [FromBody]UpdateRecruiter.Request request)
        {
            request.SetId(id);

            return Ok(await _mediator.Send(request));
        }
    }
}
