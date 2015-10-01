//-------------------------------------------------------------------------------------------------
// Code from Nikos Baxevanis http://nikobaxevanis.com/
//-------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;
using System.SoftBytes.Exceptions;

namespace System.SoftBytes.Validation
{
    [Serializable]
    public sealed class InvalidValueExceptionArgs : ExceptionArgs
    {
        private readonly IList<IInvalidValueInfo> m_errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueExceptionArgs"/> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public InvalidValueExceptionArgs(IList<IInvalidValueInfo> errors)
        {
            m_errors = errors;
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error.</value>
        public IList<IInvalidValueInfo> Errors
        {
            get
            {
                return m_errors;
            }
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public override String Message
        {
            get
            {
                return (m_errors == null) ? base.Message : FormatErrors(m_errors);
            }
        }

        /// <summary>
        /// Formats the errors.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <returns></returns>
        private static String FormatErrors(IList<IInvalidValueInfo> errors)
        {
            var sb = new StringBuilder();
            foreach (IInvalidValueInfo error in errors)
            {
                // Format each error and append it as a new line on the StringBuilder instance.
                FormatError(error, ref sb);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formats the error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="sb">The sb.</param>
        private static void FormatError(IInvalidValueInfo error, ref StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendFormat("{0}.{1} {2}", 
                error.EntityType.Name, error.PropertyName, error.Message);
        }
    }
}
