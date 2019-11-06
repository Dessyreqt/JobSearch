namespace JobSearch.Identity.Factories.Contracts
{
    using JobSearch.Identity.Queries.Contracts;

    public interface IQueryFactory
    {
        string GetQuery<TQuery>() where TQuery : ISelectQuery;
        string GetQuery<TQuery, TEntity>(TEntity entity) where TQuery : ISelectQuery;
        string GetInsertQuery<TQuery, TEntity>(TEntity entity) where TQuery : IInsertQuery;
        string GetUpdateQuery<TQuery, TEntity>(TEntity entity) where TQuery : IUpdateQuery;
        string GetDeleteQuery<TQuery>() where TQuery : IDeleteQuery;
    }
}
