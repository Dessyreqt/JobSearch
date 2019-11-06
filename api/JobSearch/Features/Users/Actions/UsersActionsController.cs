namespace JobSearch.Features.Users.Actions
{
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/users/actions")]
    public class UsersActionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersActionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Allows the user to change their password.
        /// </summary>
        [Authorize]
        [HttpPost("changePassword")]
        public async Task<ActionResult<ChangePassword.Response>> ActionChangePassword([FromBody]ChangePassword.Request request)
        {
            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// If the code provided matches the userId, then marked the user as having confirmed their email. This code is sent via email to the user after invoking the "register" method.
        /// </summary>
        /// <returns>A response containing a JWT which can be used to access APIs that require authorization for up to one day. After that, the login request must be sent again.</returns>
        [HttpPost("confirmEmail")]
        public async Task<ActionResult<ConfirmEmail.Response>> ActionConfirmEmail([FromBody]ConfirmEmail.Request request)
        {
            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Sends an email to the user containing a code which should be passed to the "resetPassword" method.
        /// </summary>
        [HttpPost("forgotPassword")]
        public async Task<ActionResult<ForgotPassword.Response>> ActionForgotPassword([FromBody]ForgotPassword.Request request)
        {
            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Logs a user in.
        /// </summary>
        /// <returns>A response containing a JWT which can be used to access APIs that require authorization for up to one day. After that, the login request must be sent again.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<Login.Response>> ActionLogin([FromBody]Login.Request request)
        {
            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// Allows a user to register. The user then receives an email asking them to confirm their email, which contains a token that is sent back to the "confirmEmail" endpoint.
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<Register.Response>> ActionRegister([FromBody]Register.Request request)
        {
            return Ok(await _mediator.Send(request));
        }

        /// <summary>
        /// If the code matches what was sent to the user's email from the "forgotPassword" method, resets the password to the specified password.
        /// </summary>
        [HttpPost("resetPassword")]
        public async Task<ActionResult<ResetPassword.Response>> ActionResetPassword([FromBody]ResetPassword.Request request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}