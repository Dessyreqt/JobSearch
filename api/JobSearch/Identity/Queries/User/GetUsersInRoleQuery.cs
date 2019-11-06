namespace JobSearch.Identity.Queries.User
{
    using System;
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class GetUsersInRoleQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public GetUsersInRoleQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            throw new NotImplementedException();
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var userProperties = entity.GetColumns(_sqlConfiguration, ignoreIdProperty: true, ignoreProperties: new[] { "ConcurrencyStamp" });

            var query = _sqlConfiguration.GetUsersInRoleQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                string.Empty,
                _sqlConfiguration.ParameterNotation,
                new[] { "%ROLENAME%" },
                new[] { "RoleName" },
                new[] { "%USERFILTER%", "%USERTABLE%", "%USERROLETABLE%", "%ROLETABLE%" },
                new[]
                {
                    userProperties.SelectFilterWithTableName(_sqlConfiguration.UserTable),
                    _sqlConfiguration.UserTable,
                    _sqlConfiguration.UserRoleTable,
                    _sqlConfiguration.RoleTable
                });

            return query;
        }
    }
}
