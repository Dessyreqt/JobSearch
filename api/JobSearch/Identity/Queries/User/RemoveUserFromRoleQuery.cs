namespace JobSearch.Identity.Queries.User
{
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class RemoveUserFromRoleQuery : IDeleteQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public RemoveUserFromRoleQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.RemoveUserFromRoleQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.UserRoleTable,
                _sqlConfiguration.ParameterNotation,
                new[] { "%USERID%", "%ROLENAME%" },
                new[] { "UserId", "RoleName" },
                new[] { "%USERROLETABLE%", "%ROLETABLE%" },
                new[] { _sqlConfiguration.UserRoleTable, _sqlConfiguration.RoleTable });

            return query;
        }
    }
}
