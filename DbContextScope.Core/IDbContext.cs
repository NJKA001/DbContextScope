
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace DbContextScope.Core
{
    public interface IDbContext : IDisposable
    {
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancelToken);
    }

}
