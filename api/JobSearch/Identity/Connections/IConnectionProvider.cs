namespace JobSearch.Identity.Connections
{
    using System.Data.Common;

    public interface IConnectionProvider
    {
        DbConnection Create();
    }
}