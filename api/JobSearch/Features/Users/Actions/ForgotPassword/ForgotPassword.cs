namespace JobSearch.Features.Users.Actions.ForgotPassword
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using JobSearch.Identity.Entities;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Configuration;

    public class Request : IRequest<Response>
    {
        public string Email { get; set; }
    }

    public class Response
    {
    }

    public class Validation : AbstractValidator<Request>
    {
        public Validation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly UserManager<DapperIdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public Handler(UserManager<DapperIdentityUser> userManager, IEmailSender emailSender, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public async Task<Response> Handle([NotNull] Request request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Email);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return new Response();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{_configuration["User:ResetPasswordUrl"]}?code={Uri.EscapeDataString(code)}";
            await _emailSender.SendEmailAsync(
                request.Email,
                "Reset Password",
                $@"<p>Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>.</p> <p>For testing purposes, please send this JSON to {_configuration["User:ResetPasswordUrl"]}.</p> Here is some JSON:
<pre>
{{
    ""email"": ""{user.Email}"",
    ""password"": ""newpassword"",
    ""confirmPassword"": ""newpassword"",
    ""code"": ""{code}""
}}
</pre>");

            return new Response();
        }
    }
}
