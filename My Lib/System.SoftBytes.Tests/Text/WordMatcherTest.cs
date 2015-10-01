using System;
using System.SoftBytes.Text;
using Xunit;

namespace System.SoftBytes.Tests.Text
{
    public sealed class WordMatcherTest
    {
        [Fact]
        public void SutWillMatchNFL()
        {
            // Fixture setup.
            // Exercise system.
            Boolean matched = WordMatcher.Matches("(02/04) Everton v Aston Villa (TV)", "Everton v Aston Villa");

            // Verify outcome.
            Assert.True(matched);

            // Teardown.
        }
        
        [Fact]
        public void SutWillMatchVllaznia()
        {
            // Fixture setup.
            // Exercise system.
            Boolean matched = WordMatcher.Matches("KS Vllaznia Shko", "KS Vllaznia Shkodër");

            // Verify outcome.
            Assert.True(matched);

            // Teardown.
        }

        [Fact]
        public void SutWillNotMatchShko()
        {
            // Fixture setup.
            // Exercise system.
            Boolean matched = WordMatcher.Matches("KS Shko", "KS Vllaznia");

            // Verify outcome.
            Assert.False(matched);

            // Teardown.
        }

        [Fact]
        public void SutWillMatchSkenderbeu()
        {
            // Fixture setup.
            // Exercise system.
            Boolean matched = WordMatcher.Matches("KS Skënderbeu Korcë", "Skenderbeu Korce");

            // Verify outcome.
            Assert.True(matched);

            // Teardown.
        }

        [Fact]
        public void SutWillMatchSkenderbeuReversed()
        {
            // Fixture setup.
            // Exercise system.
            Boolean matched = WordMatcher.Matches("Skenderbeu Korce", "KS Skënderbeu Korcë");

            // Verify outcome.
            Assert.True(matched);

            // Teardown.
        }

        [Fact]
        public void SutWillMatchManDotUtdWithManUtd()
        {
            // Fixture setup.
            // Exercise system.
            Boolean matched = WordMatcher.Matches("Man. Utd", "Man Utd");
            
            // Verify outcome.
            Assert.True(matched);

            // Teardown.
        }

        [Fact]
        public void SutWillMatchManDotUtdWithManDotUtd()
        {
            // Fixture setup.
            // Exercise system.
            Boolean matched = WordMatcher.Matches("Man. Utd", "Man. Utd");

            // Verify outcome.
            Assert.True(matched);

            // Teardown.
        }

        [Fact]
        public void SutWillNotMatchManDotUtdWithManCity()
        {
            // Fixture setup.
            // Exercise system.
            Boolean matched = WordMatcher.Matches("Man. Utd", "Man City");

            // Verify outcome.
            Assert.False(matched);

            // Teardown.
        }

        [Fact]
        public void SutWillNotMatchManDotUtdWithCityMan()
        {
            // Fixture setup.
            // Exercise system.
            Boolean matched = WordMatcher.Matches("Man. Utd", "City Man");

            // Verify outcome.
            Assert.False(matched);

            // Teardown.
        }

        [Fact]
        public void SutWillNotMatchManDotUtdWithUtdMan()
        {
            // Fixture setup.
            // Exercise system.
            Boolean matched = WordMatcher.Matches("Man. Utd", "Utd Man");

            // Verify outcome.
            Assert.False(matched);

            // Teardown.
        }

        [Fact]
        public void SutWillNotMatchOlympiakosWithOlympiaBolou()
        {
            // Fixture setup.
            // Exercise system.
            Boolean matched = WordMatcher.Matches("Olympiakos", "Olympia Bolou");

            // Verify outcome.
            Assert.False(matched);

            // Teardown.
        }
    }
}
