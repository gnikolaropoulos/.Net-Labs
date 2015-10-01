//-------------------------------------------------------------------------------------------------
// Code from CLR via C#, 3rd Edition http://www.microsoft.com/learning/en/us/books.aspx?id=13874
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.SoftBytes.Threading
{
    public enum CoordinationStatus
    {
        AllDone,
        Timeout,
        Cancel
    };

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification="Disposed in ReportStatus")]
    public sealed class AsyncCoordinator
    {
        private Int32 m_opCount = 1;        // Decremented by AllBegun.
        private Int32 m_statusReported = 0; // 0=false, 1=true.
        private Action<CoordinationStatus> m_callback;
        private Timer m_timer;

        // This method MUST be called BEFORE calling a BeginXxx method.
        public void AboutToBegin(Int32 opsToAdd = 1)
        {
            Interlocked.Add(ref m_opCount, opsToAdd);
        }

        // This method MUST be called AFTER calling an EndXxx method.
        public void JustEnded()
        {
            if (Interlocked.Decrement(ref m_opCount) == 0)
            {
                ReportStatus(CoordinationStatus.AllDone);
            }
        }

        // This method MUST be called AFTER calling ALL BeginXxx methods.
        public void AllBegun(Action<CoordinationStatus> callback, Int32 timeout = Timeout.Infinite)
        {
            m_callback = callback;

            if (timeout != Timeout.Infinite)
            {
                m_timer = new Timer(TimeExpired, null, timeout, Timeout.Infinite);
            }
            
            JustEnded();
        }

        public void Cancel()
        {
            if (m_callback == null)
            {
                throw new InvalidOperationException("Cancel cannot be called before AllBegun");
            }

            ReportStatus(CoordinationStatus.Cancel);
        }

        private void TimeExpired(Object o) { ReportStatus(CoordinationStatus.Timeout); }

        private void ReportStatus(CoordinationStatus status)
        {
            if (m_timer != null)
            {  
                // If timer is still in play, kill it.
                Timer timer = Interlocked.Exchange(ref m_timer, null);
                if (timer != null) { timer.Dispose(); }
            }

            // If status has never been reported, report it; else ignore it.
            if (Interlocked.Exchange(ref m_statusReported, 1) == 0)
            {
                m_callback(status);
            }
        }
    }
}
