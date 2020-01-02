using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using XamarinNUnitRunner.Filter;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Test.Stub;

namespace XamarinNUnitRunner.Test.Models
{
    [TestFixture]
    public class NUnitSuiteTest
    {
        #region Private Members

        private const string c_SuiteXmlElement = "test-suite";

        #endregion

        #region Tests for TestAssemblies Property

        [Test]
        public void TestTestAssembliesPropertyReturnsListOfAddedAssemblies([Values] bool isEmpty)
        {
            NUnitSuite suite = new NUnitSuite("suite-name");

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;
            IList<Assembly> expected = new List<Assembly> {assembly};
            if (!isEmpty)
            {
                suite.Add(assembly);
            }

            IList<Assembly> assemblies = suite.TestAssemblies;

            Assert.IsNotNull(assemblies);
            if (isEmpty)
            {
                CollectionAssert.IsEmpty(assemblies);
            }
            else
            {
                CollectionAssert.AreEquivalent(expected, assemblies);
            }
        }

        #endregion

        #region Tests for TestAssemblyRunners Property

        [Test]
        public void TestTestAssemblyRunnersPropertyReturnsListOfAddedTestAssemblyRunners([Values] bool isEmpty)
        {
            NUnitSuite suite = new NUnitSuite("suite-name");

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;
            IList<ITestAssemblyRunner> expected = new List<ITestAssemblyRunner>();
            if (!isEmpty)
            {
                suite.Add(assembly);
                expected.Add(suite.GetTestAssemblyRunner(assembly));
            }

            IList<ITestAssemblyRunner> runners = suite.TestAssemblyRunners;

            Assert.IsNotNull(runners);
            if (isEmpty)
            {
                CollectionAssert.IsEmpty(runners);
            }
            else
            {
                CollectionAssert.AreEquivalent(expected, runners);
            }
        }

        #endregion

        #region Tests for IsTestRunning Property

        [Test]
        public void TestIsTestRunningPropertyReturnsFalseIfNoTestIsRunning([Values] bool isEmpty)
        {
            NUnitSuite suite = new NUnitSuite("suite-name");

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;
            if (!isEmpty)
            {
                suite.Add(assembly);
            }

            bool running = suite.IsTestRunning;

            Assert.IsFalse(running);
        }

        [Test]
        public void TestIsTestRunningPropertyReturnsTrueIfAnyTestIsRunning()
        {
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");

            suite.Add(GetType().Assembly);

            TestAssemblyRunnerForTest runner = new TestAssemblyRunnerForTest();
            runner.IsTestRunning = true;
            suite.RunnerToLoad = runner;
            suite.Add(typeof(TestFixtureStubOne).Assembly);

            bool running = suite.IsTestRunning;

            Assert.IsTrue(running);
        }

        #endregion

        #region Tests for IsTestComplete Property

        [Test]
        public void TestIsTestCompletePropertyReturnsFalseIfNotAllTestsAreComplete()
        {
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");

            TestAssemblyRunnerForTest runner = new TestAssemblyRunnerForTest();
            runner.IsTestComplete = true;
            suite.RunnerToLoad = runner;
            suite.Add(GetType().Assembly);

            runner = new TestAssemblyRunnerForTest();
            runner.IsTestComplete = false;
            suite.RunnerToLoad = runner;
            suite.Add(typeof(TestFixtureStubOne).Assembly);

            bool completed = suite.IsTestComplete;

            Assert.IsFalse(completed);
        }

        [Test]
        public void TestIsTestCompletePropertyReturnsTrueIfAllTestsAreComplete([Values] bool isEmpty)
        {
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");

            if (!isEmpty)
            {
                TestAssemblyRunnerForTest runner = new TestAssemblyRunnerForTest();
                runner.IsTestComplete = true;
                suite.RunnerToLoad = runner;
                suite.Add(GetType().Assembly);

                runner = new TestAssemblyRunnerForTest();
                runner.IsTestComplete = true;
                suite.RunnerToLoad = runner;
                suite.Add(typeof(TestFixtureStubOne).Assembly);
            }

            bool completed = suite.IsTestComplete;

            Assert.IsTrue(completed);
        }

