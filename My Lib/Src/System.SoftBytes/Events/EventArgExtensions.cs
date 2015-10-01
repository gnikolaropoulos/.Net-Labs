//-------------------------------------------------------------------------------------------------
// Code from CLR via C#, 3rd Edition http://www.microsoft.com/learning/en/us/books.aspx?id=13874
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.SoftBytes.Events
{
    public static class EventArgExtensions
    {
        /// <summary>
        /// Extension method that encapsulates the thread-safety logic as discussed in 
        /// Chapter 8, "Methods" of the book "CLR via C# 3rd Edition", Mictosoft Press, 2010.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="e">The <see cref="TEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="sender">The sender.</param>
        /// <param name="eventDelegate">The event delegate.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = " ")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = " ")]
        public static void Raise<TEventArgs>(
            this TEventArgs e,
            Object sender,
            ref EventHandler<TEventArgs> eventDelegate) where TEventArgs : EventArgs
        {
            // Copy a reference to the delegate field now into a temporary field for thread safety.
            EventHandler<TEventArgs> temp = 
                Interlocked.CompareExchange(ref eventDelegate, null, null);

            // If any methods registered interest with our event, notify them.
            if (temp != null) { temp(sender, e); }
        }
    }
}
