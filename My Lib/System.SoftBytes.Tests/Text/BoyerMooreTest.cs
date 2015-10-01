using System;
using System.Linq;
using System.SoftBytes.Text;
using Xunit;

namespace System.SoftBytes.Tests.Text
{
    public sealed class BoyerMooreTest
    {
        [Fact]
        public void SutWillReturnCorrectIndexForMunich()
        {
            // Fixture setup.
            String pattern = "Munich";
            String[] names = new String[] { "B Munich", "B-Munich", "B Munich I", "B Munich-I", "B.Munich-I" };
            BoyerMoore sut = new BoyerMoore(pattern);

            foreach (String name in names)
            {
                // Exercise system.
                foreach (Int32 index in sut.ApostolicoGiancarloMatch(name))
                {
                    // Verify outcome.
                    Assert.Equal(2, index); 
					break;
                }

                // Exercise system.
                foreach (Int32 index in sut.BclMatch(name))
                {
                    // Verify outcome.
                    Assert.Equal(2, index); 
					break;
                }

                // Exercise system.
                foreach (Int32 index in sut.BoyerMooreMatch(name))
                {
                    // Verify outcome.
                    Assert.Equal(2, index);
					break;
                }

                // Exercise system.
                foreach (Int32 index in sut.HorspoolMatch(name))
                {
                    // Verify outcome.
                    Assert.Equal(2, index); 
					break;
                }

                // Exercise system.
                foreach (Int32 index in sut.TurboBoyerMooreMatch(name))
                {
                    // Verify outcome.
                    Assert.Equal(2, index);
					break;
                }
            }

            // Teardown.
        }

		[Fact]
		public void SutWillReturnCorrectIndex()
		{
			// Fixture setup.
			String pattern = "Munich";
			String[] names = new String[] { "B Munich", "B-Munich", "B Munich I", "B Munich-I", "B.Munich-I" };
			BoyerMoore sut = new BoyerMoore(pattern);

			foreach (String name in names)
			{
				// Exercise system.
				foreach (Int32 index in sut.ApostolicoGiancarloMatch(name))
				{
					// Verify outcome.
					Assert.Equal(2, index);
					break;
				}

				// Exercise system.
				foreach (Int32 index in sut.BclMatch(name))
				{
					// Verify outcome.
					Assert.Equal(2, index);
					break;
				}

				// Exercise system.
				foreach (Int32 index in sut.BoyerMooreMatch(name))
				{
					// Verify outcome.
					Assert.Equal(2, index); 
					break;
				}

				// Exercise system.
				foreach (Int32 index in sut.HorspoolMatch(name))
				{
					// Verify outcome.
					Assert.Equal(2, index);
					break;
				}

				// Exercise system.
				foreach (Int32 index in sut.TurboBoyerMooreMatch(name))
				{
					// Verify outcome.
					Assert.Equal(2, index);
					break;
				}
			}

			// Teardown.
		}

        [Fact]
        public void SutWillReturnZeroIndex()
        {
            // Fixture setup.
            String pattern = "123";
            String[] names = new String[] { "B Munich", "B-Munich", "B Munich I", "B Munich-I", "B.Munich-I" };
            BoyerMoore sut = new BoyerMoore(pattern);

            // Exercise system and verify outcome.
            foreach (String name in names)
            {
                Assert.Equal(0, sut.ApostolicoGiancarloMatch(name).Count());
                Assert.Equal(0, sut.BclMatch(name).Count());
                Assert.Equal(0, sut.BoyerMooreMatch(name).Count());
                Assert.Equal(0, sut.HorspoolMatch(name).Count());
                Assert.Equal(0, sut.TurboBoyerMooreMatch(name).Count());
            }

            // Teardown.
        }
    }
}
