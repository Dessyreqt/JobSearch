namespace JobSearch.Tests
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using JobSearch.Identity.Database.SqlServer.Connections;
    using JobSearch.Identity.Database.SqlServer.Models;
    using JobSearch.Identity.Entities;
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Infrastructure.Logging;
    using JobSearch.Tests.Services;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public static class TestDependencyScope
    {
        private static IServiceScope _currentScope;

        public static IServiceScope GetScope()
        {
            if (_currentScope == null)
            {
                throw new Exception("Test dependency scope hasn't begun. Cannot return current nested container.");
            }

            return _currentScope;
        }

        public static bool IsInitialized()
        {
            return _currentScope != null;
        }

        public static void Begin()
        {
            if (_currentScope != null)
            {
                throw new Exception("Cannot begin test dependency scope. Another dependency scope is still in effect.");
            }

            var provider = TestingIoC.Container.BuildServiceProvider();
            _currentScope = provider.CreateScope();
        }

        public static T Resolve<T>()
        {
            if (_currentScope == null)
            {
                throw new Exception($"Cannot resolve type {typeof(T)}. There is no test dependency scope in effect.");
            }

            return _currentScope.ServiceProvider.GetService<T>();
        }

        public static object Resolve(Type type)
        {
            if (_currentScope == null)
            {
                throw new Exception($"Cannot resolve type {type}. There is no test dependency scope in effect.");
            }

            return _currentScope.ServiceProvider.GetService(type);
        }

        public static void End()
        {
            if (_currentScope == null)
            {
                throw new Exception("Cannot end test dependency scope. There is no current dependency scope to end.");
            }

            _currentScope.Dispose();
            _currentScope = null;
        }
    }

    public static class TestingIoC
    {
        private static readonly Lazy<IServiceCollection> Bootstrapper = new Lazy<IServiceCollection>(InternalInitialize, true);

        public static IServiceCollection Container => Bootstrapper.Value;

        public static void Initialize()
        {
            var container = Bootstrapper.Value;
        }

        private static IServiceCollection InternalInitialize()
        {
            var services = new ServiceCollection();

            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true) // load base settings
                .AddJsonFile("appsettings.local.json", true, true) // load local settings
                .AddEnvironmentVariables();

            var configuration = configBuilder.Build();

            services.AddSingleton<IConfiguration>(configuration);

            Logger.Initialize(new LoggerSettings(configuration));

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services.AddTransient(_ => new Func<IDbConnection>(() => new SqlConnection(configuration["ConnectionString"])));

            services.ConfigureDapperConnectionProvider<SqlServerConnectionProvider>(configuration["ConnectionString"])
                .ConfigureDapperIdentityCryptography(configuration.GetSection("DapperIdentityCryptography"))
                .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false }); // Change to True to use Transactions in all operations

            // This configuration will setup the password requirements.
            services.AddIdentity<DapperIdentityUser, DapperIdentityRole>(
                x =>
                {
                    x.Password.RequireDigit = false;
                    x.Password.RequiredLength = 1;
                    x.Password.RequireLowercase = false;
                    x.Password.RequireNonAlphanumeric = false;
                    x.Password.RequireUppercase = false;
                    x.SignIn.RequireConfirmedEmail = true;
                }).AddDapperIdentityFor<SqlServerConfiguration>().AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, FakeEmailSender>();

            services.AddMediatR(typeof(Startup));

            return services;
        }
    }
}
