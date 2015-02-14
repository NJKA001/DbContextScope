/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using DbContextScope.Core;

namespace DbContextScope.EntityFramework
{
    public class AmbientDbContextLocator : IAmbientDbContextLocator
    {
        public TDbContext Get<TDbContext>() where TDbContext :class, IDbContext
        {
            var ambientIDbContextScope = DbContextScope.GetAmbientScope();
            return ambientIDbContextScope == null ? null : ambientIDbContextScope.DbContexts.Get<TDbContext>();
        }
    }
}