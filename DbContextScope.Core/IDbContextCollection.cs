/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */

using System;

namespace DbContextScope.Core
{
    /// <summary>
    /// Maintains a list of lazily-created IDbContext instances.
    /// </summary>
    public interface IDbContextCollection : IDisposable
    {
        /// <summary>
        /// Get or create a IDbContext instance of the specified type. 
        /// </summary>
		TIDbContext Get<TIDbContext>() where TIDbContext : class, IDbContext;
    }
}