namespace JobSearch.Identity.Extensions
{
    using System;
    using System.Linq;
    using JobSearch.Identity.Connections;
    using JobSearch.Identity.Cryptography;
    using JobSearch.Identity.Entities;
    using JobSearch.Identity.Factories;
    using JobSearch.Identity.Factories.Contracts;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries;
    using JobSearch.Identity.Queries.Contracts;
    using JobSearch.Identity.Repositories;
    using JobSearch.Identity.Repositories.Contracts;
    using JobSearch.Identity.Stores;
    using JobSearch.Identity.UnitOfWork.Contracts;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDapperIdentityCryptography(this IServiceCollection services, IConfigurationSection configuration)
        {
            services.Configure<AESKeys>(configuration);
            services.AddSingleton<EncryptionHelper>();

            return services;
        }

        public static IServiceCollection ConfigureDapperIdentityOptions(this IServiceCollection services, DapperIdentityOptions options)
        {
            services.AddSingleton(options);

            return services;
        }

        public static IdentityBuilder AddDapperIdentityFor<T>(this IdentityBuilder builder) where T : SqlConfiguration
        {
            builder.Services.AddSingleton<SqlConfiguration, T>();
            builder.Services.AddScoped<IUnitOfWork, global::JobSearch.Identity.UnitOfWork.UnitOfWork>();
            AddQueries(builder);

            AddStores(builder.Services, builder.UserType, builder.RoleType);

            return builder;
        }

        public static IdentityBuilder AddDapperIdentityFor<T>(this IdentityBuilder builder, T configurationOverride) where T : SqlConfiguration
        {
            builder.Services.AddSingleton<SqlConfiguration>(configurationOverride);
            builder.Services.AddScoped<IUnitOfWork, global::JobSearch.Identity.UnitOfWork.UnitOfWork>();

            AddQueries(builder);
            AddStores(builder.Services, builder.UserType, builder.RoleType);

            return builder;
        }

        public static IdentityBuilder AddDapperIdentityFor<T, TKey>(this IdentityBuilder builder) where T : SqlConfiguration
        {
            builder.Services.AddSingleton<SqlConfiguration, T>();
            builder.Services.AddScoped<IUnitOfWork, global::JobSearch.Identity.UnitOfWork.UnitOfWork>();

            AddQueries(builder);
            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey));

            return builder;
        }

        public static IdentityBuilder AddDapperIdentityFor<T, TKey>(this IdentityBuilder builder, T configurationOverride) where T : SqlConfiguration
        {
            builder.Services.AddSingleton<SqlConfiguration>(configurationOverride);
            builder.Services.AddScoped<IUnitOfWork, global::JobSearch.Identity.UnitOfWork.UnitOfWork>();

            AddQueries(builder);
            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey));

            return builder;
        }

        public static IdentityBuilder AddDapperIdentityFor<T, TKey, TUserRole, TRoleClaim>(this IdentityBuilder builder) where T : SqlConfiguration
        {
            builder.Services.AddSingleton<SqlConfiguration, T>();
            builder.Services.AddScoped<IUnitOfWork, global::JobSearch.Identity.UnitOfWork.UnitOfWork>();

            AddQueries(builder);
            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey), typeof(TUserRole), typeof(TRoleClaim));

            return builder;
        }

        public static IdentityBuilder AddDapperIdentityFor<T, TKey, TUserRole, TRoleClaim>(this IdentityBuilder builder, T configurationOverride) where T : SqlConfiguration
        {
            builder.Services.AddSingleton<SqlConfiguration>(configurationOverride);
            builder.Services.AddScoped<IUnitOfWork, global::JobSearch.Identity.UnitOfWork.UnitOfWork>();

            AddQueries(builder);
            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TKey), typeof(TUserRole), typeof(TRoleClaim));

            return builder;
        }

        public static IServiceCollection ConfigureDapperConnectionProvider<T>(this IServiceCollection services, IConfigurationSection configuration)
            where T : class, IConnectionProvider
        {
            if (configuration.Key.Equals("DapperIdentity"))
            {
                services.Configure<ConnectionProviderOptions>(configuration);
            }
            else if (configuration.Key.Equals("ConnectionStrings"))
            {
                var defaultConnection = configuration.GetValue<string>("DefaultConnection");
                if (!string.IsNullOrEmpty(defaultConnection))
                {
                    services.Configure<ConnectionProviderOptions>(x => { x.ConnectionString = defaultConnection; });
                }
                else
                {
                    var children = configuration.GetChildren();
                    if (children.Any())
                    {
                        services.Configure<ConnectionProviderOptions>(x => { x.ConnectionString = configuration.GetChildren().First().Value; });
                    }
                    else
                    {
                        throw new Exception("There's no DapperIdentity nor ConnectionStrings section with a connection string configured. Please provide one of them.");
                    }
                }
            }
            else
            {
                throw new Exception("There's no DapperIdentity nor ConnectionStrings section with a connection string configured. Please provide one of them.");
            }

            services.AddScoped<IConnectionProvider, T>();

            return services;
        }

        public static IServiceCollection ConfigureDapperConnectionProvider<T>(this IServiceCollection services, string connectionString) where T : class, IConnectionProvider
        {
            services.Configure<ConnectionProviderOptions>(x => { x.ConnectionString = connectionString; });
            services.AddScoped<IConnectionProvider, T>();

            return services;
        }

        private static void AddStores(
            IServiceCollection services,
            Type userType,
            Type roleType,
            Type keyType = null,
            Type userRoleType = null,
            Type roleClaimType = null,
            Type userClaimType = null,
            Type userLoginType = null)
        {
            Type userStoreType;
            Type roleStoreType;
            keyType = keyType ?? typeof(int);
            userRoleType = userRoleType ?? typeof(DapperIdentityUserRole<>).MakeGenericType(keyType);
            roleClaimType = roleClaimType ?? typeof(DapperIdentityRoleClaim<>).MakeGenericType(keyType);
            userClaimType = userClaimType ?? typeof(DapperIdentityUserClaim<>).MakeGenericType(keyType);
            userLoginType = userLoginType ?? typeof(DapperIdentityUserLogin<>).MakeGenericType(keyType);

            userStoreType = typeof(DapperUserStore<,,,,,,>).MakeGenericType(userType, keyType, userRoleType, roleClaimType, userClaimType, userLoginType, roleType);
            roleStoreType = typeof(DapperRoleStore<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType);

            services.AddScoped(
                typeof(IRoleRepository<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType),
                typeof(RoleRepository<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType));

            services.AddScoped(
                typeof(IUserRepository<,,,,,,>).MakeGenericType(userType, keyType, userRoleType, roleClaimType, userClaimType, userLoginType, roleType),
                typeof(UserRepository<,,,,,,>).MakeGenericType(userType, keyType, userRoleType, roleClaimType, userClaimType, userLoginType, roleType));

            services.AddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
            services.AddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);
        }

        private static void AddQueries(IdentityBuilder builder)
        {
            builder.Services.AddSingleton<IQueryList, QueryList>();
            builder.Services.AddSingleton<IQueryFactory, QueryFactory>();
        }
    }
}
