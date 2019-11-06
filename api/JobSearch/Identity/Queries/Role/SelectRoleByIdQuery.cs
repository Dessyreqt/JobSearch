namespace JobSearch.Identity.Queries.Role
{
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class SelectRoleByIdQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public SelectRoleByIdQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.SelectRoleByIdQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.RoleTable,
                _sqlConfiguration.ParameterNotation,
                new[] { "%ID%" },
                new[] { "Id" });

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
