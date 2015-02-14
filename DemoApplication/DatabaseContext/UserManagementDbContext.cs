using System;
using System.Data;
using System.Data.Entity;
using System.Reflection;
using DbContextScope.Implementation.EntityFramework;
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


}
