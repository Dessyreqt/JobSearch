namespace JobSearch.Features.Users.Actions.Login
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
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class Response
    {
        public bool RequiresTwoFactor { get; set; }
        public string Token { get; set; }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly SignInManager<DapperIdentityUser> _signInManager;
        private readonly UserManager<DapperIdentityUser> _userManager;
        private readonly TokenGenerator _tokenGenerator;

        public Handler(SignInManager<DapperIdentityUser> signInManager, TokenGenerator tokenGenerator, UserManager<DapperIdentityUser> userManager)
        {
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(request.Email);
                return new Response { RequiresTwoFactor = false, Token = _tokenGenerator.GenerateJwtToken(request.Email, user) };
            }

            if (result.RequiresTwoFactor)
            {
                return new Response { RequiresTwoFactor = true };
            }

            if (result.IsLockedOut)
            {
                throw new ValidationException("User account locked out.");
            }

            throw new ValidationException("Invalid login attempt.");
        }
    }
}
