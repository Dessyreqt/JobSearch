using JobSearch.Infrastructure.CommandProcessing;
using Microsoft.AspNetCore.Mvc;

[assembly: ApiConventionType(typeof(ApiResponseConventions))]
namespace JobSearch
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Reflection;
    using System.Text;
    using FluentValidation.AspNetCore;
    using JobSearch.Identity.Database.SqlServer.Connections;
    using JobSearch.Identity.Database.SqlServer.Models;
    using JobSearch.Identity.Entities;
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Infrastructure.CommandProcessing;
    using JobSearch.Infrastructure.Logging;
    using JobSearch.Infrastructure.Validations;
    using JobSearch.Services;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureDapperConnectionProvider<SqlServerConnectionProvider>(_configuration["ConnectionString"])
                .ConfigureDapperIdentityCryptography(_configuration.GetSection("DapperIdentityCryptography"))
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

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(
                cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            services.AddControllers(options => { options.Filters.Add(new ValidatorActionFilter()); })
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<Startup>(); });

            services.AddCors();

            if (bool.Parse(_configuration["Swagger:Enabled"]))
            {
                services.AddSwaggerGen(
                    c =>
                    {
                        c.SwaggerDoc(
                            "v1",
                            new OpenApiInfo
                            {
                                Version = "v1",
                                Title = "Base API",
                                Description = "This documentation provides information about the Base API.",
                                Contact = new OpenApiContact { Name = "David Carroll", Url = new Uri("https://www.dscarroll.com/") }
                            });

                        c.AddSecurityDefinition(
                            "oauth2",
                            new OpenApiSecurityScheme
                            {
                                Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                                In = ParameterLocation.Header,
                                Name = "Authorization",
                                Type = SecuritySchemeType.ApiKey
                            });

                        c.OperationFilter<SecurityRequirementsOperationFilter>();

                        // Locate the XML file being generated by ASP.NET...
                        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                        // ... and tell Swagger to use those XML comments.
                        c.IncludeXmlComments(xmlPath);

                        c.AddFluentValidationRules();
                    });

                services.ConfigureSwaggerGen(options => options.CustomSchemaIds(x => x.FullName));
            }

            services.AddHealthChecks().AddSqlServer(_configuration["ConnectionString"], name: "Sql Server").AddUrlGroup(
                new Uri("http://status.sendgrid.com/api/v2/status.json"),
                "Sendgrid API",
                HealthStatus.Degraded);

            services.AddTransient<IDbConnection>(_ => new SqlConnection(_configuration["ConnectionString"]));
            services.AddTransient<TokenGenerator>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IValidatorInterceptor, ValidatorInterceptor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceLoggingBehavior<,>));
            services.AddMediatR(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Logger.Initialize(new LoggerSettings(_configuration));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            if (bool.Parse(_configuration["Swagger:Enabled"]))
            {
                app.UseSwagger();
                app.UseSwaggerUI(
                    c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Base API V3");
                    });
            }

            app.UseRouting();

            // This allows any requests to the API. For a new project, this must be changed.
            app.UseCors(builder => builder.WithOrigins("https://*.dscarroll.com").AllowAnyHeader().AllowAnyMethod().AllowCredentials());

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks(
                        "/healthcheck",
                        new HealthCheckOptions
                        {
                            ResponseWriter = async (context, report) =>
                            {
                                var result = JsonConvert.SerializeObject(
                                    new
                                    {
                                        status = report.Status.ToString(),
                                        errors = report.Entries.Select(e => new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
                                    });
                                context.Response.ContentType = MediaTypeNames.Application.Json;
                                await context.Response.WriteAsync(result);
                            }
                        });
                });
        }
    }
}
