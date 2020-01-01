using System.Threading;
using NUnit.Framework;

namespace XamarinNUnitRunner.Test.Stub
{
    [TestFixture]
    [Category("TestFixtureCategory")]
    public class TestFixtureStubOne
    {
        #region Public Members

        /// <summary>
        /// The number of test cases.
        /// </summary>
        public const int TestCount = 17;

        /// <summary>
        /// The number of test case results including parent results.
        /// </summary>
        public const int ResultCount = 19;

        /// <summary>
        /// The depth of the test result. One level for the DLL and each namespace including the class name.
        /// </summary>
        public const int ResultsDepth = 5;

        #endregion

        #region Test SetUp/TearDown

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Teardown()
        {
        }

        #endregion

        #region Test Methods

        [Test]
        public void Test1()
        {
            // Basic test
            Assert.IsTrue(true);
        }

        [Test]
        public void Test2()
        {
            // Basic test to filter on
            Assert.IsTrue(true);
        }

        [Test]
        public void Test3()
        {
            // Failing test
            Assert.Fail();
        }

        [Test]
        [Ignore("To be ignored")]
        public void Test4()
        {
            // Ignore test
            Assert.IsTrue(true);
        }

        [Test]
        [Category("Test5Category")]
        public void Test5()
        {
            // Test with Category
            Assert.IsTrue(true);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void Test6(int value)
        {
            // Test with Test Cases
            // Counts as 2 in test count and 3 in results count
            Assert.IsTrue(true);
        }

        [Test]
        public void Test7([Range(0, 2)] int value1, [Range(0, 2)] int value2)
        {
            // Test with Test Cases as combinatorial
            // Counts as 9 in test count and 3 in results count
            Assert.IsTrue(true);
        }

        [Test]
        public void Test8()
        {
            // Long duration test
            Thread.Sleep(500);
            Assert.IsTrue(true);
        }

        #endregion
    }
}