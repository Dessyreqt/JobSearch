namespace JobSearch.Identity.Queries.User
{
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class UpdateUserQuery : IUpdateQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public UpdateUserQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            var roleProperties = entity.GetColumns(_sqlConfiguration, ignoreIdProperty: true, ignoreProperties: new[] { "ConcurrencyStamp" });

            var setFragment = roleProperties.UpdateQuerySetFragment(_sqlConfiguration.ParameterNotation);

            var query = _sqlConfiguration.UpdateUserQuery.ReplaceUpdateQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.UserTable,
                setFragment,
                $"{_sqlConfiguration.ParameterNotation}Id");

            return query;
        }
    }
}
