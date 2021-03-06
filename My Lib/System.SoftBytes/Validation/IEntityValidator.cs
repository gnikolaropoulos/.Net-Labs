//-------------------------------------------------------------------------------------------------
// Code from http://fabiomaulo.blogspot.com/2009/11/validation-abstraction-custom.html
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.SoftBytes.Validation
{
	///<summary>
	/// Contract for the common entity validator.
	///</summary>
	public interface IEntityValidator
	{
		///<summary>Returns true if the entity is valid.</summary>
		///<param name="entityInstance">The entity instance.</param>
		///<returns>
		/// true if the <paramref name="entityInstance"/> 
		/// has, at least, one invalid value.
		/// </returns>
		Boolean IsValid(Object entityInstance);

		///<summary>
		/// Validates an entity and returns the information about invalid values.
		/// </summary>
		///<param name="entityInstance">The entity instance.</param>
		///<returns>The list of invalid values for the entity.</returns>
		IList<IInvalidValueInfo> Validate(Object entityInstance);

		///<summary>
		/// Validates a property of the entity and returns the information about invalid values.
		///</summary>
		///<param name="entityInstance">The entity instance.</param>
		///<param name="targetProperty">The property.</param>
		///<typeparam name="T">Type of the entity.</typeparam>
		///<typeparam name="TP">Type of the property.</typeparam>
		///<returns>The list of invalid values for the given property.</returns>
		IList<IInvalidValueInfo> Validate<T, TP>(T entityInstance, Expression<Func<T, TP>> targetProperty) where T : class;

        /// <summary>
        /// Validates a property of the entity and returns the information about invalid values.
        /// </summary>
        /// <param name="entityInstance">The entity instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
		IList<IInvalidValueInfo> Validate(Object entityInstance, String propertyName);
	}
}