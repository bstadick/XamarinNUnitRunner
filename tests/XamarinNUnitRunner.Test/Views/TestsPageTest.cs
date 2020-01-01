using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Xamarin.Forms;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Services;
using XamarinNUnitRunner.Test.Stub;
using XamarinNUnitRunner.Views;

namespace XamarinNUnitRunner.Test.Views
{
    [TestFixture]
    public class TestsPageTest
    {
        // Test for ViewModel Property covered by tests for the Constructor

        #region Tests for Constructor

        [Test]
        public void TestConstructorWithNUnitRunner([Values] bool isRunnerNull, [Values] bool isTestNull)
        {
            NUnitRunner runner = isRunnerNull ? null : new NUnitRunner("runner-name");
            NUnitTest test = isTestNull ? null : new NUnitTest(new NUnitSuite("suite-name"));

            TestsPage page = new TestsPage(runner, test, false);

            Assert.IsNotNull(page.ViewModel);
            Assert.AreSame(runner, page.ViewModel.TestRunner);
            Assert.AreSame(test, page.ViewModel.Test);
        }

        #endregion

        #region Tests for OnAppearing

        [Test]
        public void TestOnAppearingLoadsTests()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);

            TestsPageForTest page = new TestsPageForTest(runner);

            CollectionAssert.IsEmpty(page.ViewModel.Tests);

            page.InvokeOnAppearing();

            while (page.ViewModel.IsBusy)
            {
                Thread.Sleep(10);
            }

            CollectionAssert.IsNotEmpty(page.ViewModel.Tests);

            IList<NUnitTest> tests = new List<NUnitTest>(page.ViewModel.Tests);

            runner.AddTestAssembly(GetType().Assembly);

            page.InvokeOnAppearing();

            while (page.ViewModel.IsBusy)
            {
                Thread.Sleep(10);
            }

            CollectionAssert.AreEquivalent(tests, page.ViewModel.Tests);
        }

        #endregion

        #region Tests for OnItemSelected

        [Test]
        public void TestOnItemSelectedWithEventArgsOrSelectedItemNullReturnsImmediately([Values] bool isArgNull)
        {
            SelectedItemChangedEventArgs args = isArgNull ? null : new SelectedItemChangedEventArgs(null, 0);

            NUnitRunner runner = new NUnitRunner("runner-name");

            TestsPageForTest page = new TestsPageForTest(runner);

            page.InvokeOnItemSelected(null, args);

            CollectionAssert.IsEmpty(page.NavigationStack);
        }

        [Test]
        public void TestOnItemSelectedWithSelectedItemNotNUnitTestTypeReturnsImmediately()
        {
            SelectedItemChangedEventArgs args = new SelectedItemChangedEventArgs("Hello", 0);

            NUnitRunner runner = new NUnitRunner("runner-name");

            TestsPageForTest page = new TestsPageForTest(runner);

            page.InvokeOnItemSelected(null, args);

            CollectionAssert.IsEmpty(page.NavigationStack);
        }

        [Test]
        public void TestOnItemSelectedWithSelectedItemTestIsNullReturnsImmediately()
        {
            NUnitTest test = new NUnitTest(null);
            SelectedItemChangedEventArgs args = new SelectedItemChangedEventArgs(test, 0);

            NUnitRunner runner = new NUnitRunner("runner-name");

            TestsPageForTest page = new TestsPageForTest(runner);

            page.InvokeOnItemSelected(null, args);

            CollectionAssert.IsEmpty(page.NavigationStack);
        }

        [Test]
        public void TestOnItemSelectedWithSelectedItemTestWithChildTestsPushesTestsPageToStack()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            NUnitTest test = new NUnitTest(runner.ExploreTests());

            Assert.IsTrue(test.Test.HasChildren);

            SelectedItemChangedEventArgs args = new SelectedItemChangedEventArgs(test, 0);

            TestsPageForTest page = new TestsPageForTest(runner);

            page.InvokeOnItemSelected(null, args);

            Assert.AreEqual(1, page.NavigationStack.Count);
            TestsPage testsPage = page.NavigationStack.First() as TestsPage;
            Assert.IsNotNull(testsPage);
            Assert.AreEqual(runner, testsPage.ViewModel.TestRunner);
            Assert.AreEqual(test, testsPage.ViewModel.Test);
        }

        [Test]
        public void TestOnItemSelectedWithSelectedItemTestWithoutChildTestsPushesTestDetailPageToStack()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            NUnitTest test = new NUnitTest(new TestSuite("suite-name"));

            Assert.IsFalse(test.Test.HasChildren);

            SelectedItemChangedEventArgs args = new SelectedItemChangedEventArgs(test, 0);

            TestsPageForTest page = new TestsPageForTest(runner);

            page.InvokeOnItemSelected(null, args);

            Assert.AreEqual(1, page.NavigationStack.Count);
            TestDetailPage testsPage = page.NavigationStack.First() as TestDetailPage;
            Assert.IsNotNull(testsPage);
            Assert.AreEqual(runner, testsPage.ViewModel.TestRunner);
            Assert.AreEqual(test, testsPage.ViewModel.Test);
        }

        #endregion

        #region Tests for RunTestsClicked

        [Test]
        public void TestRunTestsClickedRunsTests()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            NUnitTest test = new NUnitTest(new NUnitSuite("suite-name"));

            TestsPageForTest page = new TestsPageForTest(runner, test);

            Assert.IsNull(page.ViewModel.Result);

            page.InvokeRunTestsClicked(this, EventArgs.Empty);

            while (page.ViewModel.IsBusy)
            {
                Thread.Sleep(10);
            }

            Assert.IsNotNull(page.ViewModel.Result);
        }

        #endregion

        #region Nested Class: TestsPageForTest

        private class TestsPageForTest : TestsPage
        {
            #region Public Members

            /// <summary>
            ///     Gets the navigation stack of the page.
            /// </summary>
            public IList<Page> NavigationStack { get; } = new List<Page>();

            #endregion

            #region Constructor

            /// <inheritdoc />
            public TestsPageForTest(INUnitRunner runner, NUnitTest test = null) : base(runner, test, false)
            {
            }

            #endregion

            #region Public Methods

            /// <inheritdoc cref="TestsPage.OnAppearing" />
            public void InvokeOnAppearing()
            {
                OnAppearing();
            }

            /// <inheritdoc cref="TestsPage.OnItemSelected" />
            public void InvokeOnItemSelected(object sender, SelectedItemChangedEventArgs args)
            {
                OnItemSelected(sender, args);
            }

            /// <inheritdoc cref="TestsPage.RunTestsClicked" />
            public void InvokeRunTestsClicked(object sender, EventArgs e)
            {
                RunTestsClicked(sender, e);
            }

            #endregion

            #region Protected Methods

            /// <inheritdoc />
            protected override void NavigatePushAsync(Page page)
            {
                NavigationStack.Add(page);
            }

            #endregion
        }

        #endregion
    }
}