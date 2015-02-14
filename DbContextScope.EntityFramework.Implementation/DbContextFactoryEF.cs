using System;
using DbContextScope.Core;

namespace DbContextScope.EntityFramework
{
    public class DbContextFactoryEF : IDbContextFactory
    {
        public TIDbContext CreateDbContext<TIDbContext>(bool readOnly = false) where TIDbContext : class, IDbContext
        {
            var dbContext = Activator.CreateInstance<TIDbContext>() ;
 
            var dbContextInterface = dbContext as System.Data.Entity.DbContext;
            if (dbContextInterface != null)
            {
                dbContextInterface.Configuration.AutoDetectChangesEnabled = !readOnly;
            }

            return dbContext;
        }
    }
}
