using System;
using System.Data;
using System.Data.Entity;
using System.Reflection;
using DbContextScope.Core;
using Numero3.EntityFramework.Demo.DomainModel;

namespace Numero3.EntityFramework.Demo.DatabaseContext
{
    public class UserManagementDbContext : DbContextBase
    {
        // Map our 'User' model by convention
        public DbSet<User> Users { get; set; }

        private const string SqlExpressServerConn = @"Data Source=.\SQLEXPRESS;Initial Catalog=DbContextScopeDemo;Integrated Security=True;MultipleActiveResultSets=True";

        public UserManagementDbContext()
            : base(SqlExpressServerConn)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Overrides for the convention-based mappings.
            // We're assuming that all our fluent mappings are declared in this assembly.
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetAssembly(typeof (UserManagementDbContext)));
        }
    }

    public abstract class DbContextBase : DbContext, DbContextScope.Core.IDbContext
    {
        protected DbContextBase(string nameOrConnectionString) : base(nameOrConnectionString)
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
