using NUnit.Framework;

namespace XamarinNUnitRunner.Test.Stub
{
    [TestFixture]
    public class TestFixtureStubTwo : ITestFixtureStub
    {
        #region Implementation of ITestFixtureStub

        /// <inheritdoc />
        public int TestCount { get; } = 1;

        /// <inheritdoc />
        public int ResultCount { get; } = 1;

        /// <inheritdoc />
        public int ResultsDepth => TestFixtureStubHelper.CountResultsDepth(typeof(TestFixtureStubOne));

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