﻿namespace JobSearch.Identity.Queries.User
{
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class SelectUserByIdQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public SelectUserByIdQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.SelectUserByIdQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                string.Empty,
                _sqlConfiguration.ParameterNotation,
                new[] { "%ID%" },
                new[] { "Id" },
                new[] { "%USERTABLE%", "%ROLETABLE%", "%USERROLETABLE%" },
                new[] { _sqlConfiguration.UserTable, _sqlConfiguration.RoleTable, _sqlConfiguration.UserRoleTable });

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
