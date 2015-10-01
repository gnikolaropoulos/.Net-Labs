//-------------------------------------------------------------------------------------------------
// Code from http://fabiomaulo.blogspot.com/2009/11/validation-abstraction-custom.html
//-------------------------------------------------------------------------------------------------

using System;
using System.Reflection;

namespace System.SoftBytes.Validation
{
	///<summary>
	/// Contract for the invalid values resulting from a validation.
	///</summary>
	public interface IInvalidValueInfo
	{
		/// <summary>
		/// Gets the entity type. 
        /// This is the class type that the validation result is applicable to. For instance,
		/// if the validation result concerns a duplicate record found for an employee, then
		/// this property would hold the typeof(Employee). It should be expected that this
		/// property will never be null.
		/// </summary>
		Type EntityType { get; }

		/// <summary>
		/// Gets the property name.
        /// If the validation result is applicable to a specific property, then this
		/// <see cref="PropertyInfo" /> would be set to a property name.
		/// </summary>
		String PropertyName { get; }

		/// <summary>
		/// Gets the message describing the validation result 
		/// for the EntityType and/or PropertyContext.
		/// </summary>
		String Message { get; }
	}
}