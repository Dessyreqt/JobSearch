namespace JobSearch.Identity.Queries.User
{
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class UpdateClaimForUserQuery : IUpdateQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public UpdateClaimForUserQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var query = _sqlConfiguration.UpdateClaimForUserQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.UserClaimTable,
                _sqlConfiguration.ParameterNotation,
                new[] { "%NEWCLAIMTYPE%", "%NEWCLAIMVALUE%", "%USERID%", "%CLAIMTYPE%", "%CLAIMVALUE%" },
                new[] { "NewClaimType", "NewClaimValue", "UserId", "ClaimType", "ClaimValue" });

            return query;
        }
    }
}
