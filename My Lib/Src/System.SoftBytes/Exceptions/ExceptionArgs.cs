//-------------------------------------------------------------------------------------------------
// Code from CLR via C#, 3rd Edition http://www.microsoft.com/learning/en/us/books.aspx?id=13874
//-------------------------------------------------------------------------------------------------

using System;

namespace System.SoftBytes.Exceptions
{
    [Serializable]
    public abstract class ExceptionArgs
    {
        public virtual String Message { get { return String.Empty; } }
    }
}
