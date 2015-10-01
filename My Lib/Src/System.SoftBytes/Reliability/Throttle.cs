//-------------------------------------------------------------------------------------------------
// Code from Nikos Baxevanis http://nikobaxevanis.com/
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.SoftBytes.Reliability
{
    public class Throttle : IDisposable
    {
        private readonly Int32 m_maxCalls;
        private readonly Timer m_timer;

        private Int32 m_totalCalls;

        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = " ")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = " ")]
        protected Boolean m_discardCallsOnTimeout;

        public Throttle(Int32 maxNumberOfCalls)
            : this(maxNumberOfCalls, TimeSpan.FromSeconds(60)) { }

        public Throttle(Int32 maxNumberOfCalls, TimeSpan timeout)
        {
            m_maxCalls = maxNumberOfCalls;

            m_timer = new Timer(
                delegate
                {
                    Interlocked.Exchange(ref m_totalCalls, 0);
                },
                null,
                TimeSpan.Zero,
                timeout);
        }

        public void Call(Action a, Int32 totalCallsToAdd)
        {
            Interlocked.Add(ref m_totalCalls, totalCallsToAdd);
            Call(a);
        }

        public virtual void Call(Action a)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a", "Action can not be null.");
            }

            Interlocked.Increment(ref m_totalCalls);

            if (m_totalCalls >= m_maxCalls)
            {
                if (m_discardCallsOnTimeout)
                {
                    return;
                }

                SpinWait.SpinUntil(() => m_totalCalls == 0);
            }

            a();
        }

        public virtual void Wait()
        {
            SpinWait.SpinUntil(() => m_totalCalls == 0);
        }

        public Throttle DiscardCallsOnTimeout(Boolean discard)
        {
            m_discardCallsOnTimeout = discard;
            return this;
        }

        ~Throttle() { Dispose(false); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (m_timer != null)
                {
                    m_timer.Dispose();
                }
            }
        }
    }
}