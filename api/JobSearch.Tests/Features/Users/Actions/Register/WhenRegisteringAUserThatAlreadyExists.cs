namespace JobSearch.Tests.Features.Users.Actions.Register
{
    using System;
    using System.Linq;
    using FluentValidation;
    using JobSearch.Features.Users.Actions.Register;
    using JobSearch.Identity.Entities;
    using Microsoft.AspNetCore.Identity;
    using Shouldly;

    public class WhenRegisteringAUserThatAlreadyExists
    {
        private readonly AggregateException _exception;

        public WhenRegisteringAUserThatAlreadyExists(UserManager<DapperIdentityUser> userManager)
        {
            var email = "user@dscarroll.com";
            var password = "testpassword";

            var result = userManager.CreateAsync(new DapperIdentityUser { UserName = email, Email = email }, password).Result;

            var request = new Request { Email = email, Password = password, ConfirmPassword = password };

            try
            {
                Testing.Send(request);
            }
            catch (AggregateException ex)
            {
                _exception = ex;
            }
        }

        public void ShouldThrowException()
        {
            _exception.ShouldNotBeNull();
        }

        public void ShouldHaveValidationException()
        {
            var innerAggregateException = _exception.InnerExceptions.First() as AggregateException;
            innerAggregateException.InnerExceptions.ShouldContain(ex => ex is ValidationException);
        }

        public void ShouldHaveCorrectError()
        {
            var innerAggregateException = _exception.InnerExceptions.First() as AggregateException;
            innerAggregateException.InnerExceptions.First().Message.ShouldBe("User name 'user@dscarroll.com' is already taken.");
        }
    }
}
