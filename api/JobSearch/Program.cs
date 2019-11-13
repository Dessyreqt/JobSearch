namespace JobSearch
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(
                (webHostBuilderContext, configurationBuilder) =>
                {
                    var env = webHostBuilderContext.HostingEnvironment;
                    configurationBuilder.SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.local.json", true, true); // load local settings
                }).ConfigureWebHostDefaults(webBuilder => { webBuilder.CaptureStartupErrors(true).UseStartup<Startup>(); });
    }
}
