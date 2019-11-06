namespace JobSearch.Identity.Queries.Role
{
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class DeleteRoleQuery : IDeleteQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public DeleteRoleQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.DeleteRoleQuery.ReplaceDeleteQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.RoleTable,
                $"{_sqlConfiguration.ParameterNotation}Id");

            return query;
        }
    }
}
