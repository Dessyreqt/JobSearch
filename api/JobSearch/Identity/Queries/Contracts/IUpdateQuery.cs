namespace JobSearch.Identity.Queries.Contracts
{
    public interface IUpdateQuery : IQuery
    {
        string GetQuery<TEntity>(TEntity entity);
    }
}
