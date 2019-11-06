namespace JobSearch.Identity.Queries.User
{
    using System;
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class GetUserLoginByLoginProviderAndProviderKeyQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public GetUserLoginByLoginProviderAndProviderKeyQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            throw new NotImplementedException();
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var userProperties = entity.GetColumns(_sqlConfiguration, ignoreIdProperty: false, ignoreProperties: new[] { "ConcurrencyStamp" }, forInsert: false);

            var query = _sqlConfiguration.GetUserLoginByLoginProviderAndProviderKeyQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                string.Empty,
                _sqlConfiguration.ParameterNotation,
                new[] { "%LOGINPROVIDER%", "%PROVIDERKEY%" },
                new[] { "LoginProvider", "ProviderKey" },
                new[] { "%USERFILTER%", "%USERTABLE%", "%USERLOGINTABLE%", "%USERROLETABLE%" },
                new[]
                {
                    userProperties.SelectFilterWithTableName(_sqlConfiguration.UserTable),
                    _sqlConfiguration.UserTable,
                    _sqlConfiguration.UserLoginTable,
                    _sqlConfiguration.UserRoleTable
                });

            return query;
        }
    }
}
