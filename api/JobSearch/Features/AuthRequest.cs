namespace JobSearch.Features
{
    using FluentValidation;
    using JobSearch.Identity.Entities;

    public class AuthRequest
    {
        private DapperIdentityUser _user;

        public DapperIdentityUser GetUser()
        {
            return _user;
        }

        public void SetUser(DapperIdentityUser user)
        {
            _user = user;
        }
    }

    public class AuthValidator<T> : AbstractValidator<T> where T : AuthRequest
    {
        public const string CouldNotFindUserError = "Could not find user.";

        public AuthValidator()
        {
            RuleFor(x => x.GetUser()).NotNull().WithMessage(CouldNotFindUserError);
        }
    }
}
