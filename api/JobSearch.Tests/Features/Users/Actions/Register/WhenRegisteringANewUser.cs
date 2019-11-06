namespace JobSearch.Tests.Features.Users.Actions.Register
{
    using JobSearch.Features.Users.Actions.Register;
    using JobSearch.Identity.Entities;
    using Shouldly;

    public class WhenRegisteringANewUser
    {
        private readonly DapperIdentityUser _user;

        public WhenRegisteringANewUser()
        {
            var email = "user@dscarroll.com";
            var request = new Request { Email = email, Password = "testpassword", ConfirmPassword = "testpassword" };

            Testing.Send(request);

            _user = Testing.QueryFirst<DapperIdentityUser>("SELECT TOP 1 * FROM [dbo].[IdentityUser] WHERE [Email] = @Email", new { Email = email });
        }

        public void ShouldRegisterUser()
        {
            _user.ShouldNotBeNull();
        }

        public void ShouldMarkEmailUnconfirmed()
        {
            _user.EmailConfirmed.ShouldBe(false);
        }
    }
}
