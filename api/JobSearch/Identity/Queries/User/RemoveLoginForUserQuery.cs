namespace JobSearch.Identity.Queries.User
{
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class RemoveLoginForUserQuery : IDeleteQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public RemoveLoginForUserQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.RemoveLoginForUserQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.UserLoginTable,
                _sqlConfiguration.ParameterNotation,
                new[] { "%USERID%", "%LOGINPROVIDER%", "%PROVIDERKEY%" },
                new[] { "UserId", "LoginProvider", "ProviderKey" });

            return query;
        }
    }
}
