//-------------------------------------------------------------------------------------------------
// Code from Davy Brion http://davybrion.com/blog/2008/05/the-circuit-breaker/
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.SoftBytes.Exceptions;

namespace System.SoftBytes.Threading
{
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = " ")]
    public struct TimedLock : IDisposable
	{
		private readonly Object m_target;

		private TimedLock(Object o)
		{
			m_target = o;
		}

        public static TimedLock Lock(Object o)
        {
            return Lock(o, TimeSpan.FromSeconds(5));
        }

        public static TimedLock Lock(Object o, TimeSpan timeout)
        {
            return Lock(o, Convert.ToInt32(timeout.TotalMilliseconds));
        }

        public static TimedLock Lock(Object o, Int32 milliseconds)
        {
            var timedLock = new TimedLock(o);

            if (!Monitor.TryEnter(o, milliseconds))
            {
                throw new Exception<LockTimeoutExceptionArgs>();
            }

            return timedLock;
        }

		public void Dispose()
		{
			Monitor.Exit(m_target);
		}
	}

	public sealed class LockTimeoutExceptionArgs : ExceptionArgs
	{
        public override string Message
        {
            get
            {
                return "Timeout waiting for lock.";
            }
        }
	}
}