using System;
using System.Threading;
using NUnit.Framework;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Services;
using XamarinNUnitRunner.Test.Stub;
using XamarinNUnitRunner.ViewModels;
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
            TestsViewModel test = isTestNull ? null : new TestsViewModel(runner, new NUnitSuite("suite-name"));

            TestDetailPage page = new TestDetailPage(test, false);

            Assert.AreSame(test, page.ViewModel);
            Assert.AreSame(test?.TestRunner, page.ViewModel?.TestRunner);
            Assert.AreSame(test?.Test, page.ViewModel?.Test);
        }

        #endregion

        #region Tests for RunTestsClicked

        [Test]
        public void TestRunTestsClickedRunsTests()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            TestsViewModel test = new TestsViewModel(runner, new NUnitSuite("suite-name"));

            TestDetailPageForTest page = new TestDetailPageForTest(test);

            Assert.IsNotNull(page.ViewModel.Result);
            Assert.IsNull(page.ViewModel.Result.Result);

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
            public TestDetailPageForTest(TestsViewModel test) : base(test, false)
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