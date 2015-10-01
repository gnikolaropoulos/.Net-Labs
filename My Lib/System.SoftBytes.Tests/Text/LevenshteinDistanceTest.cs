using System;
using System.SoftBytes.Text;
using Xunit;

namespace System.SoftBytes.Tests.Text
{
    public sealed class LevenshteinDistanceTest
    {
		[Fact]
		public void SutWillComputeDistance()
		{
			// Fixture setup.
			// Exercise system.
			Int32 dist = LevenshteinDistance.Compute("Manchester United", "Man Utd");
			
			// Verify outcome.
			Assert.Equal(10, dist);
			
			// Teardown.
		}
    }
}
