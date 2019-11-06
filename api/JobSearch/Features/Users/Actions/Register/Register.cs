namespace JobSearch.Features.Users.Actions.Register
{
    using System;
    using System.Linq;
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
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class Response
    {
    }

    public class Validation : AbstractValidator<Request>
    {
        public Validation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).Length(6, 100);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("'Confirm Password' should match 'Password'.");
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

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = new DapperIdentityUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                // Send an email with this link
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = $"{_configuration["User:EmailConfirmationUrl"]}?userId={user.Id}&code={Uri.EscapeDataString(code)}";
                await _emailSender.SendEmailAsync(
                    request.Email,
                    "Confirm Email",
                    $@"<p>Please confirm your email by clicking here: <a href='{callbackUrl}'>link</a>.</p> <p>For testing purposes, please send this JSON to {_configuration["User:EmailConfirmationUrl"]}.</p> Here is some JSON:
<pre>
{{
    ""userId"": ""{user.Id}"",
    ""code"": ""{code}""
}}
</pre>");

                return new Response();
            }

            throw new AggregateException(result.Errors.Select(x => new ValidationException(x.Description)));
        }
    }
}
