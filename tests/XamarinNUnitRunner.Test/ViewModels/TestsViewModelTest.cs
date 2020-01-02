using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using XamarinNUnitRunner.Filter;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Services;
using XamarinNUnitRunner.Test.Models;
using XamarinNUnitRunner.Test.Services;
using XamarinNUnitRunner.Test.Stub;
using XamarinNUnitRunner.ViewModels;

namespace XamarinNUnitRunner.Test.ViewModels
{
    [TestFixture]
    public class TestsViewModelTest
    {
        #region Private Members

        /// <summary>
        ///     Holds the NUnit runner for tests.
        /// </summary>
        private readonly NUnitRunner v_Runner = new NUnitRunner("runner-name");

        #endregion

        // Tests for BaseViewModel covered by BaseViewModelTest test fixture

        #region Tests for Constructors

        [Test]
        public void TestConstructorSetsTitleAndCommands([Values] bool withTestRunner)
        {
            const string runnerName = "runner-name";
            string expectedTitle = withTestRunner ? runnerName : string.Empty;
            NUnitRunner runner = withTestRunner ? new NUnitRunner(runnerName) : null;

            TestsViewModel model = new TestsViewModel(runner);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(expectedTitle, model.Title);
            Assert.IsNull(model.Test);
            Assert.IsNull(model.Result);
            Assert.AreEqual(runner, model.TestRunner);
            Assert.IsNotNull(model.Tests);
            CollectionAssert.IsEmpty(model.Tests);
            Assert.IsNotNull(model.LoadTestsCommand);
            Assert.IsTrue(model.LoadTestsCommand.CanExecute(null));
            Assert.IsNotNull(model.RunTestsCommand);
            Assert.IsTrue(model.RunTestsCommand.CanExecute(null));
            Assert.IsNotNull(model.ReloadResultsCommand);
            Assert.IsTrue(model.ReloadResultsCommand.CanExecute(null));
        }

        [Test]
        public void TestConstructorWithTestSetsTestAndResultAndTitleAndCommands([Values] bool withTestRunner,
            [Values] bool withTest, [Values] bool withResult)
        {
            const string runnerName = "runner-name";
            const string suiteName = "suite-name";
            string fullSuiteName = Path.Combine(TestContext.CurrentContext.WorkDirectory, suiteName + ".dll");
            TestSuite suite = new TestSuite(fullSuiteName);

            string expectedTitle = withTest ? suiteName : withTestRunner ? runnerName : string.Empty;

            NUnitRunner runner = withTestRunner ? new NUnitRunner(runnerName) : null;
            ITest test = withTest ? suite : null;
            ITestResult result = null;
            if (withTest)
            {
                result = withResult ? new TestSuiteResult(suite) : null;
            }

            TestsViewModel model = new TestsViewModel(runner, test, result);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(expectedTitle, model.Title);
            Assert.AreEqual(test, model.Test);
            Assert.IsNotNull(model.Result);
            Assert.AreEqual(result, model.Result.Result);
            Assert.AreEqual(runner, model.TestRunner);
            Assert.IsNotNull(model.Tests);
            CollectionAssert.IsEmpty(model.Tests);
            Assert.IsNotNull(model.LoadTestsCommand);
            Assert.IsTrue(model.LoadTestsCommand.CanExecute(null));
            Assert.IsNotNull(model.RunTestsCommand);
            Assert.IsTrue(model.RunTestsCommand.CanExecute(null));
            Assert.IsNotNull(model.ReloadResultsCommand);
            Assert.IsTrue(model.ReloadResultsCommand.CanExecute(null));
        }

        #endregion

        // Tests for Tests Property are covered by tests for Constructors

        #region Tests for LoadTestsCommand Property

        [Test]
        public void TestLoadTestsCommandReturnsImmediatelyWhenIsBusyIsTrue()
        {
            int invocationCount = 0;
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);

