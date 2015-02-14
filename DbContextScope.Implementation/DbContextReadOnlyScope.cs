/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using System.Data;
using DbContextScope.Core;

namespace DbContextScope.Implementation
{
    internal class DbContextReadOnlyScope : IDataContextReadOnlyScope
    {
        private IDbContextScope _internalScope;

        public IDbContextCollection DataContexts { get { return _internalScope.DbContexts; } }


        public static DbContextReadOnlyScope Create<TDbContextScope>(IsolationLevel? isolationLevel, IDbContextFactory factory = null)
             where TDbContextScope : DbContextScope, new()
        {
            return Create<TDbContextScope>(joiningOption: DbContextScopeOption.JoinExisting, isolationLevel: null,
                factory: factory);
        }

        public static DbContextReadOnlyScope Create<TDbContextScope>(IsolationLevel isolationLevel, IDbContextFactory factory = null)
             where TDbContextScope : DbContextScope, new()
        {
            return Create<TDbContextScope>(joiningOption: DbContextScopeOption.ForceCreateNew,
                isolationLevel: isolationLevel, factory: factory);
        }

        public static DbContextReadOnlyScope Create<TDbContextScope>(DbContextScopeOption joiningOption, IsolationLevel? isolationLevel, IDbContextFactory factory = null)
             where TDbContextScope : DbContextScope, new()
        {
            DbContextReadOnlyScope scope = new DbContextReadOnlyScope
            {
                _internalScope = DbContextScope.Create<TDbContextScope>(joiningOption: joiningOption, readOnly: true, isolationLevel: isolationLevel, factory: factory)
            };
            return scope;
        }


        public void Dispose()
        {
            _internalScope.Dispose();
        }
    }
}