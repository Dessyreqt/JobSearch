namespace JobSearch.Identity.Queries.User
{
    using System;
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class GetClaimsByUserIdQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public GetClaimsByUserIdQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.GetClaimsByUserIdQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.UserClaimTable,
                _sqlConfiguration.ParameterNotation,
                new[] { "%ID%" },
                new[] { "UserId" });

            return query;
        }

        public string GetQuery<TEntity>(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
