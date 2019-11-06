namespace JobSearch.Identity.Queries.Role
{
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class DeleteRoleClaimQuery : IDeleteQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public DeleteRoleClaimQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.DeleteRoleClaimQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.RoleClaimTable,
                _sqlConfiguration.ParameterNotation,
                new[] { "%ROLEID%", "%CLAIMVALUE%", "%CLAIMTYPE%" },
                new[] { "RoleId", "ClaimValue", "ClaimType" });

            return query;
        }
    }
}
