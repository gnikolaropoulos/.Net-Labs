using System;
using System.Threading;
using System.SoftBytes.Exceptions;
using System.SoftBytes.Reliability;
using Xunit;

namespace System.SoftBytes.Tests.Reliability
{
    public sealed class CircuitBreakerTest
    {
        private static void CallXAmountOfTimes(Action codeToCall, Int32 timesToCall)
        {
            for (Int32 i = 0; i < timesToCall; i++)
            {
                codeToCall();
            }
        }

        [Fact]
        public void AttemptCallCallsProtectedCode()
        {
            Boolean protectedCodeWasCalled = false;
            Action protectedCode = () => protectedCodeWasCalled = true;

            var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMinutes(5));
            circuitBreaker.AttemptCall(protectedCode);
            Assert.True(protectedCodeWasCalled);
        }

        [Fact]
        public void FailuresIsNotIncreasedWhenProtectedCodeSucceeds()
        {
            Action protectedCode = () => { return; };

            var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMinutes(5));
            circuitBreaker.AttemptCall(protectedCode);
            Assert.Equal(0, circuitBreaker.Failures);
        }

        [Fact]
        public void ConstructorWithInvalidThresholdThrowsException()
        {
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				new CircuitBreaker(0, TimeSpan.FromMinutes(5));
			});

			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				new CircuitBreaker(-1, TimeSpan.FromMinutes(5));
			});
        }

        [Fact]
        public void ConstructorWithInvalidTimeoutThrowsException()
        {
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				new CircuitBreaker(10, TimeSpan.Zero);
			});

			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				new CircuitBreaker(10, TimeSpan.FromMilliseconds(-1));
			});
		}

        [Fact]
        public void FailuresIncreasesWhenProtectedCodeFails()
        {
            Action protectedCode = () => { throw new Exception("blah"); };

            var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMinutes(5));
            Assert.Equal(0, circuitBreaker.Failures);

			Assert.Throws<Exception>(() =>
			{
				circuitBreaker.AttemptCall(protectedCode);
			});
            Assert.Equal(1, circuitBreaker.Failures);
        }

        [Fact]
        public void NewCircuitBreakerIsClosed()
        {
            var circuitBreaker = new CircuitBreaker(5, TimeSpan.FromMinutes(5));
            Assert.True(circuitBreaker.IsClosed);
        }

		[Fact]
		public void OpensWhenThresholdIsReached()
		{
			Action protectedCode = () => { throw new Exception("blah"); };

			var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMinutes(5));

			CallXAmountOfTimes(() =>
				{
					try
					{
						circuitBreaker.AttemptCall(protectedCode);
					}
					catch
					{
						// Do nothing.
					}
				}, 10);

			Assert.True(circuitBreaker.IsOpen);
		}

        [Fact]
        public void TestConstructorWithValidArguments()
        {
            var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMinutes(5));
            Assert.Equal(10, circuitBreaker.Threshold);
            Assert.Equal(TimeSpan.FromMinutes(5), circuitBreaker.Timeout);
        }

		[Fact]
		public void ThrowsOpenCircuitExceptionWhenCallIsAttemptedIfCircuitBreakerIsOpen()
		{
			Action protectedCode = () => { throw new Exception("blah"); };

			var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMinutes(5));

			CallXAmountOfTimes(() =>
			{
				try
				{
					circuitBreaker.AttemptCall(protectedCode);
				}
				catch
				{
					// Do nothing.
				}
			}, 10);

			Assert.Throws<Exception<OpenCircuitExceptionArgs>>(() =>
			{
				circuitBreaker.AttemptCall(protectedCode);
			});
		}

		[Fact]
		public void SwitchesToHalfOpenWhenTimeOutIsReachedAfterOpening()
		{
			Action protectedCode = () => { throw new Exception("blah"); };

			var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMilliseconds(50));

			CallXAmountOfTimes(() =>
			{
				try
				{
					circuitBreaker.AttemptCall(protectedCode);
				}
				catch
				{
					// Do nothing.
				}
			}, 10);
			
			Thread.Sleep(100);
			Assert.True(circuitBreaker.IsHalfOpen);
		}

		[Fact]
		public void OpensIfExceptionIsThrownInProtectedCodeWhenInHalfOpenState()
		{
			Action protectedCode = () => { throw new Exception("blah"); };

			var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMilliseconds(50));

			CallXAmountOfTimes(() =>
			{
				try
				{
					circuitBreaker.AttemptCall(protectedCode);
				}
				catch
				{
					// Do nothing.
				}
			}, 10);

			Thread.Sleep(100);

			Assert.Throws<Exception>(() =>
			{
				circuitBreaker.AttemptCall(protectedCode);
			});

			Assert.True(circuitBreaker.IsOpen);
		}

		[Fact]
		public void ClosesIfProtectedCodeSucceedsInHalfOpenState()
		{
			var stub = new Stub(10);
			var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMilliseconds(50));

			CallXAmountOfTimes(() =>
			{
				try
				{
					circuitBreaker.AttemptCall(stub.DoStuff);
				}
				catch
				{
					// Do nothing.
				}
			}, 10);
			
			Thread.Sleep(100);
			circuitBreaker.AttemptCall(stub.DoStuff);
			Assert.True(circuitBreaker.IsClosed);
		}

		[Fact]
		public void FailuresIsResetWhenCircuitBreakerCloses()
		{
			var stub = new Stub(10);
			var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMilliseconds(50));

			CallXAmountOfTimes(() =>
			{
				try
				{
					circuitBreaker.AttemptCall(stub.DoStuff);
				}
				catch
				{
					// Do nothing.
				}
			}, 10);
			
			Assert.Equal(10, circuitBreaker.Failures);
			Thread.Sleep(100);
			circuitBreaker.AttemptCall(stub.DoStuff);
			Assert.Equal(0, circuitBreaker.Failures);
		}

		[Fact]
		public void CanCloseCircuitBreaker()
		{
		    Action protectedCode = () => { throw new Exception("blah"); };

		    var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMilliseconds(50));

			CallXAmountOfTimes(() =>
			{
				try
				{
					circuitBreaker.AttemptCall(protectedCode);
				}
				catch
				{
					// Do nothing.
				}
			}, 10);

		    Assert.True(circuitBreaker.IsOpen);
		    circuitBreaker.Close();
		    Assert.True(circuitBreaker.IsClosed);
		}

        [Fact]
        public void CanOpenCircuitBreaker()
        {
            var circuitBreaker = new CircuitBreaker(10, TimeSpan.FromMilliseconds(50));
            Assert.True(circuitBreaker.IsClosed);
            circuitBreaker.Open();
            Assert.True(circuitBreaker.IsOpen);
        }

        #region SUT

        private sealed class Stub
        {
            private readonly Int32 m_timesToFail;
            private Int32 m_counter;

            public Stub(Int32 timesToFail)
            {
                m_counter = 0;
                m_timesToFail = timesToFail;
            }

            public void DoStuff()
            {
                if (++m_counter <= m_timesToFail)
                {
                    throw new Exception("from DoStuff");
                }
            }
        }

        #endregion
    }
}
