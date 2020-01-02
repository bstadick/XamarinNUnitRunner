using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using Xamarin.Forms;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Services;
using XamarinNUnitRunner.Test.Stub;
using XamarinNUnitRunner.ViewModels;
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
            TestsViewModel test = isTestNull ? null : new TestsViewModel(runner, new NUnitSuite("suite-name"));

            TestsPage page = new TestsPage(runner, test, false);

            Assert.IsNotNull(page.ViewModel);
            if (!isTestNull)
            {
                Assert.AreSame(test, page.ViewModel);
            }

            Assert.AreSame(runner, page.ViewModel.TestRunner);
            Assert.AreSame(test?.Test, page.ViewModel.Test);
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
            Assert.IsNull(page.ViewModel?.Result?.Result);

            page.InvokeOnAppearing();

            while (page.ViewModel.IsBusy)
            {
                Thread.Sleep(10);
            }

            CollectionAssert.IsNotEmpty(page.ViewModel.Tests);

            page.ViewModel.Result = new NUnitTestResult(new TestSuiteResult(runner.TestSuite));

            IList<TestsViewModel> tests = new List<TestsViewModel>(page.ViewModel.Tests);

            runner.AddTestAssembly(GetType().Assembly);

            page.InvokeOnAppearing();

            while (page.ViewModel.IsBusy)
            {
                Thread.Sleep(10);
            }

            CollectionAssert.AreEquivalent(tests, page.ViewModel.Tests);
            Assert.IsNotNull(page.ViewModel.Result.Result);
            CollectionAssert.IsNotEmpty(page.ViewModel.Tests.Select(x => x.Result));
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
        public void TestOnItemSelectedWithSelectedItemNotTestTypeReturnsImmediately()
        {
            SelectedItemChangedEventArgs args = new SelectedItemChangedEventArgs("Hello", 0);

            NUnitRunner runner = new NUnitRunner("runner-name");

            TestsPageForTest page = new TestsPageForTest(runner);

            page.InvokeOnItemSelected(null, args);

            CollectionAssert.IsEmpty(page.NavigationStack);
        }

        [Test]
        public void TestOnItemSelectedWithSelectedItemTestIdIsNullOrEmptyReturnsImmediately([Values] bool isNull)
        {
            TestForTest test = new TestForTest();
            test.Id = isNull ? null : string.Empty;
            SelectedItemChangedEventArgs args = new SelectedItemChangedEventArgs(test, 0);

            NUnitRunner runner = new NUnitRunner("runner-name");

            TestsPageForTest page = new TestsPageForTest(runner);

            page.InvokeOnItemSelected(null, args);

            CollectionAssert.IsEmpty(page.NavigationStack);
        }

        [Test]
        public void TestOnItemSelectedWithSelectedItemTestDoesNotHaveChildrenAndIsTestSuiteReturnsImmediately()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            ITest suite = new NUnitSuite("suite-name");
            TestsViewModel test = new TestsViewModel(runner, suite);
            Assert.IsTrue(test.Test.IsSuite);
            Assert.IsFalse(test.Test.HasChildren);

            SelectedItemChangedEventArgs args = new SelectedItemChangedEventArgs(test, 0);

            TestsPageForTest page = new TestsPageForTest(runner, test);

            page.InvokeOnItemSelected(null, args);

            CollectionAssert.IsEmpty(page.NavigationStack);
        }

        [Test]
        public void TestOnItemSelectedWithSelectedItemTestWithChildTestsPushesTestsPageToStackAndCaches()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            ITest firstTestInstance = runner.ExploreTests();
            ITest secondTestInstance = runner.ExploreTests();
            TestsViewModel firstTest = new TestsViewModel(runner, firstTestInstance);
            TestsViewModel secondTest = new TestsViewModel(runner, secondTestInstance);

            Assert.IsTrue(firstTest.Test.IsSuite);
            Assert.IsTrue(firstTest.Test.HasChildren);
            Assert.IsTrue(secondTest.Test.IsSuite);
            Assert.IsTrue(secondTest.Test.HasChildren);

            SelectedItemChangedEventArgs firstArgs = new SelectedItemChangedEventArgs(firstTest, 0);
            SelectedItemChangedEventArgs secondArgs = new SelectedItemChangedEventArgs(secondTest, 0);

            TestsPageForTest page = new TestsPageForTest(runner);

            // Load first page
            page.InvokeOnItemSelected(null, firstArgs);

            Assert.AreEqual(1, page.NavigationStack.Count);
            TestsPage firstTestsPage = page.NavigationStack.First() as TestsPage;
            Assert.IsNotNull(firstTestsPage);
            Assert.AreEqual(firstTest, firstTestsPage.ViewModel);
            Assert.AreEqual(runner, firstTestsPage.ViewModel.TestRunner);
            Assert.AreEqual(firstTestInstance, firstTestsPage.ViewModel.Test);

            // Load second page
            page.InvokeOnItemSelected(null, secondArgs);

            Assert.AreEqual(2, page.NavigationStack.Count);
            TestsPage secondTestsPage = page.NavigationStack[1] as TestsPage;
            Assert.IsNotNull(secondTestsPage);
            Assert.AreEqual(secondTest, secondTestsPage.ViewModel);
            Assert.AreEqual(runner, secondTestsPage.ViewModel.TestRunner);
            Assert.AreEqual(secondTestInstance, secondTestsPage.ViewModel.Test);

            // Load first page again
            IList<Page> expectedStack = new List<Page>(page.NavigationStack);
            expectedStack.Add(firstTestsPage);

            page.InvokeOnItemSelected(null, firstArgs);

            Assert.AreEqual(3, page.NavigationStack.Count);
            Assert.AreSame(firstTestsPage, page.NavigationStack[2]);
            CollectionAssert.AreEqual(expectedStack, page.NavigationStack);
        }

        [Test]
        public void TestOnItemSelectedWithSelectedItemTestWithoutChildTestsPushesTestDetailPageToStackAndCaches()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            IMethodInfo methodOne = new MethodWrapper(typeof(TestsPageTest), GetType().GetMethods().First());
            IMethodInfo methodTwo = new MethodWrapper(typeof(TestsPageTest), GetType().GetMethods().Last());
            ITest firstTestInstance = new TestMethod(methodOne);
            ITest secondTestInstance = new TestMethod(methodTwo);
            TestsViewModel firstTest = new TestsViewModel(runner, firstTestInstance);
            TestsViewModel secondTest = new TestsViewModel(runner, secondTestInstance);

            Assert.IsFalse(firstTest.Test.IsSuite);
            Assert.IsFalse(firstTest.Test.HasChildren);
            Assert.IsFalse(secondTest.Test.IsSuite);
            Assert.IsFalse(secondTest.Test.HasChildren);

            SelectedItemChangedEventArgs firstArgs = new SelectedItemChangedEventArgs(firstTest, 0);
            SelectedItemChangedEventArgs secondArgs = new SelectedItemChangedEventArgs(secondTest, 0);

            TestsPageForTest page = new TestsPageForTest(runner);

            // Load first page
            page.InvokeOnItemSelected(null, firstArgs);

            Assert.AreEqual(1, page.NavigationStack.Count);
            TestDetailPage firstTestsPage = page.NavigationStack.First() as TestDetailPage;
            Assert.IsNotNull(firstTestsPage);
            Assert.AreEqual(firstTest, firstTestsPage.ViewModel);
            Assert.AreEqual(runner, firstTestsPage.ViewModel.TestRunner);
            Assert.AreEqual(firstTestInstance, firstTestsPage.ViewModel.Test);

            // Load second page
            page.InvokeOnItemSelected(null, secondArgs);

            Assert.AreEqual(2, page.NavigationStack.Count);
            TestDetailPage secondTestsPage = page.NavigationStack[1] as TestDetailPage;
            Assert.IsNotNull(secondTestsPage);
            Assert.AreEqual(secondTest, secondTestsPage.ViewModel);
            Assert.AreEqual(runner, secondTestsPage.ViewModel.TestRunner);
            Assert.AreEqual(secondTestInstance, secondTestsPage.ViewModel.Test);

            // Load first page again
            IList<Page> expectedStack = new List<Page>(page.NavigationStack);
            expectedStack.Add(firstTestsPage);

            page.InvokeOnItemSelected(null, firstArgs);

            Assert.AreEqual(3, page.NavigationStack.Count);
            Assert.AreSame(firstTestsPage, page.NavigationStack[2]);
            CollectionAssert.AreEqual(expectedStack, page.NavigationStack);
        }

        #endregion

        #region Tests for RunTestsClicked

        [Test]
        public void TestRunTestsClickedRunsTests()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            TestsViewModel test = new TestsViewModel(runner, new NUnitSuite("suite-name"));

            TestsPageForTest page = new TestsPageForTest(runner, test);

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
            public TestsPageForTest(INUnitRunner runner, TestsViewModel test = null) : base(runner, test, false)
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

        #region Nested Class: TestForTest

        /// <summary>
        ///     Implementation of ITest for test.
        /// </summary>
        // ReSharper disable UnassignedGetOnlyAutoProperty
        private class TestForTest : ITest
        {
            #region Implementation if ITest

            /// <inheritdoc />
            public TNode ToXml(bool recursive)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public TNode AddToXml(TNode parentNode, bool recursive)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public string Id { get; set; }

            /// <inheritdoc />
            public string Name { get; }

            /// <inheritdoc />
            public string TestType { get; }

            /// <inheritdoc />
            public string FullName { get; }

            /// <inheritdoc />
            public string ClassName { get; }

            /// <inheritdoc />
            public string MethodName { get; }

            /// <inheritdoc />
            public ITypeInfo TypeInfo { get; }

            /// <inheritdoc />
            public IMethodInfo Method { get; }

            /// <inheritdoc />
            public RunState RunState { get; }

            /// <inheritdoc />
            public int TestCaseCount { get; }

            /// <inheritdoc />
            public IPropertyBag Properties { get; }

            /// <inheritdoc />
            public ITest Parent { get; }

            /// <inheritdoc />
            public bool IsSuite { get; }

            /// <inheritdoc />
            public bool HasChildren { get; }

            /// <inheritdoc />
            public IList<ITest> Tests { get; }

            /// <inheritdoc />
            public object Fixture { get; }

            /// <inheritdoc />
            public object[] Arguments { get; }

            #endregion
        }
        // ReSharper restore UnassignedGetOnlyAutoProperty

        #endregion
    }
}