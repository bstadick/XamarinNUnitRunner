using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
    public class TestViewModelTest
    {
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
        }

        [Test]
        public void TestConstructorWithTestSetsTestAndResultAndTitleAndCommands([Values] bool withTestRunner,
            [Values] bool withTest, [Values] bool withResult)
        {
            const string runnerName = "runner-name";
            const string suiteName = "suite-name";
            string fullSuiteName = Path.Combine(TestContext.CurrentContext.WorkDirectory, suiteName + ".dll");
            TestSuite suite = new TestSuite(fullSuiteName);

            string expectedTitle = withTest ? suiteName : string.Empty;

            NUnitRunner runner = withTestRunner ? new NUnitRunner(runnerName) : null;
            NUnitTest test = withTest ? new NUnitTest(suite) : null;
            ITestResult result = null;
            if (withTest)
            {
                result = withResult ? new TestSuiteResult(suite) : null;
                test.Result = result;
            }

            TestsViewModel model = new TestsViewModel(runner, test);

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(expectedTitle, model.Title);
            Assert.AreEqual(test, model.Test);
            Assert.AreEqual(result, model.Result);
            Assert.AreEqual(runner, model.TestRunner);
            Assert.IsNotNull(model.Tests);
            CollectionAssert.IsEmpty(model.Tests);
            Assert.IsNotNull(model.LoadTestsCommand);
            Assert.IsTrue(model.LoadTestsCommand.CanExecute(null));
            Assert.IsNotNull(model.RunTestsCommand);
            Assert.IsTrue(model.RunTestsCommand.CanExecute(null));
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
            NUnitTest test = new NUnitTest(runner.TestSuite);

            TestsViewModel model = new TestsViewModel(runner, test);
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
            NUnitTest test = new NUnitTest(runner.TestSuite);

            TestsViewModel model = new TestsViewModel(null, test);

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
            NUnitTest test = new NUnitTest(runner.TestSuite);

            Assert.AreEqual(2, runner.TestSuite.TestAssemblyRunners.Count);

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
            Assert.AreEqual(2, invocationCount);
            CollectionAssert.IsEmpty(model.Tests);
        }

        [Test]
        public void TestLoadTestsCommandClearsAndReturnsWhenTestTestIsNullOrHasNoChildren([Values] bool isNull)
        {
            int invocationCount = 0;
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            NUnitTest test = new NUnitTest(runner.TestSuite);
            test.Test = isNull ? null : new TestSuite("suite-name");

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
            if (isNull)
            {
                Assert.IsNull(model.Test.Test);
            }
            else
            {
                Assert.IsNotNull(model.Test.Test);
                Assert.IsFalse(model.Test.Test.HasChildren);
            }

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

            NUnitTest testInstance = new NUnitTest(runner.TestSuite);
            NUnitTest test = withTest ? testInstance : null;

            TestsViewModel model = new TestsViewModel(runner, test);

            ITestFilter filter = NUnitFilter.Where.Class(typeof(TestFixtureStubOne).Name).Build().Filter;
            ITestResult expectedResult = withResults ? runner.RunTests(null, filter) : null;
            List<ITestResult> expectedResults = new List<ITestResult>();
            model.Result = expectedResult;
            if (withResults)
            {
                expectedResults.AddRange(expectedResult.Children);
            }
            else
            {
                expectedResults.Add(null);
            }

            if (withAdditionalTests && !withResults)
            {
                expectedResults.Add(null);
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
            Assert.AreEqual(testInstance.Test, model.Test.Test);

            IList<ITest> actualTests = model.Tests.Select(x => x.Test).ToList();
            CollectionAssert.AreEquivalent(testInstance.Test.Tests, actualTests);

            IList<ITestResult> actualResults = model.Tests.Select(x => x.Result).ToList();
            CollectionAssert.AreEquivalent(expectedResults, actualResults);
            Assert.AreEqual(expectedResult, model.Result);
        }

        #endregion

        #region Tests for RunTestsCommand Property

        [Test]
        public void TestRunTestsCommandReturnsImmediatelyWhenIsBusyIsTrue()
        {
            int invocationCount = 0;
            NUnitRunner runner = new NUnitRunner("runner-name");
            runner.AddTestAssembly(typeof(TestFixtureStubOne).Assembly);
            NUnitTest test = new NUnitTest(runner.TestSuite);

            TestsViewModel model = new TestsViewModel(runner, test);
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
            NUnitTest test = new NUnitTest(runner.TestSuite);

            TestsViewModel model = new TestsViewModel(null, test);

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
            NUnitTest test = new NUnitTest(runner.TestSuite);

            Assert.AreEqual(2, runner.TestSuite.TestAssemblyRunners.Count);

            TestsViewModel model = new TestsViewModel(runner, test);

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

            NUnitTest testInstance = new NUnitTest(runner.TestSuite);
            NUnitTest test = withTest ? testInstance : null;

            TestsViewModel model = new TestsViewModel(runner, test);

            ITestFilter filterInstance = NUnitFilter.Where.Class(typeof(TestFixtureStubOne).Name).Build().Filter;
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
            Assert.AreEqual(testInstance.Test, model.Test.Test);

            IList<ITestResult> actualResults = model.Tests.Select(x => x.Result).ToList();
            if (withPreload)
            {
                IList<ITest> actualTests = model.Tests.Select(x => x.Test).ToList();
                CollectionAssert.AreEquivalent(testInstance.Test.Tests, actualTests);
                CollectionAssert.AreEquivalent(expectedResults.Select(x => x.Test), actualResults.Select(x => x.Test));
                CollectionAssert.AreEquivalent(expectedResults.Select(x => x.ResultState),
                    actualResults.Select(x => x.ResultState));
            }
            else
            {
                CollectionAssert.IsEmpty(actualResults);
            }

            Assert.AreEqual(expectedResult.Test, model.Result.Test);
            Assert.AreEqual(expectedResult.ResultState, model.Result.ResultState);
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
            NUnitTest testInstance = new NUnitTest(suite);
            NUnitTest test = withTest ? testInstance : null;
            NUnitTest expectedTest = withTest ? null : testInstance;
            string initialTitle = withTest ? testInstance.DisplayName : string.Empty;
            string expectedTitle = withTest ? string.Empty : testInstance.DisplayName;

            TestsViewModel model = new TestsViewModel(runner, test);

            if (!isChangedEventNull)
            {
                model.PropertyChanged += (s, a) =>
                {
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
            }
        }

        [Test]
        public void TestTestPropertyCanBeSetAndDoesNotInvokePropertyChangedEventIfSetValueIsSameAsCurrentValue(
            [Values] bool withTest)
        {
            int invocationCount = 0;

            NUnitRunner runner = new NUnitRunner("runner-name");
            TestSuite suite = new TestSuite("suite-name");
            NUnitTest testInstance = new NUnitTest(suite);
            NUnitTest test = withTest ? testInstance : null;
            string expectedTitle = withTest ? testInstance.DisplayName : string.Empty;

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
            [Values] bool withResult, [Values] bool withTest, [Values] bool isChangedEventNull)
        {
            object sender = null;
            PropertyChangedEventArgs args = null;
            int invocationCount = 0;
            int expectedCount = isChangedEventNull ? 0 : 1;

            NUnitRunner runner = new NUnitRunner("runner-name");
            TestSuite suite = new TestSuite("suite-name");
            ITestResult resultInstance = new TestSuiteResult(suite);
            ITestResult result = withResult ? null : resultInstance;
            NUnitTest test = withTest ? new NUnitTest(suite) : null;
            if (withTest)
            {
                test.Result = withResult ? resultInstance : null;
            }

            TestsViewModel model = new TestsViewModel(runner, test);

            ITestResult initialResult = test?.Result;
            if (withResult && !withTest)
            {
                initialResult = resultInstance;
                model.Result = resultInstance;
            }

            if (!isChangedEventNull)
            {
                model.PropertyChanged += (s, a) =>
                {
                    sender = s;
                    args = a;
                    invocationCount++;
                };
            }

            Assert.AreEqual(initialResult, model.Result);
            Assert.AreEqual(0, invocationCount);

            model.Result = result;

            Assert.AreEqual(result, model.Result);
            Assert.AreEqual(test, model.Test);
            Assert.AreEqual(test?.Result, model.Test?.Result);
            Assert.AreEqual(expectedCount, invocationCount);
            if (!isChangedEventNull)
            {
                Assert.IsNotNull(sender);
                Assert.AreSame(model, sender);
                Assert.IsNotNull(args);
                Assert.AreEqual("Result", args.PropertyName);
            }
        }

        [Test]
        public void TestResultPropertyCanBeSetAndDoesNotInvokePropertyChangedEventIfSetValueIsSameAsCurrentValue(
            [Values] bool withResult, [Values] bool withTest)
        {
            int invocationCount = 0;

            NUnitRunner runner = new NUnitRunner("runner-name");
            TestSuite suite = new TestSuite("suite-name");
            ITestResult resultInstance = new TestSuiteResult(suite);
            ITestResult result = withResult ? resultInstance : null;
            NUnitTest test = withTest ? new NUnitTest(suite) : null;
            if (withTest)
            {
                test.Result = result;
            }

            TestsViewModel model = new TestsViewModel(runner, test);

            if (!withTest)
            {
                model.Result = result;
            }

            model.PropertyChanged += (s, a) => { invocationCount++; };

            Assert.AreEqual(result, model.Result);
            Assert.AreEqual(0, invocationCount);

            model.Result = result;

            Assert.AreEqual(result, model.Result);
            Assert.AreEqual(test, model.Test);
            Assert.AreEqual(test?.Result, model.Test?.Result);
            Assert.AreEqual(0, invocationCount);
        }

        #endregion

        #region Tests for ResultString Property

        [Test]
        public void TestResultStringPropertyWithResultNullReturnsDefaultString()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            TestsViewModel model = new TestsViewModel(runner);

            Assert.IsNull(model.Result);
            Assert.AreEqual("Test not executed.", model.ResultString);
        }

        [Test]
        public void TestResultStringPropertyWithResultReturnsResultStateStatusString()
        {
            NUnitRunner runner = new NUnitRunner("runner-name");
            TestSuite suite = new TestSuite("suite-name");
            ITestResult result = new TestSuiteResult(suite);

            TestsViewModel model = new TestsViewModel(runner);
            model.Result = result;

            Assert.IsNotNull(model.Result);
            Assert.AreEqual(result.ResultState.Status.ToString(), model.ResultString);
        }

        #endregion

        #region Private Methods

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