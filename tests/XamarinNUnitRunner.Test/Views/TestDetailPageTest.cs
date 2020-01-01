using System;
using System.Threading;
using NUnit.Framework;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Services;
using XamarinNUnitRunner.Test.Stub;
using XamarinNUnitRunner.Views;

namespace XamarinNUnitRunner.Test.Views
{
    [TestFixture]
    public class TestDetailPageTest
    {
        // Test for ViewModel Property covered by tests for the Constructor

        #region Tests for Constructors

        [Test]
        public void TestConstructorWithNUnitRunner([Values] bool isRunnerNull, [Values] bool isTestNull)
        {
            NUnitRunner runner = isRunnerNull ? null : new NUnitRunner("runner-name");
            NUnitTest test = isTestNull ? null : new NUnitTest(new NUnitSuite("suite-name"));

            TestDetailPage page = new TestDetailPage(runner, test, false);

            Assert.IsNotNull(page.ViewModel);
            Assert.AreSame(runner, page.ViewModel.TestRunner);
            Assert.AreSame(test, page.ViewModel.Test);
        }

        #endregion

        #region Tests for RunTestsClicked

        [Test]
        public void TestRunTestsClickedRunsTests()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            NUnitTest test = new NUnitTest(new NUnitSuite("suite-name"));

            TestDetailPageForTest page = new TestDetailPageForTest(runner, test);

            Assert.IsNull(page.ViewModel.Result);

            page.InvokeRunTestsClicked(this, EventArgs.Empty);

            while (page.ViewModel.IsBusy)
            {
                Thread.Sleep(10);
            }

            Assert.IsNotNull(page.ViewModel.Result);
        }

        #endregion

        #region Nested Class: TestDetailPageForTest

        private class TestDetailPageForTest : TestDetailPage
        {
            #region Constructor

            /// <inheritdoc />
            public TestDetailPageForTest(INUnitRunner runner, NUnitTest test) : base(runner, test, false)
            {
            }

            #endregion

            #region Public Methods

            /// <inheritdoc cref="TestDetailPage.RunTestsClicked" />
            public void InvokeRunTestsClicked(object sender, EventArgs e)
            {
                RunTestsClicked(sender, e);
            }

            #endregion
        }

        #endregion
    }
}