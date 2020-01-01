using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using XamarinNUnitRunner.Filter;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Services;
using XamarinNUnitRunner.Test.Models;
using XamarinNUnitRunner.Test.Stub;

namespace XamarinNUnitRunner.Test.Services
{
    [TestFixture]
    public class NUnitRunnerTest
    {
        // Tests for TestSuite Property are covered by Constructor tests.
        // Tests for IsTestRunning and IsTestComplete Properties are covered by RunTestsAsync tests.

        #region Tests for Constructor

        [Test]
        public void TestConstructorThrowsArgumentExceptionWhenNameIsNullOrEmpty([Values] bool isNull)
        {
            string name = isNull ? null : string.Empty;

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The name cannot be null or empty. (Parameter 'name')"),
                // ReSharper disable once ObjectCreationAsStatement
                () => { new NUnitRunner(name); });
        }

        [Test]
        public void TestConstructorThrowsArgumentNullExceptionWhenSuiteIsNull()
        {
            NUnitSuite suite = null;

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The suite cannot be null. (Parameter 'suite')"),
                // ReSharper disable once ObjectCreationAsStatement
                // ReSharper disable once ExpressionIsAlwaysNull
                () => { new NUnitRunnerForTest(suite); });
        }

        [Test]
        public void TestConstructorWithName()
        {
            const string name = "suite-name";
            NUnitRunner runner = new NUnitRunner(name);

            Assert.IsNotNull(runner.TestSuite);
            Assert.AreEqual(name, runner.TestSuite.Name);
            Assert.AreEqual(name, runner.TestSuite.FullName);
        }

        [Test]
        public void TestConstructorWithSuite()
        {
            const string name = "suite-name";
            NUnitSuite suite = new NUnitSuite(name);
            NUnitRunnerForTest runner = new NUnitRunnerForTest(suite);

            Assert.IsNotNull(runner.TestSuite);
            Assert.AreSame(suite, runner.TestSuite);
            Assert.AreEqual(name, runner.TestSuite.Name);
            Assert.AreEqual(name, runner.TestSuite.FullName);
        }

        #endregion

        #region Tests for AddTestAssembly

        [Test]
        public void TestAddTestAssemblyThrowsArgumentNullExceptionWhenAssemblyIsNull()
        {
            NUnitSuite suite = new NUnitSuite("suite-name");
            Assembly assembly = null;
            NUnitRunnerForTest runner = new NUnitRunnerForTest(suite);

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The assembly cannot be null. (Parameter 'assembly')"),
                // ReSharper disable once ExpressionIsAlwaysNull
                () => runner.AddTestAssembly(assembly));
        }

        [Test]
        public void TestAddTestAssemblyWithAssemblyAlreadyAddedDoesNotReAddAssemblyAndReturnsAlreadyAddedAssembly()
        {
            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;
            List<Assembly> expectedAssemblies = new List<Assembly> {assembly};

            ITest testInitial = runner.AddTestAssembly(assembly);

            Assert.IsNotNull(testInitial);
            Assert.AreEqual(RunState.Runnable, testInitial.RunState);
            Assert.AreEqual("XamarinNUnitRunner.Test.Stub.dll", testInitial.Name);
            Assert.IsTrue(runner.TestSuite.ContainsAssembly(assembly));
            Assert.IsTrue(runner.TestSuite.GetTestAssemblyRunner(assembly).IsTestLoaded);
            CollectionAssert.AreEquivalent(expectedAssemblies, runner.TestSuite.TestAssemblies);

            ITest test = runner.AddTestAssembly(assembly);

            Assert.IsNotNull(test);
            Assert.AreSame(testInitial, test);
            Assert.AreEqual(RunState.Runnable, test.RunState);
            Assert.AreEqual("XamarinNUnitRunner.Test.Stub.dll", test.Name);
            Assert.IsTrue(runner.TestSuite.ContainsAssembly(assembly));
            Assert.IsTrue(runner.TestSuite.GetTestAssemblyRunner(assembly).IsTestLoaded);
            CollectionAssert.AreEquivalent(expectedAssemblies, runner.TestSuite.TestAssemblies);
        }

        [Test]
        public void TestAddTestAssemblyWithAssemblyLoadInvalidDoesNotAddAssemblyAndReturnsNull()
        {
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");
            suite.IsLoadedTestNull = true;
            NUnitRunnerForTest runner = new NUnitRunnerForTest(suite);

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            ITest test = runner.AddTestAssembly(assembly);

            Assert.IsNull(test);
            Assert.IsFalse(runner.TestSuite.ContainsAssembly(assembly));
            CollectionAssert.IsEmpty(runner.TestSuite.TestAssemblies);
        }

        [Test]
        public void TestAddTestAssemblyWithAssemblyLoadNotRunnableDoesNotAddAssemblyAndReturnsErroneousTest()
        {
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");
            suite.IsLoadedTestInvalid = true;
            NUnitRunnerForTest runner = new NUnitRunnerForTest(suite);

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            ITest test = runner.AddTestAssembly(assembly);

            Assert.IsNotNull(test);
            Assert.AreEqual(RunState.NotRunnable, test.RunState);
            Assert.IsFalse(suite.ContainsAssembly(assembly));
            CollectionAssert.IsEmpty(suite.TestAssemblies);
        }

        [Test]
        public void TestAddTestAssemblyWithAssemblyNotAlreadyAddedLoadsAndAddsAssemblyAndReturnsAddedTest(
            [Values] bool withSettings)
        {
            string workingDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));
            string expectedDirectory = withSettings ? workingDirectory : Directory.GetCurrentDirectory();
            Dictionary<string, object> settings = withSettings
                ? new Dictionary<string, object>
                    {{FrameworkPackageSettings.WorkDirectory, workingDirectory}}
                : null;

            INUnitRunner runner = new NUnitRunner("suite-name");

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;
            List<Assembly> expectedAssemblies = new List<Assembly> {assembly};

            ITest test = runner.AddTestAssembly(assembly, settings);

            Assert.IsNotNull(test);
            Assert.AreEqual(RunState.Runnable, test.RunState);
            Assert.AreEqual("XamarinNUnitRunner.Test.Stub.dll", test.Name);
            Assert.IsTrue(runner.TestSuite.ContainsAssembly(assembly));
            Assert.IsTrue(runner.TestSuite.GetTestAssemblyRunner(assembly).IsTestLoaded);
            Assert.AreEqual(expectedDirectory, TestContext.CurrentContext.WorkDirectory);
            CollectionAssert.AreEquivalent(expectedAssemblies, runner.TestSuite.TestAssemblies);
        }

        #endregion

        #region Tests for AddTestAssemblyAsync

        [Test]
        public void TestAddTestAssemblyAsyncThrowsArgumentNullExceptionWhenAssemblyIsNull()
        {
            NUnitSuite suite = new NUnitSuite("suite-name");
            Assembly assembly = null;
            NUnitRunnerForTest runner = new NUnitRunnerForTest(suite);

            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The assembly cannot be null. (Parameter 'assembly')"),
                // ReSharper disable once ExpressionIsAlwaysNull
                async () => await runner.AddTestAssemblyAsync(assembly));
        }

        [Test]
        public void TestAddTestAssemblyAsyncWithAssemblyAlreadyAddedDoesNotReAddAssemblyAndReturnsAlreadyAddedAssembly()
        {
            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;
            List<Assembly> expectedAssemblies = new List<Assembly> {assembly};

            Task<ITest> testInitialTask = runner.AddTestAssemblyAsync(assembly);
            Assert.IsNotNull(testInitialTask);
            testInitialTask.Wait();
            ITest testInitial = testInitialTask.Result;

            Assert.IsNotNull(testInitial);
            Assert.AreEqual(RunState.Runnable, testInitial.RunState);
            Assert.AreEqual("XamarinNUnitRunner.Test.Stub.dll", testInitial.Name);
            Assert.IsTrue(runner.TestSuite.ContainsAssembly(assembly));
            Assert.IsTrue(runner.TestSuite.GetTestAssemblyRunner(assembly).IsTestLoaded);
            CollectionAssert.AreEquivalent(expectedAssemblies, runner.TestSuite.TestAssemblies);

            Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
            Assert.IsNotNull(testTask);
            testTask.Wait();
            ITest test = testTask.Result;

            Assert.IsNotNull(test);
            Assert.AreSame(testInitial, test);
            Assert.AreEqual(RunState.Runnable, test.RunState);
            Assert.AreEqual("XamarinNUnitRunner.Test.Stub.dll", test.Name);
            Assert.IsTrue(runner.TestSuite.ContainsAssembly(assembly));
            Assert.IsTrue(runner.TestSuite.GetTestAssemblyRunner(assembly).IsTestLoaded);
            CollectionAssert.AreEquivalent(expectedAssemblies, runner.TestSuite.TestAssemblies);
        }

        [Test]
        public void TestAddTestAssemblyAsyncWithAssemblyLoadInvalidDoesNotAddAssemblyAndReturnsNull()
        {
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");
            suite.IsLoadedTestNull = true;
            NUnitRunnerForTest runner = new NUnitRunnerForTest(suite);

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
            Assert.IsNotNull(testTask);
            testTask.Wait();
            ITest test = testTask.Result;

            Assert.IsNull(test);
            Assert.IsFalse(runner.TestSuite.ContainsAssembly(assembly));
            CollectionAssert.IsEmpty(runner.TestSuite.TestAssemblies);
        }

        [Test]
        public void TestAddTestAssemblyAsyncWithAssemblyLoadNotRunnableDoesNotAddAssemblyAndReturnsErroneousTest()
        {
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");
            suite.IsLoadedTestInvalid = true;
            NUnitRunnerForTest runner = new NUnitRunnerForTest(suite);

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
            Assert.IsNotNull(testTask);
            testTask.Wait();
            ITest test = testTask.Result;

            Assert.IsNotNull(test);
            Assert.AreEqual(RunState.NotRunnable, test.RunState);
            Assert.IsFalse(suite.ContainsAssembly(assembly));
            CollectionAssert.IsEmpty(suite.TestAssemblies);
        }

        [Test]
        public void TestAddTestAssemblyAsyncWithAssemblyNotAlreadyAddedLoadsAndAddsAssemblyAndReturnsAddedTest(
            [Values] bool withSettings)
        {
            string workingDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));
            string expectedDirectory = withSettings ? workingDirectory : Directory.GetCurrentDirectory();
            Dictionary<string, object> settings = withSettings
                ? new Dictionary<string, object>
                    {{FrameworkPackageSettings.WorkDirectory, workingDirectory}}
                : null;

            INUnitRunner runner = new NUnitRunner("suite-name");

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;
            List<Assembly> expectedAssemblies = new List<Assembly> {assembly};

            Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly, settings);
            Assert.IsNotNull(testTask);
            testTask.Wait();
            ITest test = testTask.Result;

            Assert.IsNotNull(test);
            Assert.AreEqual(RunState.Runnable, test.RunState);
            Assert.AreEqual("XamarinNUnitRunner.Test.Stub.dll", test.Name);
            Assert.IsTrue(runner.TestSuite.ContainsAssembly(assembly));
            Assert.IsTrue(runner.TestSuite.GetTestAssemblyRunner(assembly).IsTestLoaded);
            Assert.AreEqual(expectedDirectory, TestContext.CurrentContext.WorkDirectory);
            CollectionAssert.AreEquivalent(expectedAssemblies, runner.TestSuite.TestAssemblies);
        }

        #endregion

        #region Tests for CountTestCases

        [Test]
        public void TestCountTestCasesReturnsNumberOfTestCasesLoaded([Values] bool withChildTests,
            [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expected = 0;
            if (withChildTests)
            {
                expected = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            int count = runner.CountTestCases(filter);

            Assert.AreEqual(expected, count);
        }

        [Test]
        public void TestCountTestCasesThrowsArgumentNullExceptionWhenAssemblyIsNull()
        {
            INUnitRunner runner = new NUnitRunner("suite-name");

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The assembly cannot be null. (Parameter 'assembly')"),
                () => runner.CountTestCases(null, null));
        }

        [Test]
        public void TestCountTestCasesWithAssemblyReturnsNumberOfTestCasesLoaded(
            [Values] bool withChildTests, [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expected = 0;
            if (withChildTests)
            {
                expected = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            int count = runner.CountTestCases(assembly, filter);

            Assert.AreEqual(expected, count);
        }

        #endregion

        #region Tests for CountTestCasesAsync

        [Test]
        public void TestCountTestCasesAsyncReturnsNumberOfTestCasesLoaded([Values] bool withChildTests,
            [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expected = 0;
            if (withChildTests)
            {
                expected = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            Task<int> countTask = runner.CountTestCasesAsync(filter);
            Assert.IsNotNull(countTask);
            countTask.Wait();
            int count = countTask.Result;

            Assert.AreEqual(expected, count);
        }

        [Test]
        public void TestCountTestCasesAsyncThrowsArgumentNullExceptionWhenAssemblyIsNull()
        {
            INUnitRunner runner = new NUnitRunner("suite-name");

            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The assembly cannot be null. (Parameter 'assembly')"),
                async () => await runner.CountTestCasesAsync(null, null));
        }

        [Test]
        public void TestCountTestCasesAsyncWithAssemblyReturnsNumberOfTestCasesLoaded(
            [Values] bool withChildTests, [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expected = 0;
            if (withChildTests)
            {
                expected = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            Task<int> countTask = runner.CountTestCasesAsync(assembly, filter);
            Assert.IsNotNull(countTask);
            countTask.Wait();
            int count = countTask.Result;

            Assert.AreEqual(expected, count);
        }

        #endregion

        #region Tests for ExploreTests

        [Test]
        public void TestExploreTestsReturnsLoadedTestCases([Values] bool withChildTests, [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expected = 0;
            if (withChildTests)
            {
                expected = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            ITest tests = runner.ExploreTests(filter);

            Assert.IsNotNull(tests);
            if (withChildTests)
            {
                Assert.IsTrue(tests.HasChildren);
                Assert.AreEqual(1, tests.Tests.Count);
                Assert.AreEqual(expected, tests.TestCaseCount);
            }
            else
            {
                Assert.IsFalse(tests.HasChildren);
            }
        }

        [Test]
        public void TestExploreTestsThrowsArgumentNullExceptionWhenAssemblyIsNull()
        {
            INUnitRunner runner = new NUnitRunner("suite-name");

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The assembly cannot be null. (Parameter 'assembly')"),
                () => runner.ExploreTests(null, null));
        }

        [Test]
        public void TestExploreTestsWithAssemblyReturnsLoadedTestCases([Values] bool withChildTests,
            [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expected = 0;
            if (withChildTests)
            {
                expected = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            ITest test = runner.ExploreTests(assembly, filter);

            if (withChildTests)
            {
                Assert.IsNotNull(test);
                Assert.AreEqual(expected, test.TestCaseCount);
            }
            else
            {
                Assert.IsNull(test);
            }
        }

        #endregion

        #region Tests for ExploreTestsAsync

        [Test]
        public void TestExploreTestsAsyncReturnsLoadedTestCases([Values] bool withChildTests, [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expected = 0;
            if (withChildTests)
            {
                expected = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            Task<ITest> exploreTask = runner.ExploreTestsAsync(filter);
            Assert.IsNotNull(exploreTask);
            exploreTask.Wait();
            ITest tests = exploreTask.Result;

            Assert.IsNotNull(tests);
            if (withChildTests)
            {
                Assert.IsTrue(tests.HasChildren);
                Assert.AreEqual(1, tests.Tests.Count);
                Assert.AreEqual(expected, tests.TestCaseCount);
            }
            else
            {
                Assert.IsFalse(tests.HasChildren);
            }
        }

        [Test]
        public void TestExploreTestsAsyncThrowsArgumentNullExceptionWhenAssemblyIsNull()
        {
            INUnitRunner runner = new NUnitRunner("suite-name");

            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The assembly cannot be null. (Parameter 'assembly')"),
                async () => await runner.ExploreTestsAsync(null, null));
        }

        [Test]
        public void TestExploreTestsAsyncWithAssemblyReturnsLoadedTestCases([Values] bool withChildTests,
            [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expected = 0;
            if (withChildTests)
            {
                expected = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            Task<ITest> exploreTask = runner.ExploreTestsAsync(assembly, filter);
            Assert.IsNotNull(exploreTask);
            exploreTask.Wait();
            ITest test = exploreTask.Result;

            if (withChildTests)
            {
                Assert.IsNotNull(test);
                Assert.AreEqual(expected, test.TestCaseCount);
            }
            else
            {
                Assert.IsNull(test);
            }
        }

        #endregion

        #region Tests for RunTests

        [Test]
        public void TestRunTestsRunsTestsAndReturnsTheRanTests([Values] bool withChildTests, [Values] bool withListener,
            [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;
            ResultState expectedState = ResultState.Inconclusive;
            if (withChildTests)
            {
                expectedState = withFilter ? ResultState.Success : ResultState.ChildFailure;
            }

            TestListenerForTest listener = withListener ? new TestListenerForTest() : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expectedCaseCount = 0;
            int expectedResultCount = 0;
            if (withChildTests)
            {
                expectedCaseCount = TestFixtureStubOne.ResultsDepth +
                                    (withFilter ? 1 : TestFixtureStubOne.ResultCount);
                expectedResultCount = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            ITestResult results = runner.RunTests(listener, filter);

            Assert.IsNotNull(results);
            Assert.IsFalse(runner.IsTestRunning);
            Assert.IsTrue(runner.IsTestComplete);
            Assert.AreEqual(expectedState, results.ResultState);
            int totalResultCount = results.FailCount + results.InconclusiveCount + results.PassCount +
                                   results.SkipCount +
                                   results.WarningCount;
            Assert.AreEqual(expectedResultCount, totalResultCount);

            if (withListener)
            {
                Assert.AreEqual(expectedCaseCount, listener.Tests.Count);
            }
        }

        [Test]
        public void TestRunTestsThrowsArgumentNullExceptionWhenAssemblyIsNull()
        {
            INUnitRunner runner = new NUnitRunner("suite-name");

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The assembly cannot be null. (Parameter 'assembly')"),
                () => runner.RunTests(null, null, null));
        }

        [Test]
        public void TestRunTestsWithAssemblyRunsTestsAndReturnsTheRanTests([Values] bool withChildTests,
            [Values] bool withListener, [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;
            ResultState expectedState = ResultState.Inconclusive;
            if (withChildTests)
            {
                expectedState = withFilter ? ResultState.Success : ResultState.ChildFailure;
            }

            TestListenerForTest listener = withListener ? new TestListenerForTest() : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expectedCaseCount = 0;
            int expectedResultCount = 0;
            if (withChildTests)
            {
                expectedCaseCount = TestFixtureStubOne.ResultsDepth +
                                    (withFilter ? 1 : TestFixtureStubOne.ResultCount);
                expectedResultCount = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            ITestResult results = runner.RunTests(assembly, listener, filter);

            if (withChildTests)
            {
                Assert.IsNotNull(results);
                Assert.IsFalse(runner.IsTestRunning);
                Assert.IsTrue(runner.IsTestComplete);
                Assert.AreEqual(expectedState, results.ResultState);
                int totalResultCount = results.FailCount + results.InconclusiveCount + results.PassCount +
                                       results.SkipCount +
                                       results.WarningCount;
                Assert.AreEqual(expectedResultCount, totalResultCount);

                if (withListener)
                {
                    Assert.AreEqual(expectedCaseCount, listener.Tests.Count);
                }
            }
            else
            {
                Assert.IsNull(results);
            }
        }

        #endregion

        #region Tests for RunTestsAsync

        [Test]
        public void TestRunTestsAsyncRunsTestsAndReturnsTheRanTests([Values] bool withChildTests,
            [Values] bool withListener, [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;
            ResultState expectedState = ResultState.Inconclusive;
            if (withChildTests)
            {
                expectedState = withFilter ? ResultState.Success : ResultState.ChildFailure;
            }

            TestListenerForTest listener = withListener ? new TestListenerForTest() : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            Assert.IsFalse(runner.IsTestRunning);

            int expectedCaseCount = 0;
            int expectedResultCount = 0;
            if (withChildTests)
            {
                expectedCaseCount = TestFixtureStubOne.ResultsDepth +
                                    (withFilter ? 1 : TestFixtureStubOne.ResultCount);
                expectedResultCount = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            Task<ITestResult> testsTask = runner.RunTestsAsync(listener, filter);
            Assert.IsNotNull(testsTask);
            testsTask.Wait();
            ITestResult results = testsTask.Result;

            Assert.IsFalse(runner.IsTestRunning);

            Assert.IsNotNull(results);
            Assert.IsFalse(runner.IsTestRunning);
            Assert.IsTrue(runner.IsTestComplete);
            Assert.AreEqual(expectedState, results.ResultState);
            int totalResultCount = results.FailCount + results.InconclusiveCount + results.PassCount +
                                   results.SkipCount +
                                   results.WarningCount;
            Assert.AreEqual(expectedResultCount, totalResultCount);

            if (withListener)
            {
                Assert.AreEqual(expectedCaseCount, listener.Tests.Count);
            }
        }

        [Test]
        public void TestRunTestsAsyncThrowsArgumentNullExceptionWhenAssemblyIsNull()
        {
            INUnitRunner runner = new NUnitRunner("suite-name");

            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The assembly cannot be null. (Parameter 'assembly')"),
                async () => await runner.RunTestsAsync(null, null, null));
        }

        [Test]
        public void
            TestRunTestsAsyncWithAlreadyRunningTestWaitsForPreviousCompletionAndThenRunsTestsAndReturnsTheRanTests()
        {
            TestListenerForTest listener = new TestListenerForTest();

            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");
            TestAssemblyRunnerForTest assemblyRunner = new TestAssemblyRunnerForTest();
            assemblyRunner.IsTestLoaded = true;
            assemblyRunner.IsTestRunning = true;
            suite.RunnerToLoad = assemblyRunner;

            NUnitRunnerForTest runner = new NUnitRunnerForTest(suite);
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            const int expectedCaseCount = TestFixtureStubOne.ResultsDepth + TestFixtureStubOne.ResultCount;
            const int expectedResultCount = TestFixtureStubOne.TestCount;
            Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
            testTask.Wait();

            Assert.IsTrue(runner.IsTestRunning);
            Assert.IsFalse(runner.IsTestComplete);

            Task<ITestResult> testsTask = runner.RunTestsAsync(listener);
            Assert.IsNotNull(testsTask);
            Assert.IsTrue(runner.IsTestRunning);
            Assert.IsFalse(runner.IsTestComplete);

            Thread.Sleep(50);
            assemblyRunner.IsTestRunning = false;

            testsTask.Wait();
            ITestResult results = testsTask.Result;

            Assert.IsFalse(runner.IsTestRunning);
            Assert.IsTrue(runner.IsTestComplete);

            Assert.IsNotNull(results);
            Assert.IsFalse(runner.IsTestRunning);
            Assert.IsTrue(runner.IsTestComplete);
            Assert.AreEqual(ResultState.ChildFailure, results.ResultState);
            int totalResultCount = results.FailCount + results.InconclusiveCount + results.PassCount +
                                   results.SkipCount +
                                   results.WarningCount;
            Assert.AreEqual(expectedResultCount, totalResultCount);
            Assert.AreEqual(expectedCaseCount, listener.Tests.Count);
        }

        [Test]
        public void TestRunTestsAsyncWithAssemblyRunsTestsAndReturnsTheRanTests([Values] bool withChildTests,
            [Values] bool withListener, [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;
            ResultState expectedState = ResultState.Inconclusive;
            if (withChildTests)
            {
                expectedState = withFilter ? ResultState.Success : ResultState.ChildFailure;
            }

            TestListenerForTest listener = withListener ? new TestListenerForTest() : null;

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expectedCaseCount = 0;
            int expectedResultCount = 0;
            if (withChildTests)
            {
                expectedCaseCount = TestFixtureStubOne.ResultsDepth +
                                    (withFilter ? 1 : TestFixtureStubOne.ResultCount);
                expectedResultCount = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            Task<ITestResult> testsTask = runner.RunTestsAsync(assembly, listener, filter);
            Assert.IsNotNull(testsTask);
            testsTask.Wait();
            ITestResult results = testsTask.Result;

            if (withChildTests)
            {
                Assert.IsNotNull(results);
                Assert.IsFalse(runner.IsTestRunning);
                Assert.IsTrue(runner.IsTestComplete);
                Assert.AreEqual(expectedState, results.ResultState);
                int totalResultCount = results.FailCount + results.InconclusiveCount + results.PassCount +
                                       results.SkipCount +
                                       results.WarningCount;
                Assert.AreEqual(expectedResultCount, totalResultCount);

                if (withListener)
                {
                    Assert.AreEqual(expectedCaseCount, listener.Tests.Count);
                }
            }
            else
            {
                Assert.IsNull(results);
            }
        }

        #endregion

        #region Tests for GetTestResults

        [Test]
        public void TestGetTestResultsReturnsResults([Values] bool withChildTests, [Values] bool runTests,
            [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;
            ResultState expectedState = ResultState.Inconclusive;
            if (withChildTests && runTests)
            {
                expectedState = withFilter ? ResultState.Success : ResultState.ChildFailure;
            }

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expectedCount = 0;
            if (withChildTests)
            {
                expectedCount = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            if (runTests)
            {
                runner.RunTests(null, filter);
            }
            else
            {
                expectedCount = 0;
            }

            ITestResult results = runner.GetTestResults();

            Assert.IsNotNull(results);
            Assert.AreEqual(expectedState, results.ResultState);
            int totalCount = results.FailCount + results.InconclusiveCount + results.PassCount + results.SkipCount +
                             results.WarningCount;
            Assert.AreEqual(expectedCount, totalCount);
        }

        [Test]
        public void TestGetTestResultsThrowsArgumentNullExceptionWhenAssemblyIsNull()
        {
            INUnitRunner runner = new NUnitRunner("suite-name");

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The assembly cannot be null. (Parameter 'assembly')"),
                () => runner.GetTestResults(null));
        }

        [Test]
        public void TestGetTestResultsWithAssemblyReturnsResults([Values] bool withChildTests,
            [Values] bool runTests, [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;
            ResultState expectedState = ResultState.Inconclusive;
            if (withChildTests && runTests)
            {
                expectedState = withFilter ? ResultState.Success : ResultState.ChildFailure;
            }

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expectedCount = 0;
            if (withChildTests)
            {
                expectedCount = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            if (runTests)
            {
                runner.RunTests(null, filter);
            }
            else
            {
                expectedCount = 0;
            }

            ITestResult results = runner.GetTestResults(assembly);

            Assert.IsNotNull(results);
            Assert.AreEqual(expectedState, results.ResultState);
            int totalCount = results.FailCount + results.InconclusiveCount + results.PassCount + results.SkipCount +
                             results.WarningCount;
            Assert.AreEqual(expectedCount, totalCount);
        }

        #endregion

        #region Tests for GetTestResultsAsync

        [Test]
        public void TestGetTestResultsAsyncReturnsResults([Values] bool withChildTests, [Values] bool runTests,
            [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;
            ResultState expectedState = ResultState.Inconclusive;
            if (withChildTests && runTests)
            {
                expectedState = withFilter ? ResultState.Success : ResultState.ChildFailure;
            }

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expectedCount = 0;
            if (withChildTests)
            {
                expectedCount = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            if (runTests)
            {
                runner.RunTests(null, filter);
            }
            else
            {
                expectedCount = 0;
            }

            Task<ITestResult> resultTask = runner.GetTestResultsAsync();
            Assert.IsNotNull(resultTask);
            resultTask.Wait();
            ITestResult results = resultTask.Result;

            Assert.IsNotNull(results);
            Assert.AreEqual(expectedState, results.ResultState);
            int totalCount = results.FailCount + results.InconclusiveCount + results.PassCount + results.SkipCount +
                             results.WarningCount;
            Assert.AreEqual(expectedCount, totalCount);
        }

        [Test]
        public void TestGetTestResultsAsyncThrowsArgumentNullExceptionWhenAssemblyIsNull()
        {
            INUnitRunner runner = new NUnitRunner("suite-name");

            Assert.ThrowsAsync(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The assembly cannot be null. (Parameter 'assembly')"),
                async () => await runner.GetTestResultsAsync(null));
        }

        [Test]
        public void TestGetTestResultsAsyncWithAssemblyReturnsResults([Values] bool withChildTests,
            [Values] bool runTests, [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : null;
            ResultState expectedState = ResultState.Inconclusive;
            if (withChildTests && runTests)
            {
                expectedState = withFilter ? ResultState.Success : ResultState.ChildFailure;
            }

            INUnitRunner runner = new NUnitRunner("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            int expectedCount = 0;
            if (withChildTests)
            {
                expectedCount = withFilter ? 1 : TestFixtureStubOne.TestCount;
                Task<ITest> testTask = runner.AddTestAssemblyAsync(assembly);
                testTask.Wait();
            }

            if (runTests)
            {
                runner.RunTests(null, filter);
            }
            else
            {
                expectedCount = 0;
            }

            Task<ITestResult> resultTask = runner.GetTestResultsAsync(assembly);
            Assert.IsNotNull(resultTask);
            resultTask.Wait();
            ITestResult results = resultTask.Result;

            Assert.IsNotNull(results);
            Assert.AreEqual(expectedState, results.ResultState);
            int totalCount = results.FailCount + results.InconclusiveCount + results.PassCount + results.SkipCount +
                             results.WarningCount;
            Assert.AreEqual(expectedCount, totalCount);
        }

        #endregion
    }
}