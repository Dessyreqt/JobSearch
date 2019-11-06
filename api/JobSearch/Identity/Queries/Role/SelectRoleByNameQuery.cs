namespace JobSearch.Identity.Queries.Role
{
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class SelectRoleByNameQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public SelectRoleByNameQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.SelectRoleByNameQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.RoleTable,
                _sqlConfiguration.ParameterNotation,
                new[] { "%NAME%" },
                new[] { "Name" });

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
