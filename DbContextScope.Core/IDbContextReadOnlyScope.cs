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
    /// A read-only IDbContextScope. Refer to the comments for IDbContextScope
    /// for more details.
    /// </summary>
    public interface IDataContextReadOnlyScope : IDisposable
    {
        /// <summary>
        /// The IDbContext instances that this IDbContextScope manages.
        /// </summary>
        IDbContextCollection DataContexts { get; }
    }
}