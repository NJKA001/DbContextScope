using System;
using System.Threading.Tasks;
using Numero3.EntityFramework.Demo.DatabaseContext;
using Numero3.EntityFramework.Demo.DomainModel;
using DbContextScope.Core;

namespace Numero3.EntityFramework.Demo.Repositories
{
	/*
	 * An example "repository" relying on an ambient IDbContext instance.
	 * 
	 * Since we use EF to persist our data, the actual repository is of course the EF IDbContext. This
	 * class is called a "repository" for old time's sake but is merely just a collection 
	 * of pre-built Linq-to-Entities queries. This avoids having these queries copied and 
	 * pasted in every service method that need them and facilitates unit testing. 
	 * 
	 * Whether your application would benefit from using this additional layer or would
	 * be better off if its service methods queried the IDbContext directly or used some sort of query 
	 * object pattern is a design decision for you to make.
	 * 
	 * IDbContextScope is agnostic to this and will happily let you use any approach you
	 * deem most suitable for your application.
	 * 
	 */
	public class UserRepository : IUserRepository
	{
		private readonly IAmbientDbContextLocator _ambientDbContextLocator;

		private UserManagementDbContext Context
		{
			get
			{
				var context = _ambientDbContextLocator.Get<UserManagementDbContext>();

				if (context == null)
					throw new InvalidOperationException("No ambient IDbContext of type UserManagementIDbContext found. This means that this repository method has been called outside of the scope of a IDbContextScope. A repository must only be accessed within the scope of a IDbContextScope, which takes care of creating the IDbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given IDbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a IDbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");
				
				return context;
			}
		}

		public UserRepository(IAmbientDbContextLocator ambientIDbContextLocator)
		{
			if (ambientIDbContextLocator == null) throw new ArgumentNullException("ambientIDbContextLocator");
			_ambientDbContextLocator = ambientIDbContextLocator;
		}

		public User Get(Guid userId)
		{
			return Context.Users.Find(userId);
		}

		public Task<User> GetAsync(Guid userId)
		{
			return Context.Users.FindAsync(userId);
		}

		public void Add(User user)
		{
			Context.Users.Add(user);
		}
	}
}
