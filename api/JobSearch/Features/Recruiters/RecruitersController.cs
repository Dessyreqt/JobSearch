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
        public async Task<ActionResult<List<GetRecruiters.Response>>> GetRecruiters([FromBody]GetRecruiters.Request request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}
