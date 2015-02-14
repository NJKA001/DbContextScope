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
    /// Factory for IDbContext-derived classes that don't expose 
    /// a default constructor.
    /// </summary>
    /// <remarks>
	/// If your IDbContext-derived classes have a default constructor, 
	/// you can ignore this factory. IDbContextScope will take care of
	/// instanciating your IDbContext class with Activator.CreateInstance() 
	/// when needed.
	/// 
	/// If your IDbContext-derived classes don't expose a default constructor
	/// however, you must impement this interface and provide it to IDbContextScope
	/// so that it can create instances of your IDbContexts.
	/// 
	/// A typical situation where this would be needed is in the case of your IDbContext-derived 
	/// class having a dependency on some other component in your application. For example, 
	/// some data in your database may be encrypted and you might want your IDbContext-derived
	/// class to automatically decrypt this data on entity materialization. It would therefore 
	/// have a mandatory dependency on an IDataDecryptor component that knows how to do that. 
	/// In that case, you'll want to implement this interface and pass it to the IDbContextScope
	/// you're creating so that IDbContextScope is able to create your IDbContext instances correctly. 
    /// </remarks>
    public interface IDbContextFactory
    {
		TIDbContext CreateDbContext<TIDbContext>(bool readOnly = false) where TIDbContext : class, IDbContext;
    }
}
