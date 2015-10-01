using System;
using System.Collections.Generic;
using System.Threading;
using System.SoftBytes.Exceptions;
using System.SoftBytes.Reliability;
using Xunit;

namespace System.SoftBytes.Tests.Reliability
{
    public sealed class BlockingCallerTest
    {
        private static readonly TimeSpan s_defaultWaiting = TimeSpan.FromSeconds(5);

        [Fact]
        public void WithThrottle()
        {
            Int32 maxNumberOfCalls = FakeSoapApi.c_maxNumTimesCall;
            Int32 actualNumOfCalls = maxNumberOfCalls * 2;

            Assert.DoesNotThrow(() =>
            {
                using (var throttle = new Throttle(maxNumberOfCalls, s_defaultWaiting))
                {
                    for (Int32 i = 0; i < actualNumOfCalls; i++)
                    {
                        try
                        {
                            throttle.Call(() => {
                                new FakeSoapApi().DoSomething();
                            });
                        }
                        catch (Exception<FakeSoapApiExceptionArgs>)
                        {
                            throttle.Wait();
                        }
                    }
                }
            });
        }

        [Fact]
        public void WithThrottle_DiscardCallsOnTimeout()
        {
            Int32 maxNumberOfCalls = FakeSoapApi.c_maxNumTimesCall;
            Int32 actualNumOfCalls = maxNumberOfCalls * 2;

            var values = new List<Int32>();

            Assert.DoesNotThrow(() =>
            {
                using (var throttle = new Throttle(maxNumberOfCalls, s_defaultWaiting))
                {
                    throttle.DiscardCallsOnTimeout(true);

                    var api = new FakeSoapApi();
                    for (Int32 i = 0; i < actualNumOfCalls; i++)
                    {
                        try
                        {
                            throttle.Call(() => {
                                values.Add(api.DoSomething());
                            });
                        }
                        catch (Exception<FakeSoapApiExceptionArgs>)
                        {
                            throttle.Wait();
                        }
                    }
                }
            });

            Assert.True(values.Count < actualNumOfCalls);
        }

        #region SUT

        private sealed class FakeSoapApiExceptionArgs : ExceptionArgs { }

        private sealed class FakeSoapApi : IDisposable
        {
            public const Int32 c_maxNumTimesCall = 100;
            public const Int32 c_maxNumTimesCallCombined = 150;

            private static Int32 s_totalNumTimesCall;
            private static Int32 s_totalNumTimesCallCombined;

            private static Timer s_timer = new Timer(
                delegate
                {
                    Interlocked.Exchange(ref s_totalNumTimesCall, 0);
                    Interlocked.Exchange(ref s_totalNumTimesCallCombined, 0);
                },
                null,
                TimeSpan.Zero,
                s_defaultWaiting);

            private Int32 m_nextInt32;

            public Int32 DoSomething()
            {
                CombinedCallCountCheck();

                if (s_totalNumTimesCall >= c_maxNumTimesCall)
                {
                    throw new Exception<FakeSoapApiExceptionArgs>("Call limit reached.");
                }

                s_totalNumTimesCall++;

                return m_nextInt32++;
            }

            public void DoSomethingElse()
            {
                // This method has no specific call limit. It uses the shared limit instead.
                CombinedCallCountCheck();
            }

            private void CombinedCallCountCheck()
            {
                if (s_totalNumTimesCallCombined >= c_maxNumTimesCallCombined)
                {
                    throw new Exception<FakeSoapApiExceptionArgs>("Count of all calls made to the api reached.");
                }

                s_totalNumTimesCallCombined++;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            ~FakeSoapApi() { Dispose(false); }

            private void Dispose(Boolean disposing)
            {
                if (disposing)
                {
                    if (s_timer != null)
                    {
                        s_timer.Dispose();
                    }
                }
            }
        }

        #endregion
    }
}
