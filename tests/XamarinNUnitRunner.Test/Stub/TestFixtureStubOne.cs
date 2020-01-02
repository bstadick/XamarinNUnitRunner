using System;
using System.Threading;
using NUnit.Framework;

namespace XamarinNUnitRunner.Test.Stub
{
    [TestFixture]
    [Category("TestFixtureCategory")]
    public class TestFixtureStubOne : ITestFixtureStub
    {
        #region Implementation of ITestFixtureStub

        /// <inheritdoc />
        public int TestCount { get; } = 18;

        /// <inheritdoc />
        public int ResultCount { get; } = 20;

        /// <inheritdoc />
        public int ResultsDepth => TestFixtureStubHelper.CountResultsDepth(typeof(TestFixtureStubOne));

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
            // Basic test with output
            Console.WriteLine("Test message to display.");
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

        [Test]
        public void Test9()
        {
            // Test that throws exception
            throw new InvalidOperationException("Test exception.");
        }

        #endregion
    }
}