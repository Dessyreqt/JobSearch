namespace JobSearch.Identity.Queries.User
{
    using System;
    using JobSearch.Identity.Extensions;
    using JobSearch.Identity.Models;
    using JobSearch.Identity.Queries.Contracts;

    public class GetUserLoginInfoByIdQuery : ISelectQuery
    {
        private readonly SqlConfiguration _sqlConfiguration;

        public GetUserLoginInfoByIdQuery(SqlConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public string GetQuery()
        {
            var query = _sqlConfiguration.GetUserLoginInfoByIdQuery.ReplaceQueryParameters(
                _sqlConfiguration.SchemaName,
                _sqlConfiguration.UserLoginTable,
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
