namespace JobSearch.Features.Users.Actions.ChangePassword
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using JobSearch.Identity.Entities;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    public class Request : AuthRequest, IRequest<Response>
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class Response
    {
    }

    public class Validation : AbstractValidator<Request>
    {
        public Validation()
        {
            RuleFor(x => x.NewPassword).Length(6, 100);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword).WithMessage("'Confirm Password' should match 'Password'.");
        }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly UserManager<DapperIdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(UserManager<DapperIdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (result.Succeeded)
            {
                return new Response();
            }

            throw new AggregateException(result.Errors.Select(x => new ValidationException(x.Description)));
        }
    }
}
