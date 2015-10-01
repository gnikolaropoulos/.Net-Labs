//-------------------------------------------------------------------------------------------------
// Code from CLR via C#, 3rd Edition http://www.microsoft.com/learning/en/us/books.aspx?id=13874
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security;

namespace System.SoftBytes.Exceptions
{
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = " ")]
    [Serializable]
    public sealed class Exception<TExceptionArgs> : Exception, ISerializable
        where TExceptionArgs : ExceptionArgs
    {
        private const String c_args = "Args";  // For (de)serialization.
        private readonly TExceptionArgs m_args;

        /// <summary>
        /// Gets the args.
        /// </summary>
        /// <value>The args.</value>
        public TExceptionArgs Args { get { return m_args; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exception&lt;TExceptionArgs&gt;"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public Exception(String message = null, Exception innerException = null)
            : this(null, message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exception&lt;TExceptionArgs&gt;"/> class.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public Exception(TExceptionArgs args, String message = null,
            Exception innerException = null)
            : base(message, innerException)
        {
            m_args = args;
        }

        // The constructor is for deserialization; since the class is sealed, the constructor is 
        // private. If this class were not sealed, this constructor should be protected.
        [SecurityCritical]
        private Exception(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_args = (TExceptionArgs)info.GetValue(c_args, typeof(TExceptionArgs));
        }

        // The method for serialization; it’s public because of the ISerializable interface.
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = " ")]
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(c_args, m_args);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <value>The message.</value>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        public override String Message
        {
            get
            {
                String baseMsg = base.Message;
                return (m_args == null) ? baseMsg : baseMsg + " (" + m_args.Message + ")";
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override Boolean Equals(Object obj)
        {
            Exception<TExceptionArgs> other = obj as Exception<TExceptionArgs>;
            if (obj == null) { return false; }
            return Object.Equals(m_args, other.m_args) && base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms
        /// and data structures like a hash table. 
        /// </returns>
        public override Int32 GetHashCode() { return base.GetHashCode(); }
    }
}
