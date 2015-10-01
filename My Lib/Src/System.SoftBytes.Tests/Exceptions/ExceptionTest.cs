using System;
using System.SoftBytes.Exceptions;
using Xunit;

namespace System.SoftBytes.Tests.Exceptions
{
    public sealed class ExceptionTest
    {
        [Fact]
        public static void ExceptionMessage()
        {
            String message = "The disk is full.";
            try
            {
                throw new Exception<DiskFullExceptionArgs>(
                  new DiskFullExceptionArgs(@"C:\"), message);
            }
            catch (Exception<DiskFullExceptionArgs> e)
            {
                Assert.True(e.Message.Contains(message));
            }
        }

		[Fact]
		public static void ThrowException()
		{
			Assert.Throws<Exception<DiskFullExceptionArgs>>(() =>
			{
				throw new Exception<DiskFullExceptionArgs>(
				   new DiskFullExceptionArgs(@"C:\"));
			});
		}

        #region SUT

        private sealed class DiskFullExceptionArgs : ExceptionArgs
        {
            private readonly String m_diskpath; // Private field set at construction time.

            public DiskFullExceptionArgs(String diskpath) { m_diskpath = diskpath; }

            // Public read-only property that returns the field.
            public String DiskPath { get { return m_diskpath; } }

            // Override the Message property to include our field (if set).
            public override String Message
            {
                get
                {
                    return (m_diskpath == null) ? base.Message : "DiskPath=" + m_diskpath;
                }
            }
        }

        #endregion
    }
}
