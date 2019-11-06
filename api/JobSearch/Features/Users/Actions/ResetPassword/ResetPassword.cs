namespace JobSearch.Features.Users.Actions.ResetPassword
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using JobSearch.Identity.Entities;
    using MediatR;
    using Microsoft.AspNetCore.Identity;

    public class Request : IRequest<Response>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Code { get; set; }
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

        public Handler(UserManager<DapperIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return new Response();
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);
            if (result.Succeeded)
            {
                return new Response();
            }

            throw new AggregateException(result.Errors.Select(x => new ValidationException(x.Description)));
        }
    }
}
