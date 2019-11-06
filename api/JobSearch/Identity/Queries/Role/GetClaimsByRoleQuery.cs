namespace JobSearch.Identity.Queries.Role
{
    using System;
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class GetClaimsByRoleQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public GetClaimsByRoleQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.SelectClaimByRoleQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                string.Empty,
                _sqlConfiguration.ParameterNotation,
                new[] { "%ROLEID%" },
                new[] { "RoleId" },
                new[] { "%ROLETABLE%", "%ROLECLAIMTABLE%" },
                new[] { _sqlConfiguration.RoleTable, _sqlConfiguration.RoleClaimTable });

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity) => throw new NotImplementedException();
    }
}