        #endregion

        #region Tests for Constructor

        [Test]
        public void TestConstructorThrowsArgumentExceptionWhenNameIsEmpty()
        {
            string name = string.Empty;
            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("Argument name must not be the empty string (Parameter 'name')"),
                // ReSharper disable once ObjectCreationAsStatement
                () => { new NUnitSuite(name); });
        }

        [Test]
        public void TestConstructorThrowsArgumentExceptionWhenNameIsNullOrEmpty()
        {
            const string name = null;
            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("Argument name must not be null (Parameter 'name')"),
                // ReSharper disable once ObjectCreationAsStatement
                () => { new NUnitSuite(name); });
        }

        [Test]
        public void TestConstructorWithFixtureType()
        {
            TypeWrapper wrapper = new TypeWrapper(GetType());
            string name = wrapper.GetDisplayName();
            string parentName = wrapper.Namespace;
            string expectedFullName = name;
            if (!string.IsNullOrEmpty(parentName))
            {
                expectedFullName = parentName + "." + name;
            }

            NUnitSuite suite = new NUnitSuite(GetType());

            Assert.AreEqual(name, suite.Name);
            Assert.AreEqual(expectedFullName, suite.FullName);
            Assert.AreEqual(wrapper.FullName, suite.ClassName);
            Assert.AreEqual(wrapper.FullName, suite.TypeInfo.FullName);

            TestCommonConstructorProperties(suite);
        }

        [Test]
        public void TestConstructorWithFixtureTypeInfoAndArguments([Values] bool isArgsNull)
        {
            object[] args = isArgsNull ? null : new object[] {"arg1", "arg2"};
            TypeWrapper wrapper = new TypeWrapper(GetType());
            string name = wrapper.GetDisplayName();
            string parentName = wrapper.Namespace;
            string expectedFullName = name;
            if (!string.IsNullOrEmpty(parentName))
            {
                expectedFullName = parentName + "." + name;
            }

            NUnitSuite suite = new NUnitSuite(wrapper, args);

            Assert.AreEqual(name, suite.Name);
            Assert.AreEqual(expectedFullName, suite.FullName);
            Assert.AreEqual(wrapper.FullName, suite.ClassName);
            Assert.IsNotNull(suite.Arguments);
            Assert.AreEqual(wrapper.FullName, suite.TypeInfo.FullName);

            TestCommonConstructorProperties(suite, isArgsNull);
            if (!isArgsNull)
            {
                CollectionAssert.AreEquivalent(args, suite.Arguments);
            }
        }

        [Test]
        public void TestConstructorWithName()
        {
            const string name = "suite-name";

            NUnitSuite suite = new NUnitSuite(name);

            Assert.AreEqual(name, suite.Name);
            Assert.AreEqual(name, suite.FullName);
            Assert.IsNull(suite.ClassName);
            Assert.IsNull(suite.TypeInfo);

            TestCommonConstructorProperties(suite);
        }

        [Test]
        public void TestConstructorWithParentSuiteNameAndName([Values("parent-name", null, "")] string parentName,
            [Values("suite-name", null, "")] string name)
        {
            string expectedFullName = name;
            if (!string.IsNullOrEmpty(parentName))
            {
                expectedFullName = parentName + "." + name;
            }

            NUnitSuite suite = new NUnitSuite(parentName, name);

            Assert.AreEqual(name, suite.Name);
            Assert.AreEqual(expectedFullName, suite.FullName);
            Assert.IsNull(suite.ClassName);
            Assert.IsNull(suite.TypeInfo);

            TestCommonConstructorProperties(suite);
        }

        [Test]
        public void TestConstructorWithTestSuiteAndFilter([Values] bool withChildTests, [Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : NUnitFilter.Empty;

            NUnitSuite suiteInitial = new NUnitSuite("suite-one");
            if (withChildTests)
            {
                suiteInitial.Add(typeof(TestFixtureStubOne).Assembly);
            }

            NUnitSuite suite = new NUnitSuite(suiteInitial, filter);

            Assert.AreEqual(suiteInitial.Name, suite.Name);
            Assert.AreEqual(suiteInitial.FullName, suite.FullName);
            Assert.IsNull(suite.ClassName);
            Assert.IsNull(suite.TypeInfo);
            Assert.IsNotNull(suite.Id);
            Assert.IsNotEmpty(suite.Id);
            Assert.IsNull(suite.MethodName);
            Assert.IsNull(suite.Method);
            Assert.IsNull(suite.Arguments);
            Assert.AreEqual(RunState.Runnable, suite.RunState);
            Assert.AreEqual(c_SuiteXmlElement, suite.XmlElementName);
            Assert.AreEqual(suite.GetType().Name, suite.TestType);
            Assert.IsNotNull(suite.Properties);
            CollectionAssert.IsEmpty(suite.Properties.Keys);
            Assert.IsTrue(suite.IsSuite);
            Assert.IsNull(suite.Parent);
            Assert.IsNotNull(suite.Tests);
            if (withChildTests)
            {
                int testCount = withFilter ? 1 : TestFixtureStubHelper.GeTestFixtureStub().TestCount;
                Assert.AreEqual(testCount, suite.TestCaseCount);
                Assert.IsTrue(suite.HasChildren);
                CollectionAssert.IsNotEmpty(suite.Tests);
            }
            else
            {
                Assert.AreEqual(0, suite.TestCaseCount);
                Assert.IsFalse(suite.HasChildren);
                CollectionAssert.IsEmpty(suite.Tests);
            }

            Assert.IsNotNull(suite.SetUpMethods);
            CollectionAssert.IsEmpty(suite.SetUpMethods);
            Assert.IsNotNull(suite.TearDownMethods);
            CollectionAssert.IsEmpty(suite.TearDownMethods);
        }

        #endregion

        #region Tests for Add

        [Test]
        public void TestAddThrowsArgumentNullExceptionWhenAssemblyIsNull()
        {
            NUnitSuite suite = new NUnitSuite("suite-name");
            Assembly assembly = null;

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The assembly cannot be null. (Parameter 'assembly')"),
                // ReSharper disable once ExpressionIsAlwaysNull
                () => suite.Add(assembly));
        }

        [Test]
        public void TestAddWithAssemblyAlreadyAddedDoesNotReAddAssemblyAndReturnsAlreadyAddedAssembly()
        {
            NUnitSuite suite = new NUnitSuite("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;
            List<Assembly> expectedAssemblies = new List<Assembly> {assembly};

            ITest testInitial = suite.Add(assembly);

            Assert.IsNotNull(testInitial);
            Assert.AreEqual(RunState.Runnable, testInitial.RunState);
            Assert.AreEqual("XamarinNUnitRunner.Test.Stub.dll", testInitial.Name);
            Assert.IsTrue(suite.ContainsAssembly(assembly));
            Assert.IsTrue(suite.GetTestAssemblyRunner(assembly).IsTestLoaded);
            CollectionAssert.AreEquivalent(expectedAssemblies, suite.TestAssemblies);

            ITest test = suite.Add(assembly);

            Assert.IsNotNull(test);
            Assert.AreSame(testInitial, test);
            Assert.AreEqual(RunState.Runnable, test.RunState);
            Assert.AreEqual("XamarinNUnitRunner.Test.Stub.dll", test.Name);
            Assert.IsTrue(suite.ContainsAssembly(assembly));
            Assert.IsTrue(suite.GetTestAssemblyRunner(assembly).IsTestLoaded);
            CollectionAssert.AreEquivalent(expectedAssemblies, suite.TestAssemblies);
        }

        [Test]
        public void TestAddWithAssemblyLoadInvalidDoesNotAddAssemblyAndReturnsNull()
        {
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");
            suite.IsLoadedTestNull = true;

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            ITest test = suite.Add(assembly);

            Assert.IsNull(test);
            Assert.IsFalse(suite.ContainsAssembly(assembly));
            CollectionAssert.IsEmpty(suite.TestAssemblies);
        }

        [Test]
        public void TestAddWithAssemblyLoadNotRunnableDoesNotAddAssemblyAndReturnsErroneousTest()
        {
            NUnitSuiteForTest suite = new NUnitSuiteForTest("suite-name");
            suite.IsLoadedTestInvalid = true;

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;

            ITest test = suite.Add(assembly);

            Assert.IsNotNull(test);
            Assert.AreEqual(RunState.NotRunnable, test.RunState);
            Assert.IsFalse(suite.ContainsAssembly(assembly));
            CollectionAssert.IsEmpty(suite.TestAssemblies);
        }

        [Test]
        public void TestAddWithAssemblyNotAlreadyAddedLoadsAndAddsAssemblyAndReturnsAddedTest(
            [Values] bool withSettings)
        {
            string workingDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));
            string expectedDirectory = withSettings ? workingDirectory : Directory.GetCurrentDirectory();
            Dictionary<string, object> settings = withSettings
                ? new Dictionary<string, object>
                    {{FrameworkPackageSettings.WorkDirectory, workingDirectory}}
                : null;

            NUnitSuite suite = new NUnitSuite("suite-name");

            Assembly assembly = typeof(TestFixtureStubOne).Assembly;
            List<Assembly> expectedAssemblies = new List<Assembly> {assembly};

            ITest test = suite.Add(assembly, settings);

            Assert.IsNotNull(test);
            Assert.AreEqual(RunState.Runnable, test.RunState);
            Assert.AreEqual("XamarinNUnitRunner.Test.Stub.dll", test.Name);
            Assert.IsTrue(suite.ContainsAssembly(assembly));
            Assert.IsTrue(suite.GetTestAssemblyRunner(assembly).IsTestLoaded);
            Assert.AreEqual(expectedDirectory, TestContext.CurrentContext.WorkDirectory);
            CollectionAssert.AreEquivalent(expectedAssemblies, suite.TestAssemblies);
        }

        #endregion

        #region Tests for ContainsAssembly

        [Test]
        public void TestContainsAssemblyWithAssemblyAddedReturnsTrue()
        {
            NUnitSuite suite = new NUnitSuite("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;
            suite.Add(assembly);

            bool contains = suite.ContainsAssembly(assembly);

            Assert.IsTrue(contains);
        }

        [Test]
        public void TestContainsAssemblyWithAssemblyNotAddedReturnsFalse()
        {
            NUnitSuite suite = new NUnitSuite("suite-name");
            suite.Add(typeof(TestFixtureStubOne).Assembly);

            bool contains = suite.ContainsAssembly(GetType().Assembly);

            Assert.IsFalse(contains);
        }

        [Test]
        public void TestContainsAssemblyWithAssemblyNullReturnsFalse()
        {
            NUnitSuite suite = new NUnitSuite("suite-name");
            suite.Add(typeof(TestFixtureStubOne).Assembly);

            bool contains = suite.ContainsAssembly(null);

            Assert.IsFalse(contains);
        }

        #endregion

        #region Tests for GetTestAssemblyRunner

        [Test]
        public void TestGetTestAssemblyRunnerWithAssemblyAddedReturnsTestAssemblyRunner()
        {
            NUnitSuite suite = new NUnitSuite("suite-name");
            Assembly assembly = typeof(TestFixtureStubOne).Assembly;
            suite.Add(assembly);

            ITestAssemblyRunner runner = suite.GetTestAssemblyRunner(assembly);

            Assert.IsNotNull(runner);
        }

        [Test]
        public void TestGetTestAssemblyRunnerWithAssemblyNotAddedReturnsNull()
        {
            NUnitSuite suite = new NUnitSuite("suite-name");
            suite.Add(typeof(TestFixtureStubOne).Assembly);

            ITestAssemblyRunner runner = suite.GetTestAssemblyRunner(GetType().Assembly);

            Assert.IsNull(runner);
        }

        [Test]
        public void TestGetTestAssemblyRunnerWithAssemblyNullReturnsNull()
        {
            NUnitSuite suite = new NUnitSuite("suite-name");
            suite.Add(typeof(TestFixtureStubOne).Assembly);

            ITestAssemblyRunner runner = suite.GetTestAssemblyRunner(null);

            Assert.IsNull(runner);
        }

        #endregion

        #region Tests for ExploreTests

        [Test]
        public void TestExploreTestsWithAssemblyAddedReturnsLoadedTestCases([Values] bool withFilter)
        {
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(typeof(TestFixtureStubOne).FullName).And.Method("Test2").Build().Filter
                : NUnitFilter.Empty;
            int expected = withFilter ? 1 : TestFixtureStubHelper.GeTestFixtureStub().TestCount;

            NUnitSuite suite = new NUnitSuite("suite-name");
            suite.Add(typeof(TestFixtureStubOne).Assembly);

            ITest tests = suite.ExploreTests(filter);

            Assert.IsNotNull(tests);
            Assert.AreEqual(suite.Name, tests.Name);
            Assert.IsTrue(tests.HasChildren);
            Assert.AreEqual(1, tests.Tests.Count);
            Assert.AreEqual(expected, tests.TestCaseCount);
        }

        [Test]
        public void TestExploreTestsWithAssemblyNotAddedReturnsEmptyTestCases()
        {
            NUnitSuite suite = new NUnitSuite("suite-name");

            ITest tests = suite.ExploreTests(NUnitFilter.Empty);

            Assert.IsNotNull(tests);
            Assert.AreEqual(suite.Name, tests.Name);
            Assert.IsFalse(tests.HasChildren);
            Assert.AreEqual(0, tests.Tests.Count);
            Assert.AreEqual(0, tests.TestCaseCount);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Check common NUnitSuite constructor properties.
        /// </summary>
        /// <param name="suite">The NUnitSuite to check.</param>
        /// <param name="argsEmpty">If the argument list is considered empty</param>
        private static void TestCommonConstructorProperties(NUnitSuite suite, bool argsEmpty = true)
        {
            Assert.IsNotNull(suite.Id);
            Assert.IsNotEmpty(suite.Id);
            Assert.IsNull(suite.MethodName);
            Assert.IsNull(suite.Method);
            Assert.IsNotNull(suite.Arguments);
            if (argsEmpty)
            {
                CollectionAssert.IsEmpty(suite.Arguments);
            }

            Assert.AreEqual(RunState.Runnable, suite.RunState);
            Assert.AreEqual(c_SuiteXmlElement, suite.XmlElementName);
            Assert.AreEqual(suite.GetType().Name, suite.TestType);
            Assert.AreEqual(0, suite.TestCaseCount);
            Assert.IsNotNull(suite.Properties);
            CollectionAssert.IsEmpty(suite.Properties.Keys);
            Assert.IsTrue(suite.IsSuite);
            Assert.IsFalse(suite.HasChildren);
            Assert.IsNull(suite.Parent);
            Assert.IsNotNull(suite.Tests);
            CollectionAssert.IsEmpty(suite.Tests);
            Assert.IsNotNull(suite.SetUpMethods);
            CollectionAssert.IsEmpty(suite.SetUpMethods);
            Assert.IsNotNull(suite.TearDownMethods);
            CollectionAssert.IsEmpty(suite.TearDownMethods);
        }

        #endregion
    }
}