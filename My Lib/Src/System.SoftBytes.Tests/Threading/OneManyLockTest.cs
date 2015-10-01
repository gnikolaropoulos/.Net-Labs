using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.SoftBytes.Threading;
using Xunit;

namespace System.SoftBytes.Tests.Threading
{
    public sealed class OneManyLockTest
    {
        private const Int32 c_iterations = 10000000; // 10 million.

        [Fact]
        public void OneManyLock()
        {
            Int32 x = 0;
            Assert.DoesNotThrow(delegate
            {
                using (var oml = new OneManyLock())
                {
                    oml.Enter(true); 
                    x++; 
                    oml.Leave(true);
                    for (Int32 i = 0; i < c_iterations; i++)
                    {
                        oml.Enter(true); 
                        x++; 
                        oml.Leave(true);
                    }
                }
            });
        }

        [Fact(Skip = @"Test fails if running with debugger attached and/or in debug.
                       Test fails if running on VMware Workstation using Win2008 R2.")]
        public void OneManyLockIsFasterThanReaderWriterLockSlim()
        {
            // OneManyLock.
            var sw = Stopwatch.StartNew();
            using (var oml = new OneManyLock())
            {
                oml.Enter(true); 
                M(); 
                oml.Leave(true);
                for (Int32 i = 0; i < c_iterations; i++)
                {
                    oml.Enter(true); 
                    M();
                    oml.Leave(true);
                }
            }

            Int64 omlElapsedMilliseconds = sw.ElapsedMilliseconds;

            // ReaderWriterLockSlim.
            sw.Restart();
            using (var rwls = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion))
            {
                rwls.EnterReadLock();
                M();
                rwls.ExitReadLock();
                for (Int32 i = 0; i < c_iterations; i++)
                {
                    rwls.EnterReadLock();
                    M();
                    rwls.ExitReadLock();
                }
            }

            Int64 rwlsElapsedMilliseconds = sw.ElapsedMilliseconds;

            Assert.True(omlElapsedMilliseconds < rwlsElapsedMilliseconds);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void M() { }
    }
}
