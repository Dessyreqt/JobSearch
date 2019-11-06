namespace JobSearch.Identity.Queries.Contracts
{
    using System;
    using System.Collections.Concurrent;

    public interface IQueryList
    {
        ConcurrentDictionary<Type, IQuery> RetrieveQueryList();
    }
}