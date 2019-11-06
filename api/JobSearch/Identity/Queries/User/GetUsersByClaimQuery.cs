namespace JobSearch.Identity.Queries.User
{
    using System;
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class GetUsersByClaimQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public GetUsersByClaimQuery(SqlConfiguration sqlConfiguration)
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

            var query = _sqlConfiguration.GetUsersByClaimQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.UserTable,
                _sqlConfiguration.ParameterNotation,
                new[] { "%CLAIMVALUE%", "%CLAIMTYPE%" },
                new[] { "ClaimValue", "ClaimType" },
                new[] { "%USERFILTER%", "%USERTABLE%", "%USERCLAIMTABLE%" },
                new[] { userProperties.SelectFilterWithTableName(_sqlConfiguration.UserTable), _sqlConfiguration.UserTable, _sqlConfiguration.UserClaimTable });

            return query;
        }
    }
}
