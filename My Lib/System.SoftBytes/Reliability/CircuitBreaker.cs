//-------------------------------------------------------------------------------------------------
// Code from Davy Brion http://davybrion.com/blog/2008/05/the-circuit-breaker/
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.SoftBytes.Threading;

namespace System.SoftBytes.Reliability
{
	public class CircuitBreaker : IDisposable
	{
		private readonly Object m_lockObj = new Object();

		private CircuitBreakerState m_state;

		public CircuitBreaker(Int32 threshold, TimeSpan timeout)
		{
			if (threshold < 1)
			{
				throw new ArgumentOutOfRangeException("threshold", "Threshold should be greater than 0.");
			}

			if (timeout.TotalMilliseconds < 1)
			{
				throw new ArgumentOutOfRangeException("timeout", "Timeout should be greater than 0.");
			}

			Threshold = threshold;
			Timeout = timeout;

			MoveToClosedState();
		}

        protected CircuitBreakerState State { get { return m_state; } }

		public Int32 Failures { get; protected set; }

        public Int32 Threshold { get; private set; }
		
        public TimeSpan Timeout { get; private set; }

		public Boolean IsClosed
		{
			get { return m_state is ClosedState; }
		}

		public Boolean IsOpen
		{
			get { return m_state is OpenState; }
		}

		public Boolean IsHalfOpen
		{
			get { return m_state is HalfOpenState; }
		}

		internal void MoveToClosedState()
		{
			m_state = new ClosedState(this);
		}

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = " ")]
        internal void MoveToOpenState()
		{
			m_state = new OpenState(this);
		}

		internal void MoveToHalfOpenState()
		{
			m_state = new HalfOpenState(this);
		}

		internal void IncreaseFailureCount()
		{
			Failures++;
		}

		internal void ResetFailureCount()
		{
			Failures = 0;
		}

		public Boolean ThresholdReached()
		{
			return Failures >= Threshold;
		}

		public void AttemptCall(Action protectedCode)
		{
            if (protectedCode == null)
            {
                throw new ArgumentNullException("protectedCode", "Action can not be null.");
            }

			using (TimedLock.Lock(m_lockObj)) 
			{
				m_state.ProtectedCodeIsAboutToBeCalled();
			}

			try
			{
				protectedCode();
			}
			catch (Exception e)
			{
				using (TimedLock.Lock(m_lockObj))
				{
					m_state.ActUponException(e);
				}

				throw;
			}

			using (TimedLock.Lock(m_lockObj))
			{
				m_state.ProtectedCodeHasBeenCalled();
			}
		}

		public void Close()
		{
			using (TimedLock.Lock(m_lockObj))
			{
				MoveToClosedState();
			}
		}

		public void Open()
		{
			using (TimedLock.Lock(m_lockObj))
			{
				MoveToOpenState();
			}
		}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CircuitBreaker() { Dispose(false); }

        private void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (m_state != null)
                {
                    OpenState openState = m_state as OpenState;

                    if (openState != null)
                    {
                        openState.Dispose();
                    }
                }
            }
        }
    }
}