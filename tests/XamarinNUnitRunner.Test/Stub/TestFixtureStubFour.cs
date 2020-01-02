using System;
using NUnit.Framework;

namespace XamarinNUnitRunner.Test.Stub
{
    [TestFixture]
    public class TestFixtureStubFour : ITestFixtureStub
    {
        #region Implementation of ITestFixtureStub

        /// <inheritdoc />
        public int TestCount { get; } = 1;

        /// <inheritdoc />
        public int ResultCount { get; } = 1;

        /// <inheritdoc />
        public int ResultsDepth => TestFixtureStubHelper.CountResultsDepth(typeof(TestFixtureStubOne));

        #endregion

        #region Test SetUp/TearDown

        [SetUp]
        public void Setup()
        {
            throw new InvalidOperationException("Test setup exception.");
        }

        [TearDown]
        public void Teardown()
        {
        }

        #endregion

        #region Test Methods

        [Test]
        public void SingleTest()
        {
            Assert.IsTrue(true);
        }

        #endregion
    }
}