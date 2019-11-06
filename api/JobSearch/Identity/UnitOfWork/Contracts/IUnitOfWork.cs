namespace JobSearch.Identity.UnitOfWork.Contracts
{
    using System;
    using System.Data.Common;

    public interface IUnitOfWork : IDisposable
    {
        DbTransaction Transaction { get; }
        DbConnection Connection { get; }

        DbConnection CreateOrGetConnection();
        void DiscardChanges();
        void CommitChanges();
    }
}