using System;
using System.SoftBytes.Exceptions;
using Xunit;

namespace System.SoftBytes.Tests.Exceptions
{
    public sealed class ExceptionServicesTest
    {
        [Fact]
        public void ShouldFlattenException()
        {
            Exception e = new Exception("Step 1",
                new ArgumentException("Step 2",
                    new NullReferenceException("Step 3",
                        new InvalidOperationException("Step 4",
                            new NotImplementedException("Step 5")))));

            AggregateException exception = e.Flatten();
            Assert.Equal(5, exception.InnerExceptions.Count);
        }
    }
}
