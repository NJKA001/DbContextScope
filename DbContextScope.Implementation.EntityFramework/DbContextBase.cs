
using System.Data;
using System.Data.Entity;
using DbContextScope.Core;

namespace DbContextScope.Implementation.EntityFramework
{
    public abstract class DbContextBase : DbContext, IDbContext
    {
        protected DbContextBase(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }


        public System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return new EntityFrameworkTrancactionContextAdapter(Database.BeginTransaction(isolationLevel));
        }

        private class EntityFrameworkTrancactionContextAdapter : IDbTransaction
        {
            private readonly DbContextTransaction _transaction;

            public EntityFrameworkTrancactionContextAdapter(DbContextTransaction transaction)
            {
                _transaction = transaction;
            }

            public void Dispose()
            {
                _transaction.Dispose();
            }

            public void Commit()
            {
                _transaction.Commit();
            }

            public void Rollback()
            {
                _transaction.Rollback();
            }

            public IDbConnection Connection
            {
                get { return _transaction.UnderlyingTransaction.Connection; }
            }

            public IsolationLevel IsolationLevel
            {
                get { return _transaction.UnderlyingTransaction.IsolationLevel; }
            }
        }

    }
}
