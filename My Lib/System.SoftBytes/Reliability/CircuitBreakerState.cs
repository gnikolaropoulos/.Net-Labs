//-------------------------------------------------------------------------------------------------
// Code from Davy Brion http://davybrion.com/blog/2008/05/the-circuit-breaker/
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.SoftBytes.Exceptions;

namespace System.SoftBytes.Reliability
{
    public abstract class CircuitBreakerState
    {
        private CircuitBreaker m_circuitBreaker;

        protected CircuitBreakerState(CircuitBreaker circuitBreaker)
        {
            m_circuitBreaker = circuitBreaker;
        }

        protected CircuitBreaker CircuitBreaker
        {
            get
            {
                return m_circuitBreaker;
            }

            set
            {
                m_circuitBreaker = value;
            }
        }

        public virtual void ProtectedCodeIsAboutToBeCalled() { }

        public virtual void ProtectedCodeHasBeenCalled() { }

        public virtual void ActUponException(Exception e) { CircuitBreaker.IncreaseFailureCount(); }
    }

    public sealed class ClosedState : CircuitBreakerState
    {
        public ClosedState(CircuitBreaker circuitBreaker)
            : base(circuitBreaker)
        {
            if (circuitBreaker == null)
            {
                throw new ArgumentNullException("circuitBreaker");
            }

            circuitBreaker.ResetFailureCount();
        }

        public override void ActUponException(Exception e)
        {
            base.ActUponException(e);

            if (CircuitBreaker.ThresholdReached()) { CircuitBreaker.MoveToOpenState(); }
        }
    }

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification="Periodic signaling is disabled")]
    public sealed class OpenState : CircuitBreakerState, IDisposable
    {
        private readonly Timer m_timer;

        public OpenState(CircuitBreaker circuitBreaker)
            : base(circuitBreaker)
        {
            if (circuitBreaker == null)
            {
                throw new ArgumentNullException("circuitBreaker");
            }

            m_timer = new Timer(TimeoutHasBeenReached, null, 
                circuitBreaker.Timeout, TimeSpan.FromMilliseconds(-1));
        }

        private void TimeoutHasBeenReached(Object state)
        {
            CircuitBreaker.MoveToHalfOpenState();
        }

        public override void ProtectedCodeIsAboutToBeCalled()
        {
            base.ProtectedCodeIsAboutToBeCalled();

            throw new Exception<OpenCircuitExceptionArgs>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OpenState() { Dispose(false); }

        private void Dispose(Boolean disposing)
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

    public sealed class OpenCircuitExceptionArgs : ExceptionArgs
    {
        public override String Message
        {
            get
            {
                return "The protected code can not be called while the circuit is open.";
            }
        }
    }

    public sealed class HalfOpenState : CircuitBreakerState
    {
        public HalfOpenState(CircuitBreaker circuitBreaker) : base(circuitBreaker) { }

        public override void ActUponException(Exception e)
        {
            base.ActUponException(e);

            CircuitBreaker.MoveToOpenState();
        }

        public override void ProtectedCodeHasBeenCalled()
        {
            base.ProtectedCodeHasBeenCalled();

            CircuitBreaker.MoveToClosedState();
        }
    }
}