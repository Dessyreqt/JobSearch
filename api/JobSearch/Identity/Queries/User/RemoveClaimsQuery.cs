namespace JobSearch.Identity.Queries.User
{
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class RemoveClaimsQuery : IDeleteQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public RemoveClaimsQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.RemoveClaimsQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.UserClaimTable,
                _sqlConfiguration.ParameterNotation,
                new[] { "%ID%", "%CLAIMVALUE%", "%CLAIMTYPE%" },
                new[] { "UserId", "ClaimValue", "ClaimType" });

            return query;
        }
    }
}
