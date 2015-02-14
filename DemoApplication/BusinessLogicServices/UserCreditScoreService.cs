using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Numero3.EntityFramework.Demo.DatabaseContext;
using DbContextScope.Core;

namespace Numero3.EntityFramework.Demo.BusinessLogicServices
{
	public class UserCreditScoreService
	{
		private readonly IDbContextScopeFactory _dbContextScopeFactory;

		public UserCreditScoreService(IDbContextScopeFactory IDbContextScopeFactory)
		{
			if (IDbContextScopeFactory == null) throw new ArgumentNullException("IDbContextScopeFactory");
			_dbContextScopeFactory = IDbContextScopeFactory;
		}

		public void UpdateCreditScoreForAllUsers()
		{
			/*
			 * Demo of IDbContextScope + parallel programming.
			 */

			using (var IDbContextScope = _dbContextScopeFactory.Create())
			{
				//-- Get all users
				var IDbContext = IDbContextScope.DbContexts.Get<UserManagementDbContext>();
				var userIds = IDbContext.Users.Select(u => u.Id).ToList();

				Console.WriteLine("Found {0} users in the database. Will calculate and store their credit scores in parallel.", userIds.Count);

				//-- Calculate and store the credit score of each user
				// We're going to imagine that calculating a credit score of a user takes some time. 
				// So we'll do it in parallel.

				// You MUST call SuppressAmbientContext() when kicking off a parallel execution flow 
				// within a IDbContextScope. Otherwise, this IDbContextScope will remain the ambient scope
				// in the parallel flows of execution, potentially leading to multiple threads
				// accessing the same IDbContext instance.
				using (_dbContextScopeFactory.SuppressAmbientContext())
				{
					Parallel.ForEach(userIds, UpdateCreditScore);
				}

				// Note: SaveChanges() isn't going to do anything in this instance since all the changes
				// were actually made and saved in separate IDbContextScopes created in separate threads.
				IDbContextScope.SaveChanges();
			}
		}

		public void UpdateCreditScore(Guid userId)
		{
			using (var IDbContextScope = _dbContextScopeFactory.Create())
			{
				var IDbContext = IDbContextScope.DbContexts.Get<UserManagementDbContext>();
				var user = IDbContext.Users.Find(userId);
				if (user == null)
					throw new ArgumentException(String.Format("Invalid userId provided: {0}. Couldn't find a User with this ID.", userId));

				// Simulate the calculation of a credit score taking some time
				var random = new Random(Thread.CurrentThread.ManagedThreadId);
				Thread.Sleep(random.Next(300, 1000));

				user.CreditScore = random.Next(1, 100);
				IDbContextScope.SaveChanges();
			}
		}
	}
}
