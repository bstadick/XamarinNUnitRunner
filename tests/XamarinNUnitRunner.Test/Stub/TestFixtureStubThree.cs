using NUnit.Framework;

namespace XamarinNUnitRunner.Test.Stub
{
    [TestFixture]
    public class TestFixtureStubThree : ITestFixtureStub
    {
        #region Implementation of ITestFixtureStub

        /// <inheritdoc />
        public int TestCount { get; } = 0;

        /// <inheritdoc />
        public int ResultCount { get; } = 0;

        /// <inheritdoc />
        public int ResultsDepth => TestFixtureStubHelper.CountResultsDepth(typeof(TestFixtureStubOne));

        #endregion
    }
}