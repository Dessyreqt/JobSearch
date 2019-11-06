namespace JobSearch.Tests.Features.Users.Actions.Register
{
    using FluentValidation.TestHelper;
    using JobSearch.Features.Users.Actions.Register;

    public class WhenValidatingARegisterUserRequest
    {
        private readonly Validation _validation;

        public WhenValidatingARegisterUserRequest(Validation validation)
        {
            _validation = validation;
        }

        public void ShouldNotBeValidIfEmailMissing()
        {
            _validation.ShouldHaveValidationErrorFor(x => x.Email, new Request { Email = string.Empty });
        }

        public void ShouldNotBeValidIfEmailNotEmail()
        {
            _validation.ShouldHaveValidationErrorFor(x => x.Email, new Request { Email = "not an email" });
        }

        public void ShouldNotBeValidIfPasswordTooShort()
        {
            _validation.ShouldHaveValidationErrorFor(x => x.Password, new Request { Password = "short" });
        }

        public void ShouldNotBeValidIfPasswordTooLong()
        {
            _validation.ShouldHaveValidationErrorFor(x => x.Password, new Request { Password = "longpasswdlongpasswdlongpasswdlongpasswdlongpasswdlongpasswdlongpasswdlongpasswdlongpasswdlongpasswdlongpasswd" });
        }

        public void ShouldNotBeValidIfConfirmPasswordDoesNotMatchPassword()
        {
            _validation.ShouldHaveValidationErrorFor(x => x.ConfirmPassword, new Request { Password = "password1", ConfirmPassword = "password2" });
        }
    }
}