            TestsViewModel model = new TestsViewModel(runner, runner.TestSuite);
            model.IsBusy = true;

            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("IsBusy"))
                {
                    invocationCount++;
                }
            };

            model.LoadTestsCommand.Execute(null);

            Assert.IsTrue(model.IsBusy);
            Assert.AreEqual(0, invocationCount);
            CollectionAssert.IsEmpty(model.Tests);
        }

        [Test]
        public void TestLoadTestsCommandReturnsImmediatelyWhenTestRunnerIsNull()
        {
            int invocationCount = 0;
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);

            TestsViewModel model = new TestsViewModel(null, runner.TestSuite);

            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("IsBusy"))
                {
                    invocationCount++;
                }
            };

            model.LoadTestsCommand.Execute(null);

            WaitForCommand(model);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(0, invocationCount);
            CollectionAssert.IsEmpty(model.Tests);
        }

        [Test]
        public void TestLoadTestsCommandReturnsImmediatelyWhenTestRunnerIsRunningTests()
        {
            int invocationCount = 0;
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");
            TestAssemblyRunnerForTest assemblyRunner = new TestAssemblyRunnerForTest();
            assemblyRunner.IsTestRunning = true;

            suite.RunnerToLoad = assemblyRunner;
            NUnitRunnerForTest runner = new NUnitRunnerForTest(suite);
            runner.AddTestAssembly(GetType().Assembly);
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);

            Assert.AreEqual(2, runner.TestSuite.TestAssemblyRunners.Count);

            TestsViewModel model = new TestsViewModel(runner, runner.TestSuite);

            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("IsBusy"))
                {
                    invocationCount++;
                }
            };

            model.LoadTestsCommand.Execute(null);

            WaitForCommand(model);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(2, invocationCount);
            CollectionAssert.IsEmpty(model.Tests);
        }

        [Test]
        public void TestLoadTestsCommandClearsAndReturnsWhenTestHasNoChildren()
        {
            int invocationCount = 0;
            NUnitRunner runner = new NUnitRunner("runner-name");
            ITest test = runner.TestSuite;

            TestsViewModel model = new TestsViewModel(runner, test);

            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("IsBusy"))
                {
                    invocationCount++;
                }
            };

            model.LoadTestsCommand.Execute(null);

            WaitForCommand(model);

            Assert.IsFalse(model.IsBusy);
            Assert.IsNotNull(model.Test);

            Assert.IsNotNull(model.Test);
            Assert.IsFalse(model.Test.HasChildren);

            Assert.AreEqual(2, invocationCount);
            CollectionAssert.IsEmpty(model.Tests);
        }

        [Test]
        public void TestLoadTestsCommandLoadsTestsFromTestRunner([Values] bool withTest, [Values] bool withResults,
            [Values] bool withAdditionalTests)
        {
            int invocationCount = 0;
            int testInvocationCount = 0;
            int expectedTestCount = withTest ? 0 : 1;
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            int expectedRunnerCount = withAdditionalTests ? 2 : 1;
            if (withAdditionalTests)
            {
                runner.AddTestAssembly(GetType().Assembly);
            }

            Assert.AreEqual(expectedRunnerCount, runner.TestSuite.TestAssemblyRunners.Count);

            ITest testInstance = runner.TestSuite;
            ITest test = withTest ? testInstance : null;

            TestsViewModel model = new TestsViewModel(runner, test);

            ITestFilter filter = NUnitFilter.Where.Class(typeof(TestFixtureStubOne).Namespace).Build().Filter;
            ITestResult expectedResult = withResults ? runner.RunTests(null, filter) : null;
            List<ITestResult> expectedResults = new List<ITestResult>();
            model.Result = withResults ? new NUnitTestResult(expectedResult) : null;
            if (withResults)
            {
                expectedResults.AddRange(expectedResult.Children);
            }
            else
            {
                expectedResults.Add(null);
                if (withAdditionalTests)
                {
                    expectedResults.Add(null);
                }
            }

            model.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case "IsBusy":
                        invocationCount++;
                        break;
                    case "Test":
                        testInvocationCount++;
                        break;
                }
            };

            model.LoadTestsCommand.Execute(null);

            WaitForCommand(model);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(2, invocationCount);
            Assert.AreEqual(expectedTestCount, testInvocationCount);
            Assert.IsNotNull(model.Test);
            Assert.AreEqual(testInstance, model.Test);

            IList<ITest> actualTests = model.Tests.Select(x => x.Test).ToList();

            // Matching test is not the root, so search for the test instance with the namespace of the test fixture
            IList<ITest> foundTests = new List<ITest>();
            int skipCount = 0;
            ITest foundTest = GetTestByName(testInstance, typeof(TestFixtureStubOne).Namespace, ref skipCount);
            foundTests.Add(foundTest);

            // Matching result is not the root, so search for the result instance with the namespace of the test fixture
            skipCount = 0;
            IList<ITestResult> foundResults = expectedResults
                .Select(result => GetTestResultByName(result, typeof(TestFixtureStubOne).Namespace, ref skipCount))
                .ToList();

            if (withAdditionalTests)
            {
                // Matching test is not the root, so search for the test instance with the namespace of the test fixture
                // Skip the first found match as the namespace is a sub-space of another
                // The first found is x.y of sub-space x.y.z, but the actual is just x.y with no sub-space
                skipCount = 1;
                foundTest = GetTestByName(testInstance, GetType().Assembly.GetName().Name, ref skipCount);
                foundTests.Add(foundTest);
            }

            CollectionAssert.AreEquivalent(foundTests, actualTests);

            IList<ITestResult> actualResults = model.Tests.Select(x => x.Result.Result).ToList();
            CollectionAssert.AreEquivalent(foundResults, actualResults);
            Assert.AreEqual(expectedResult, model.Result?.Result);
        }

        #endregion

        #region Tests for RunTestsCommand Property

        [Test]
        public void TestRunTestsCommandReturnsImmediatelyWhenIsBusyIsTrue()
        {
            int invocationCount = 0;
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);

            TestsViewModel model = new TestsViewModel(runner, runner.TestSuite);
            model.IsBusy = true;

            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("IsBusy"))
                {
                    invocationCount++;
                }
            };

            model.RunTestsCommand.Execute(null);

            Assert.IsTrue(model.IsBusy);
            Assert.AreEqual(0, invocationCount);
            CollectionAssert.IsEmpty(model.Tests);
        }

        [Test]
        public void TestRunTestsCommandReturnsImmediatelyWhenTestRunnerIsNull()
        {
            int invocationCount = 0;
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);

            TestsViewModel model = new TestsViewModel(null, runner.TestSuite);

            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("IsBusy"))
                {
                    invocationCount++;
                }
            };

            model.RunTestsCommand.Execute(null);

            WaitForCommand(model);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(0, invocationCount);
            CollectionAssert.IsEmpty(model.Tests);
        }

        [Test]
        public void TestRunTestsCommandReturnsImmediatelyWhenTestRunnerIsRunningTests()
        {
            int invocationCount = 0;
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");
            TestAssemblyRunnerForTest assemblyRunner = new TestAssemblyRunnerForTest();
            assemblyRunner.IsTestRunning = true;

            suite.RunnerToLoad = assemblyRunner;
            NUnitRunnerForTest runner = new NUnitRunnerForTest(suite);
            runner.AddTestAssembly(GetType().Assembly);
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);

            Assert.AreEqual(2, runner.TestSuite.TestAssemblyRunners.Count);

            TestsViewModel model = new TestsViewModel(runner, runner.TestSuite);

            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("IsBusy"))
                {
                    invocationCount++;
                }
            };

            model.RunTestsCommand.Execute(null);

            WaitForCommand(model);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(2, invocationCount);
            CollectionAssert.IsEmpty(model.Tests);
        }

        [Test]
        public void TestRunTestsCommandRunsTestsFromTestRunner([Values] bool withTest,
            [Values] bool withAdditionalTests, [Values] bool withPreload)
        {
            int invocationCount = 0;
            int testInvocationCount = 0;
            int expectedTestCount = withPreload || withTest ? 0 : 1;
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            int expectedRunnerCount = withAdditionalTests ? 2 : 1;
            if (withAdditionalTests)
            {
                runner.AddTestAssembly(GetType().Assembly);
            }

            Assert.AreEqual(expectedRunnerCount, runner.TestSuite.TestAssemblyRunners.Count);

            ITest testInstance = runner.TestSuite;
            ITest test = withTest ? testInstance : null;

            TestsViewModel model = new TestsViewModel(runner, test);

            ITestFilter filterInstance =
                NUnitFilter.Where.Namespace(typeof(TestFixtureStubOne).Namespace).Build().Filter;
            ITestFilter filter = withAdditionalTests ? filterInstance : null;
            ITestResult expectedResult = runner.RunTests(null, filter);
            List<ITestResult> expectedResults = new List<ITestResult>();
            expectedResults.AddRange(expectedResult.Children);

            if (withPreload)
            {
                model.LoadTestsCommand.Execute(null);

                WaitForCommand(model);
            }

            model.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case "IsBusy":
                        invocationCount++;
                        break;
                    case "Test":
                        testInvocationCount++;
                        break;
                }
            };

            model.RunTestsCommand.Execute(filter);

            WaitForCommand(model);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(2, invocationCount);
            Assert.AreEqual(expectedTestCount, testInvocationCount);
            Assert.IsNotNull(model.Test);
            Assert.AreEqual(testInstance, model.Test);

            IList<ITestResult> actualResults = model.Tests.Select(x => x.Result.Result).ToList();
            if (withPreload)
            {
                IList<ITest> actualTests = model.Tests.Select(x => x.Test).ToList();

                // Matching test is not the root, so search for the test instance with the namespace of the test fixture
                IList<ITest> foundTests = new List<ITest>();
                int skipCount = 0;
                ITest foundTest = GetTestByName(testInstance, typeof(TestFixtureStubOne).Namespace, ref skipCount);
                foundTests.Add(foundTest);

                // Matching result is not the root, so search for the result instance with the namespace of the test fixture
                skipCount = 0;
                IList<ITestResult> foundResults = expectedResults
                    .Select(result => GetTestResultByName(result, typeof(TestFixtureStubOne).Namespace, ref skipCount))
                    .ToList();

                if (withAdditionalTests)
                {
                    // Matching test is not the root, so search for the test instance with the namespace of the test fixture
                    // Skip the first found match as the namespace is a sub-space of another
                    // The first found is x.y of sub-space x.y.z, but the actual is just x.y with no sub-space
                    skipCount = 1;
                    foundTest = GetTestByName(testInstance, GetType().Assembly.GetName().Name, ref skipCount);
                    foundTests.Add(foundTest);
                }

                CollectionAssert.AreEquivalent(foundTests, actualTests);
                CollectionAssert.AreEquivalent(foundResults.Select(x => x?.Test), actualResults.Select(x => x?.Test));
                CollectionAssert.AreEquivalent(foundResults.Select(x => x?.ResultState),
                    actualResults.Select(x => x?.ResultState));
            }
            else
            {
                CollectionAssert.IsEmpty(actualResults);
            }

            Assert.AreEqual(expectedResult.Test, model.Result.Test);
            Assert.AreEqual(expectedResult.ResultState, model.Result.ResultState);
        }

        #endregion

        #region Tests for ReloadResultsCommand Property

        [Test]
        public void TestReloadResultsCommandReturnsImmediatelyWhenIsBusyIsTrue()
        {
            int invocationCount = 0;
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);

            TestsViewModel model = new TestsViewModel(runner, runner.TestSuite);
            model.IsBusy = true;

            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("IsBusy"))
                {
                    invocationCount++;
                }
            };

            model.ReloadResultsCommand.Execute(null);

            Assert.IsTrue(model.IsBusy);
            Assert.AreEqual(0, invocationCount);
            CollectionAssert.IsEmpty(model.Tests);
        }

        [Test]
        public void TestReloadResultsCommandReturnsImmediatelyWhenTestRunnerIsNull()
        {
            int invocationCount = 0;
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);

            TestsViewModel model = new TestsViewModel(null, runner.TestSuite);

            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("IsBusy"))
                {
                    invocationCount++;
                }
            };

            model.ReloadResultsCommand.Execute(null);

            WaitForCommand(model);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(0, invocationCount);
            CollectionAssert.IsEmpty(model.Tests);
        }

        [Test]
        public void TestReloadResultsCommandReturnsImmediatelyWhenTestRunnerIsRunningTests()
        {
            int invocationCount = 0;
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");
            TestAssemblyRunnerForTest assemblyRunner = new TestAssemblyRunnerForTest();
            assemblyRunner.IsTestRunning = true;

            suite.RunnerToLoad = assemblyRunner;
            NUnitRunnerForTest runner = new NUnitRunnerForTest(suite);
            runner.AddTestAssembly(GetType().Assembly);
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);

            Assert.AreEqual(2, runner.TestSuite.TestAssemblyRunners.Count);

            TestsViewModel model = new TestsViewModel(runner, runner.TestSuite);

            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("IsBusy"))
                {
                    invocationCount++;
                }
            };

            model.ReloadResultsCommand.Execute(null);

            WaitForCommand(model);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(2, invocationCount);
            CollectionAssert.IsEmpty(model.Tests);
        }

        [Test]
        public void TestReloadResultsCommandReloadsTestResults([Values] bool withTest, [Values] bool withResults,
            [Values] bool withAdditionalTests)
        {
            int invocationCount = 0;
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            int expectedRunnerCount = withAdditionalTests ? 2 : 1;
            if (withAdditionalTests)
            {
                runner.AddTestAssembly(GetType().Assembly);
            }

            Assert.AreEqual(expectedRunnerCount, runner.TestSuite.TestAssemblyRunners.Count);

            ITest testInstance = runner.TestSuite;
            ITest test = withTest ? testInstance : null;

            TestsViewModel model = new TestsViewModel(runner, test);

            ITestFilter filter = NUnitFilter.Where.Class(typeof(TestFixtureStubOne).Namespace).Build().Filter;
            ITestResult expectedResult = withResults ? runner.RunTests(null, filter) : null;
            List<ITestResult> expectedResults = new List<ITestResult>();
            model.Result = withResults ? new NUnitTestResult(expectedResult) : null;
            if (withResults)
            {
                expectedResults.AddRange(expectedResult.Children);
            }
            else
            {
                expectedResults.Add(null);
                if (withAdditionalTests)
                {
                    expectedResults.Add(null);
                }
            }

            model.LoadTestsCommand.Execute(null);

            WaitForCommand(model);

            model.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case "IsBusy":
                        invocationCount++;
                        break;
                }
            };

            model.ReloadResultsCommand.Execute(null);

            WaitForCommand(model);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(2, invocationCount);
            Assert.IsNotNull(model.Test);
            Assert.AreEqual(testInstance, model.Test);

            IList<ITest> actualTests = model.Tests.Select(x => x.Test).ToList();

            // Matching test is not the root, so search for the test instance with the namespace of the test fixture
            IList<ITest> foundTests = new List<ITest>();
            int skipCount = 0;
            ITest foundTest = GetTestByName(testInstance, typeof(TestFixtureStubOne).Namespace, ref skipCount);
            foundTests.Add(foundTest);

            // Matching result is not the root, so search for the result instance with the namespace of the test fixture
            skipCount = 0;
            IList<ITestResult> foundResults = expectedResults
                .Select(result => GetTestResultByName(result, typeof(TestFixtureStubOne).Namespace, ref skipCount))
                .ToList();

            if (withAdditionalTests)
            {
                // Matching test is not the root, so search for the test instance with the namespace of the test fixture
                // Skip the first found match as the namespace is a sub-space of another
                // The first found is x.y of sub-space x.y.z, but the actual is just x.y with no sub-space
                skipCount = 1;
                foundTest = GetTestByName(testInstance, GetType().Assembly.GetName().Name, ref skipCount);
                foundTests.Add(foundTest);
            }

            CollectionAssert.AreEquivalent(foundTests, actualTests);

            IList<ITestResult> actualResults = model.Tests.Select(x => x.Result.Result).ToList();
            CollectionAssert.AreEquivalent(foundResults, actualResults);
            Assert.AreEqual(expectedResult, model.Result?.Result);
        }

        #endregion

        // Tests for TestRunner Property are covered by tests for Constructors

        #region Tests for Test Property

        [Test]
        public void TestTestPropertyCanBeSetAndInvokesPropertyChangedEventIfSetValueIsNotSameAsCurrentValue(
            [Values] bool withTest, [Values] bool isChangedEventNull)
        {
            object sender = null;
            PropertyChangedEventArgs args = null;
            int invocationCount = 0;
            int titleInvocationCount = 0;
            int expectedCount = isChangedEventNull ? 0 : 1;

            NUnitRunner runner = new NUnitRunner("runner-name");
            TestSuite suite = new TestSuite("suite-name");
            ITest testInstance = suite;
            ITest test = withTest ? testInstance : null;
            ITest expectedTest = withTest ? null : testInstance;
            string initialTitle = withTest ? testInstance.FullName : runner.TestSuite.FullName;
            string expectedTitle = withTest ? string.Empty : testInstance.FullName;

            TestsViewModel model = new TestsViewModel(runner, test);

            IList<string> changedProps = new List<string>();
            if (!isChangedEventNull)
            {
                model.PropertyChanged += (s, a) =>
                {
                    changedProps.Add(a.PropertyName);
                    switch (a.PropertyName)
                    {
                        case "Test":
                            sender = s;
                            args = a;
                            invocationCount++;
                            break;
                        case "Title":
                            titleInvocationCount++;
                            break;
                    }
                };
            }

            Assert.AreEqual(test, model.Test);
            Assert.AreEqual(initialTitle, model.Title);
            Assert.AreEqual(0, invocationCount);
            Assert.AreEqual(0, titleInvocationCount);

            model.Test = expectedTest;

            Assert.AreEqual(expectedTest, model.Test);
            Assert.AreEqual(expectedTitle, model.Title);
            Assert.AreEqual(expectedCount, invocationCount);
            Assert.AreEqual(expectedCount, titleInvocationCount);
            if (!isChangedEventNull)
            {
                Assert.IsNotNull(sender);
                Assert.AreSame(model, sender);
                Assert.IsNotNull(args);
                Assert.AreEqual("Test", args.PropertyName);
                CollectionAssert.AreEquivalent(
                    new[] {"Test", "Title", "DisplayName", "FullDisplayName", "ConditionalDisplayName", "TestFilter"},
                    changedProps);
            }
        }

        [Test]
        public void TestTestPropertyCanBeSetAndDoesNotInvokePropertyChangedEventIfSetValueIsSameAsCurrentValue(
            [Values] bool withTest)
        {
            int invocationCount = 0;

            NUnitRunner runner = new NUnitRunner("runner-name");
            TestSuite suite = new TestSuite("suite-name");
            ITest testInstance = suite;
            ITest test = withTest ? testInstance : null;
            string expectedTitle = withTest ? testInstance.FullName : runner.TestSuite.FullName;

            TestsViewModel model = new TestsViewModel(runner, test);

            model.PropertyChanged += (s, a) => { invocationCount++; };

            Assert.AreEqual(test, model.Test);
            Assert.AreEqual(expectedTitle, model.Title);
            Assert.AreEqual(0, invocationCount);

            model.Test = test;

            Assert.AreEqual(test, model.Test);
            Assert.AreEqual(expectedTitle, model.Title);
            Assert.AreEqual(0, invocationCount);
        }

        #endregion

        #region Tests for Result Property

        [Test]
        public void TestResultPropertyCanBeSetAndInvokesPropertyChangedEventIfSetValueIsNotSameAsCurrentValue(
            [Values] bool withResult, [Values] bool isChangedEventNull)
        {
            object sender = null;
            PropertyChangedEventArgs args = null;
            int invocationCount = 0;
            int expectedCount = isChangedEventNull ? 0 : 1;

            NUnitRunner runner = new NUnitRunner("runner-name");
            TestSuite test = new TestSuite("suite-name");
            INUnitTestResult result = withResult ? null : new NUnitTestResult(new TestSuiteResult(test));

            TestsViewModel model = new TestsViewModel(runner, test, result);

            INUnitTestResult initialResult =
                withResult ? new NUnitTestResult(new TestSuiteResult(new TestSuite("suite-name2"))) : null;
            model.Result = initialResult;

            IList<string> changedProps = new List<string>();
            if (!isChangedEventNull)
            {
                model.PropertyChanged += (s, a) =>
                {
                    changedProps.Add(a.PropertyName);
                    if (a.PropertyName.Equals("Result"))
                    {
                        sender = s;
                        args = a;
                        invocationCount++;
                    }
                };
            }

            Assert.AreEqual(initialResult, model.Result);
            Assert.AreEqual(0, invocationCount);

            model.Result = result;

            Assert.AreEqual(result, model.Result);
            Assert.AreEqual(test, model.Test);
            Assert.AreEqual(expectedCount, invocationCount);
            if (!isChangedEventNull)
            {
                Assert.IsNotNull(sender);
                Assert.AreSame(model, sender);
                Assert.IsNotNull(args);
                Assert.AreEqual("Result", args.PropertyName);
                CollectionAssert.AreEquivalent(new[] {"Result", "HasResult"}, changedProps);
            }
        }

        [Test]
        public void TestResultPropertyCanBeSetAndDoesNotInvokePropertyChangedEventIfSetValueIsSameAsCurrentValue(
            [Values] bool withResult, [Values] bool withTest)
        {
            int invocationCount = 0;

            NUnitRunner runner = new NUnitRunner("runner-name");
            TestSuite suite = new TestSuite("suite-name");
            INUnitTestResult result = withResult ? new NUnitTestResult(new TestSuiteResult(suite)) : null;
            ITest test = withTest ? suite : null;

            TestsViewModel model = new TestsViewModel(runner, test);
            model.Result = result;

            model.PropertyChanged += (s, a) => { invocationCount++; };

            Assert.AreEqual(result, model.Result);
            Assert.AreEqual(0, invocationCount);

            model.Result = result;

            Assert.AreEqual(result, model.Result);
            Assert.AreEqual(test, model.Test);
            Assert.AreEqual(0, invocationCount);
        }

        #endregion

        #region Tests for HasResult Property

        [Test]
        public void TestHasResultPropertyReturnsIfTestHasResult([Values] bool hasResult)
        {
            TestSuite suite = new TestSuite("suite-name");
            INUnitTestResult result = hasResult ? new NUnitTestResult(new TestSuiteResult(suite)) : null;

            TestsViewModel test = new TestsViewModel(v_Runner, null);
            test.Result = result;

            Assert.AreEqual(hasResult, test.HasResult);
        }

        #endregion

        #region Tests for DisplayName Property

        [Test]
        public void TestDisplayNamePropertyWithTestNullReturnsEmptyString()
        {
            TestsViewModel test = new TestsViewModel(v_Runner, null);

            Assert.IsNull(test.Test);
            Assert.AreEqual(string.Empty, test.DisplayName);
        }

        [Test]
        public void TestDisplayNamePropertyWithTestFullNameAsDllFilePathReturnsFileNameWithoutExtension()
        {
            const string expected = "suite-name";
            string name = Path.Combine(TestContext.CurrentContext.WorkDirectory, expected + ".dll");
            ITest testFixture = new TestSuite(name);

            TestsViewModel test = new TestsViewModel(v_Runner, testFixture);

            Assert.IsNotNull(test.Test);
            Assert.AreEqual(expected, test.DisplayName);
        }

        [Test]
        public void TestDisplayNamePropertyWithTestFullNameNotADllFilePathReturnsFullName()
        {
            const string name = "suite-name";
            ITest testFixture = new TestSuite(name);

            TestsViewModel test = new TestsViewModel(v_Runner, testFixture);

            Assert.IsNotNull(test.Test);
            Assert.AreEqual(name, test.DisplayName);
        }

        #endregion

        #region Tests for FullDisplayName Property

        [Test]
        public void TestFullDisplayNamePropertyWithTestNullReturnsEmptyString()
        {
            TestsViewModel test = new TestsViewModel(v_Runner, null);

            Assert.IsNull(test.Test);
            Assert.AreEqual(string.Empty, test.FullDisplayName);
        }

        [Test]
        public void TestFullDisplayNamePropertyWithTestFullNameAsDllFilePathReturnsFileNameWithoutExtension()
        {
            const string expected = "suite-name";
            string name = Path.Combine(TestContext.CurrentContext.WorkDirectory, expected + ".dll");
            ITest testFixture = new TestSuite(name);

            TestsViewModel test = new TestsViewModel(v_Runner, testFixture);

            Assert.IsNotNull(test.Test);
            Assert.AreEqual(expected, test.FullDisplayName);
        }

        [Test]
        public void TestFullDisplayNamePropertyWithTestFullNameNotADllFilePathReturnsFullName()
        {
            const string name = "suite-name";
            ITest testFixture = new TestSuite(name);

            TestsViewModel test = new TestsViewModel(v_Runner, testFixture);

            Assert.IsNotNull(test.Test);
            Assert.AreEqual(name, test.FullDisplayName);
        }

        #endregion

        #region Tests for ConditionalDisplayName Property

        [Test]
        public void TestConditionalDisplayNamePropertyWithTestNullReturnsEmptyString()
        {
            TestsViewModel test = new TestsViewModel(v_Runner, null);

            Assert.IsNull(test.Test);
            Assert.AreEqual(string.Empty, test.ConditionalDisplayName);
        }

        [Test]
        public void TestConditionalDisplayNamePropertyWithTestIsSuiteAndNotClassAndNotMethodReturnsDisplayName()
        {
            MethodInfo info = GetType().GetMethods().First();
            IMethodInfo method = new MethodWrapper(GetType(), info);
            ITest testFixture = new TestMethod(method);

            TestsViewModel test = new TestsViewModel(v_Runner, testFixture);

            Assert.IsNotNull(test.Test);
            Assert.AreEqual(method.Name, test.ConditionalDisplayName);
            Assert.AreEqual(test.DisplayName, test.ConditionalDisplayName);
        }

        [Test]
        public void TestConditionalDisplayNamePropertyWithTestIsSuiteAndNotClassAndNotMethodReturnsFullDisplayName()
        {
            const string parentName = "parent";
            const string name = "suite-name";
            ITest testFixture = new TestSuite(parentName, name);

            TestsViewModel test = new TestsViewModel(v_Runner, testFixture);

            Assert.IsNotNull(test.Test);
            Assert.AreEqual($"{parentName}.{name}", test.ConditionalDisplayName);
            Assert.AreEqual(test.FullDisplayName, test.ConditionalDisplayName);
        }

        #endregion

        #region Tests for TestFilter Property

        [Test]
        public void TestTestFilterPropertyWithTestNullReturnsEmptyFilter()
        {
            TestsViewModel test = new TestsViewModel(v_Runner, null);

            Assert.IsNull(test.Test);
            Assert.IsNotNull(test.TestFilter);
            Assert.AreSame(NUnitFilter.Empty, test.TestFilter);
        }

        [Test]
        public void TestTestFilterPropertyWithTestNotNullReturnsFilterOnTestId()
        {
            ITest testFixture = new TestSuite("suite-name");
            string expected = NUnitFilter.Where.Id(testFixture.Id).Build().Filter.ToXml(true).OuterXml;

            TestsViewModel test = new TestsViewModel(v_Runner, testFixture);

            Assert.IsNotNull(test.Test);
            Assert.IsNotNull(test.TestFilter);
            Assert.AreEqual(expected, test.TestFilter.ToXml(true).OuterXml);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Get the first test or child test with the given name.
        /// </summary>
        /// <param name="test">The test to begin the search with.</param>
        /// <param name="fullName">The full name to search for.</param>
        /// <param name="skipCount">The number of test matches to skip before returning, or zero to return the first match.</param>
        /// <returns>The found test or null if not found.</returns>
        private static ITest GetTestByName(ITest test, string fullName, ref int skipCount)
        {
            if (test == null)
            {
                return null;
            }

            // Return if full name matches and skipCount is zero, otherwise just decrement skipCount
            if (test.FullName == fullName)
            {
                // Return test if skipCount has reached zero
                if (skipCount <= 0)
                {
                    return test;
                }

                // Decrement skipCount if greater than zero
                skipCount = skipCount > 0 ? skipCount - 1 : skipCount;
            }

            // Recursively search child tests, returning the first found test after skipCount reaches zero
            if (test.HasChildren)
            {
                foreach (ITest child in test.Tests)
                {
                    ITest found = GetTestByName(child, fullName, ref skipCount);
                    if (found == null)
                    {
                        continue;
                    }

                    // Return result if skipCount has reached zero
                    if (skipCount <= 0)
                    {
                        return found;
                    }

                    // Decrement skipCount if greater than zero
                    skipCount = skipCount > 0 ? skipCount - 1 : skipCount;
                }
            }

            return null;
        }

        /// <summary>
        ///     Get the first test result or child test result with the given name.
        /// </summary>
        /// <param name="result">The test result to begin the search with.</param>
        /// <param name="fullName">The full name to search for.</param>
        /// <param name="skipCount">The number of test result matches to skip before returning, or zero to return the first match.</param>
        /// <returns>The found test result or null if not found.</returns>
        private static ITestResult GetTestResultByName(ITestResult result, string fullName, ref int skipCount)
        {
            if (result == null)
            {
                return null;
            }

            // Return if full name matches and skipCount is zero, otherwise just decrement skipCount
            if (result.FullName == fullName)
            {
                // Return result if skipCount has reached zero
                if (skipCount <= 0)
                {
                    return result;
                }

                // Decrement skipCount if greater than zero
                skipCount = skipCount > 0 ? skipCount - 1 : skipCount;
            }

            // Recursively search child tests, returning the first found test after skipCount reaches zero
            if (result.HasChildren)
            {
                foreach (ITestResult child in result.Children)
                {
                    ITestResult found = GetTestResultByName(child, fullName, ref skipCount);
                    if (found == null)
                    {
                        continue;
                    }

                    // Return result if skipCount has reached zero
                    if (skipCount <= 0)
                    {
                        return found;
                    }

                    // Decrement skipCount if greater than zero
                    skipCount = skipCount > 0 ? skipCount - 1 : skipCount;
                }
            }

            return null;
        }

        /// <summary>
        ///     Waits for the command to finish.
        /// </summary>
        /// <param name="model">The model of the command being ran.</param>
        private static void WaitForCommand(BaseViewModel model)
        {
            while (model.IsBusy)
            {
                Thread.Sleep(10);
            }
        }

        #endregion
    }
}