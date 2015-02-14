using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Numero3.EntityFramework.Demo.DatabaseContext;
using Numero3.EntityFramework.Demo.DomainModel;
using Numero3.EntityFramework.Demo.Repositories;
using DbContextScope.Core;

namespace Numero3.EntityFramework.Demo.BusinessLogicServices
{
	/*
	 * Example business logic service implementing query functionalities (i.e. read actions).
	 */
	public class UserQueryService
	{
		private readonly IDbContextScopeFactory _dbContextScopeFactory;
		private readonly IUserRepository _userRepository;

        public UserQueryService(IDbContextScopeFactory factory, IUserRepository userRepository)
		{
            if (factory == null) throw new ArgumentNullException("factory");
			if (userRepository == null) throw new ArgumentNullException("userRepository");
            _dbContextScopeFactory = factory;
			_userRepository = userRepository;
		}

		public User GetUser(Guid userId)
		{
			/*
			 * An example of using IDbContextScope for read-only queries. 
			 * Here, we access the Entity Framework IDbContext directly from 
			 * the business logic service class.
			 * 
			 * Calling SaveChanges() is not necessary here (and in fact not 
			 * possible) since we created a read-only scope.
			 */
			using (var contextScope = _dbContextScopeFactory.CreateReadOnly())
			{
				var context = contextScope.DataContexts.Get<UserManagementDbContext>();
                var user = context.Users.Find(userId);

				if (user == null)
					throw new ArgumentException(String.Format("Invalid value provided for userId: [{0}]. Couldn't find a user with this ID.", userId));

				return user;
			}
		}

		public IEnumerable<User> GetUsers(params Guid[] userIds)
		{
            using (var contextScope = _dbContextScopeFactory.CreateReadOnly())
			{
                var context = contextScope.DataContexts.Get<UserManagementDbContext>();
                return context.Users.Where(u => userIds.Contains(u.Id)).ToList();
			}
		}

		public User GetUserViaRepository(Guid userId)
		{
			/*
			 * Same as GetUsers() but using a repository layer instead of accessing the 
			 * EF IDbContext directly.
			 * 
			 * Note how we don't have to worry about knowing what type of IDbContext the 
			 * repository will need, about creating the IDbContext instance or about passing
			 * IDbContext instances around. 
			 * 
			 * The IDbContextScope will take care of creating the necessary IDbContext instances
			 * and making them available as ambient contexts for our repository layer to use.
			 * It will also guarantee that only one instance of any given IDbContext type exists
			 * within its scope ensuring that all persistent entities managed within that scope
			 * are attached to the same IDbContext. 
			 */
			using (_dbContextScopeFactory.CreateReadOnly())
			{
				var user = _userRepository.Get(userId);

				if (user == null)
					throw new ArgumentException(String.Format("Invalid value provided for userId: [{0}]. Couldn't find a user with this ID.", userId));

				return user;
			}
		}

		public async Task<IList<User>> GetTwoUsersAsync(Guid userId1, Guid userId2)
		{
			/*
			 * A very contrived example of ambient IDbContextScope within an async flow.
			 * 
			 * Note that the ConfigureAwait(false) calls here aren't strictly necessary 
			 * and are unrelated to IDbContextScope. You can remove them if you want and 
			 * the code will run in the same way. It is however good practice to configure
			 * all your awaitables in library code to not continue 
			 * on the captured synchronization context. It avoids having to pay the overhead 
			 * of capturing the sync context and running the task continuation on it when 
			 * library code doesn't need that context. If also helps prevent potential deadlocks 
			 * if the upstream code has been poorly written and blocks on async tasks. 
			 * 
			 * "Library code" is any code in layers under the presentation tier. Typically any code
			 * other that code in ASP.NET MVC / WebApi controllers or Window Form / WPF forms.
			 * 
			 * See http://blogs.msdn.com/b/pfxteam/archive/2012/04/13/10293638.aspx for 
			 * more details.
			 */

			using (_dbContextScopeFactory.CreateReadOnly())
			{
				var user1 = await _userRepository.GetAsync(userId1).ConfigureAwait(false);

				// We're now in the continuation of the first async task. This is most
				// likely executing in a thread from the ThreadPool, i.e. in a different
				// thread that the one where we created our IDbContextScope. Our ambient
				// IDbContextScope is still available here however, which allows the call 
				// below to succeed.

				var user2 = await _userRepository.GetAsync(userId2).ConfigureAwait(false);

				// In other words, IDbContextScope works with async execution flow as you'd expect: 
 				// It Just Works.  

				return new List<User> {user1, user2}.Where(u => u != null).ToList();
			}
		}

		public User GetUserUncommitted(Guid userId)
		{
			/*
			 * An example of explicit database transaction. 
			 * 
			 * Read the comment for CreateReadOnlyWithTransaction() before using this overload
			 * as there are gotchas when doing this!
			 */
			using (_dbContextScopeFactory.CreateReadOnlyWithTransaction(IsolationLevel.ReadUncommitted))
			{
				return _userRepository.Get(userId);
			}
		}
	}
}
