﻿namespace JobSearch.Identity.Queries.User
{
    using System.Collections.Generic;
    using System.Linq;
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class InsertUserQuery : IInsertQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public InsertUserQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var columns = entity.GetColumns(_sqlConfiguration, ignoreProperties: new[] { "ConcurrencyStamp" });

            var valuesArray = new List<string>(columns.Count());
            valuesArray = valuesArray.InsertQueryValuesFragment(_sqlConfiguration.ParameterNotation, columns);

            var query = _sqlConfiguration.InsertUserQuery.ReplaceInsertQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.UserTable,
                columns.GetCommaSeparatedColumns(),
                string.Join(", ", valuesArray));

            return query;
        }
    }
}
