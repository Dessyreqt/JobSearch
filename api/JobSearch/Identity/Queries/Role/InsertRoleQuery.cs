namespace JobSearch.Identity.Queries.Role
{
    using System.Collections.Generic;
    using System.Linq;
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class InsertRoleQuery : IInsertQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public InsertRoleQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var columns = entity.GetColumns(_sqlConfiguration);

            var valuesArray = new List<string>(columns.Count());
            valuesArray = valuesArray.InsertQueryValuesFragment(_sqlConfiguration.ParameterNotation, columns);

            var query = _sqlConfiguration.InsertRoleQuery.ReplaceInsertQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.RoleTable,
                columns.GetCommaSeparatedColumns(),
                string.Join(", ", valuesArray));

            return query;
        }
    }
}
