namespace JobSearch.Features.Recruiters
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Authorize]
    [Route("api/recruiters")]
    public class RecruitersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecruitersController(IMediator mediator)
        {
            _mediator = mediator;
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
        public async Task<ActionResult<List<RecruiterResponse>>> GetRecruiterById(int id)
        {
            var request = new GetRecruiterById.Request { Id = id };

            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Saves a recruiter for the logged in user.
        /// </summary>
        /// <returns>The saved recruiter.</returns>
        [HttpPost]
        public async Task<ActionResult<List<RecruiterResponse>>> CreateRecruiter([FromBody]CreateRecruiter.Request request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}
