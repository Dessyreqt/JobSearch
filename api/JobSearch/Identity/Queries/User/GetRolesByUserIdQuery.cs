namespace JobSearch.Identity.Queries.User
{
    using System;
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class GetRolesByUserIdQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public GetRolesByUserIdQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.GetRolesByUserIdQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                string.Empty,
                _sqlConfiguration.ParameterNotation,
                new[] { "%ID%" },
                new[] { "UserId" },
                new[] { "%ROLETABLE%", "%USERROLETABLE%" },
                new[] { _sqlConfiguration.RoleTable, _sqlConfiguration.UserRoleTable });

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
