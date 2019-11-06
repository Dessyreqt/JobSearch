namespace JobSearch.Tests.Services
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity.UI.Services;

    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            return Task.CompletedTask;
        }
    }
}