namespace JobSearch.Infrastructure.CommandProcessing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using JobSearch.Features;
    using JobSearch.Identity.Entities;
    using JobSearch.Identity.Extensions;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    public class AuthRequestUserBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly UserManager<DapperIdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthRequestUserBehavior(UserManager<DapperIdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is AuthRequest authRequest)
            {
                if (authRequest.GetUser() == null)
                {
                    var user = _userManager.FindByIdAsync(_httpContextAccessor.CurrentUserId()).GetAwaiter().GetResult();

                    if (user == null)
                    {
                        throw new UnauthorizedAccessException();
                    }

                    authRequest.SetUser(user);
                }
            }

            return await next();
        }
    }
}
