//-------------------------------------------------------------------------------------------------
// Code from CLR via C#, 3rd Edition http://www.microsoft.com/learning/en/us/books.aspx?id=13874
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace System.SoftBytes.Events
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = " ")]
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = " ")]
    public abstract class WeakDelegate<TDelegate> where TDelegate : class /* MulticastDelegate */
    {
        // This lightweight private struct puts a compile-time type-safety wrapper around the
        // non-generic WeakReference class.
        private struct WeakReference<T> : IDisposable where T : class
        {
            private WeakReference m_weakReference;

            public WeakReference(T target) { m_weakReference = new WeakReference(target); }
            public T Target { get { return (T)m_weakReference.Target; } }
            public void Dispose() { m_weakReference = null; }
        }

        private WeakReference<TDelegate> m_weakDelegate;
        private Action<TDelegate> m_removeDelegateCode;

        protected WeakDelegate(TDelegate @delegate)
        {
            var md = (MulticastDelegate)(Object)@delegate;
            if (md.Target == null)
            {
                throw new ArgumentException(@"There is no reason to make a WeakDelegate to a
                                            static method.");
            }

            // Save a WeakReference to the delegate.
            m_weakDelegate = new WeakReference<TDelegate>(@delegate);
        }

        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = " ")]
        public Action<TDelegate> RemoveDelegateCode
        {
            set
            {
                // Save the delegate that refers to code that knows how to remove the 
                // WeakDelegate object when the non-weak delegate object is GC’d.
                m_removeDelegateCode = value;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = " ")]
        protected TDelegate GetRealDelegate()
        {
            // If the real delegate hasn't been GC'd yet, just return it.
            TDelegate realDelegate = m_weakDelegate.Target;
            if (realDelegate != null) { return realDelegate; }

            // The real delegate was GC'd, we don't need our WeakReference 
            // to it anymore (it can be GC'd).
            m_weakDelegate.Dispose();

            // Remove the delegate from the chain (if the user told us how).
            if (m_removeDelegateCode != null)
            {
                m_removeDelegateCode(GetDelegate());
                m_removeDelegateCode = null;  // Let the remove handler delegate be GC'd.
            }

            return null;   // The real delegate was GC'd and can't be called.
        }

        // All derived classes must return a delegate to a private method matching 
        // the TDelegate type.
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = " ")]
        public abstract TDelegate GetDelegate();

        // Implicit conversion operator to convert a WeakDelegate object to an actual delegate.
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = " ")]
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = " ")]
        public static implicit operator TDelegate(WeakDelegate<TDelegate> @delegate)
        {
            return @delegate.GetDelegate();
        }
    }
}
