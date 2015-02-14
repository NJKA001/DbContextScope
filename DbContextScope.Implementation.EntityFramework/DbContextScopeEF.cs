/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using DbContextScope.Core;

namespace DbContextScope.Implementation.EntityFramework
{

    public class EFDbContextScope : DbContextScope
    {
 
        protected override void OnRefreshEntitiesInParentScope(IEnumerable entities, IEnumerable<IDbContext> parentContexts)
        {
            if (entities == null) return;
            if (parentContexts == null) return;

            // OK, so we must loop through all the IDbContext instances in the parent scope
            // and see if their first-level cache (i.e. their ObjectStateManager) contains the provided entities. 
            // If they do, we'll need to force a refresh from the database. 

            // I'm sorry for this code but it's the only way to do this with the current version of Entity Framework 
            // as far as I can see.

            // What would be much nicer would be to have a way to merge all the modified / added / deleted
            // entities from one IDbContext instance to another. NHibernate has support for this sort of stuff 
            // but EF still lags behind in this respect. But there is hope: https://entityframework.codeplex.com/workitem/864

            // NOTE: IDbContext implements the ObjectContext property of the IObjectContextAdapter interface explicitely.
            // So we must cast the IDbContext instances to IObjectContextAdapter in order to access their ObjectContext.
            // This cast is completely safe.

            foreach (var context in parentContexts)
            {
                var contextInCurrentScope = context as IObjectContextAdapter;
                if (contextInCurrentScope == null)
                    continue; //Should never happen

                var correspondingParentContext = FindCorrespondingParentContext(context) as IObjectContextAdapter;

                if (correspondingParentContext == null)
                    continue; // No IDbContext of this type has been created in the parent scope yet. So no need to refresh anything for this IDbContext type.

                // Both our scope and the parent scope have an instance of the same IDbContext type. 
                // We can now look in the parent IDbContext instance for entities that need to
                // be refreshed.
                foreach (var toRefresh in entities)
                {
                    // First, we need to find what the EntityKey for this entity is. 
                    // We need this EntityKey in order to check if this entity has
                    // already been loaded in the parent IDbContext's first-level cache (the ObjectStateManager).
                    ObjectStateEntry stateInCurrentScope;
                    if (contextInCurrentScope.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(toRefresh, out stateInCurrentScope))
                    {
                        var key = stateInCurrentScope.EntityKey;

                        // Now we can see if that entity exists in the parent IDbContext instance and refresh it.
                        ObjectStateEntry stateInParentScope;
                        if (correspondingParentContext.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(key, out stateInParentScope))
                        {
                            // Only refresh the entity in the parent IDbContext from the database if that entity hasn't already been
                            // modified in the parent. Otherwise, let the whatever concurency rules the application uses
                            // apply.
                            if (stateInParentScope.State == EntityState.Unchanged)
                            {
                                correspondingParentContext.ObjectContext.Refresh(RefreshMode.StoreWins, stateInParentScope.Entity);
                            }
                        }
                    }
                }
            }
        }

        protected override async Task OnRefreshEntitiesInParentScopeAsync(IEnumerable entities, IEnumerable<IDbContext> parentContexts)
        {
            if (entities == null) return;
            if (parentContexts == null) return;

            foreach (var context in parentContexts)
            {
                var contextInCurrentScope = context as IObjectContextAdapter;
                if (contextInCurrentScope == null)
                    continue; //Should never happen

                var correspondingParentContext = FindCorrespondingParentContext(context) as IObjectContextAdapter;

                if (correspondingParentContext == null)
                    continue;

                foreach (var toRefresh in entities)
                {
                    ObjectStateEntry stateInCurrentScope;
                    if (contextInCurrentScope.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(toRefresh, out stateInCurrentScope))
                    {
                        var key = stateInCurrentScope.EntityKey;

                        ObjectStateEntry stateInParentScope;
                        if (correspondingParentContext.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(key, out stateInParentScope))
                        {
                            if (stateInParentScope.State == EntityState.Unchanged)
                            {
                                await correspondingParentContext.ObjectContext.RefreshAsync(RefreshMode.StoreWins, stateInParentScope.Entity).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
        }
    }

}

    