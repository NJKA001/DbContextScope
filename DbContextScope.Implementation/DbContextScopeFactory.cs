/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using System;
using System.Data;
using DbContextScope.Core;

namespace DbContextScope.Implementation
{
    public abstract class DbContextScopeFactory<TDbContextScope> : IDbContextScopeFactory where TDbContextScope : DbContextScope, new()
    {
        private readonly IDbContextFactory _factory;

        protected DbContextScopeFactory(IDbContextFactory factory = null)
        {
            _factory = factory;
        }
 
        public IDbContextScope Create(DbContextScopeOption joiningOption = DbContextScopeOption.JoinExisting)
        {
            return DbContextScope.Create<TDbContextScope>(factory: _factory,
                    joiningOption: joiningOption, 
                    readOnly: false, 
                    isolationLevel: null);
        }

        public IDataContextReadOnlyScope CreateReadOnly(DbContextScopeOption joiningOption = DbContextScopeOption.JoinExisting)
        {
            return DbContextReadOnlyScope.Create<TDbContextScope>(factory: _factory,
                joiningOption: joiningOption, 
                isolationLevel: null);
        }

        public IDbContextScope CreateWithTransaction(IsolationLevel isolationLevel)
        {
            return DbContextScope.Create<TDbContextScope>(factory: _factory,
                joiningOption: DbContextScopeOption.ForceCreateNew, 
                readOnly: false, 
                isolationLevel: isolationLevel);
        }

        public IDataContextReadOnlyScope CreateReadOnlyWithTransaction(IsolationLevel isolationLevel)
        {
            return DbContextReadOnlyScope.Create<TDbContextScope>(factory: _factory,
                joiningOption: DbContextScopeOption.ForceCreateNew, 
                isolationLevel: isolationLevel);
        }

        public IDisposable SuppressAmbientContext()
        {
            return new AmbientContextSuppressor();
        }
    }
}