/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
 
namespace DbContextScope.Core
{
    /// <summary>
    /// Convenience methods to retrieve ambient IDbContext instances. 
    /// </summary>
    public interface IAmbientDbContextLocator
    {
        /// <summary>
        /// If called within the scope of a IDbContextScope, gets or creates 
        /// the ambient IDbContext instance for the provided IDbContext type. 
        /// 
        /// Otherwise returns null. 
        /// </summary>
        TIDbContext Get<TIDbContext>() where TIDbContext : class, IDbContext;
    }
}
