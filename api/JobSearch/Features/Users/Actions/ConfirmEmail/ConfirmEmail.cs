namespace JobSearch.Features.Users.Actions.ConfirmEmail
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using JobSearch.Identity.Entities;
    using JobSearch.Services;
    using MediatR;
    using Microsoft.AspNetCore.Identity;

    public class Request : IRequest<Response>
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }

    public class Response
    {
        public string Token { get; set; }
    }

    public class Validation : AbstractValidator<Request>
    {
        public Validation()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly UserManager<DapperIdentityUser> _userManager;
        private readonly TokenGenerator _tokenGenerator;

        public Handler(UserManager<DapperIdentityUser> userManager, TokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                throw new ValidationException("Invalid login attempt.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Code);

            return new Response { Token = _tokenGenerator.GenerateJwtToken(user.Email, user) };
        }
    }
}
