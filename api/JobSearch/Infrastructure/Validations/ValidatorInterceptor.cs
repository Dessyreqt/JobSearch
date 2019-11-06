namespace JobSearch.Infrastructure.Validations
{
    using FluentValidation;
    using FluentValidation.AspNetCore;
    using FluentValidation.Results;
    using JobSearch.Features;
    using JobSearch.Identity.Entities;
    using JobSearch.Identity.Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ValidatorInterceptor : IValidatorInterceptor
    {
        private readonly UserManager<DapperIdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValidatorInterceptor(UserManager<DapperIdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public ValidationContext BeforeMvcValidation(ControllerContext controllerContext, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                return null;
            }

            if (validationContext.InstanceToValidate is AuthRequest authValue)
            {
                var user = _userManager.FindByIdAsync(_httpContextAccessor.CurrentUserId()).GetAwaiter().GetResult();
                authValue.SetUser(user);
            }

            return validationContext;
        }

        public ValidationResult AfterMvcValidation(ControllerContext controllerContext, ValidationContext validationContext, ValidationResult result)
        {
            return result;
        }
    }
}
