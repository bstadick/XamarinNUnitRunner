using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace XamarinNUnitRunner.Models
{
    /// <summary>
    ///     Extends the <see cref="TestSuite" /> which represents a composite test, which contains other tests.
    /// </summary>
    public class NUnitSuite : TestSuite
    {
        #region Private Members

        /// <summary>
        ///     Holds the loaded test assemblies and their associated test assembly runner.
        /// </summary>
        private readonly Dictionary<Assembly, ITestAssemblyRunner> v_Assemblies =
            new Dictionary<Assembly, ITestAssemblyRunner>();

        #endregion

        #region Public Members

        /// <summary>
        ///     Gets the list of loaded test assemblies.
        /// </summary>
        public IList<Assembly> TestAssemblies => v_Assemblies.Keys.ToList();

        /// <summary>
        ///     Gets the list of loaded ITestAssemblyRunner.
        /// </summary>
        public IList<ITestAssemblyRunner> TestAssemblyRunners => v_Assemblies.Values.ToList();

        /// <summary>
        ///     Indicates whether a test is currently running.
        /// </summary>
        public bool IsTestRunning => v_Assemblies.Values.Any(test => test.IsTestRunning);

        /// <summary>
        ///     Indicates whether all test runs are complete.
        /// </summary>
        public bool IsTestComplete => v_Assemblies.Values.All(test => test.IsTestComplete);

        #endregion

        #region Constructors

        /// <inheritdoc />
        public NUnitSuite(string name) : base(name)
        {
        }

        /// <inheritdoc />
        public NUnitSuite(string parentSuiteName, string name) : base(parentSuiteName, name)
        {
        }

        /// <inheritdoc />
        public NUnitSuite(ITypeInfo fixtureType, object[] arguments = null) : base(fixtureType, arguments)
        {
        }

        /// <inheritdoc />
        public NUnitSuite(Type fixtureType) : base(fixtureType)
        {
        }

        /// <inheritdoc />
        public NUnitSuite(TestSuite suite, ITestFilter filter) : base(suite, filter)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Loads and adds a test to the suite.
        /// </summary>
        /// <remarks>
        ///     Check the returned test for null or the <see cref="ITest.RunState" /> to make sure that the test has been
        ///     loaded and added.
        /// </remarks>
        /// <param name="assembly">The assembly to load and add.</param>
        /// <param name="settings">Dictionary of options to use in loading the test.</param>
        /// <returns>
        ///     An <see cref="ITest" /> representing the loaded tests, or <c>null</c> if the assembly is already loaded or the
        ///     load failed.
        /// </returns>
        /// <exception cref="ArgumentNullException"><see cref="assembly" /> is <c>null</c>.</exception>
        public ITest Add(Assembly assembly, IDictionary<string, object> settings = null)
        {
            if (assembly == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(assembly));
            }

            if (ContainsAssembly(assembly))
            {
                return GetTestAssemblyRunner(assembly).LoadedTest;
            }

            // Empty settings if null
            if (settings == null)
            {
                settings = new Dictionary<string, object>();
            }

            ITestAssemblyRunner runner = CreateTestAssemblyRunner();

            // Test will be null if not loaded or marked as invalid
            Test test = LoadTest(runner, assembly, settings);
            if (test == null)
            {
                return null;
            }

            if (test.RunState == RunState.NotRunnable)
            {
                return test;
            }

            // Add test and runner if correctly loaded
            v_Assemblies.Add(assembly, runner);
            Add(test);

            return test;
        }

        /// <summary>
        ///     Gets if the assembly is in the suite.
        /// </summary>
        /// <param name="assembly">The test assembly to check for.</param>
        /// <returns><c>true</c> if the assembly is in the suite, otherwise <c>false</c>.</returns>
        public bool ContainsAssembly(Assembly assembly)
        {
            return assembly != null && v_Assemblies.ContainsKey(assembly);
        }

        /// <summary>
        ///     Gets the <see cref="ITestAssemblyRunner" /> associated with the given assembly from the suite, or <c>null</c> if
        ///     not present.
        /// </summary>
        /// <param name="assembly">The test assembly associated with the ITestAssemblyRunner to get.</param>
        /// <returns>
        ///     The <see cref="ITestAssemblyRunner" /> associated with the given assembly from the suite, or <c>null</c> if
        ///     not present.
        /// </returns>
        public ITestAssemblyRunner GetTestAssemblyRunner(Assembly assembly)
        {
            return ContainsAssembly(assembly) ? v_Assemblies[assembly] : null;
        }

        /// <summary>
        ///     Explore the test cases using a filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The <see cref="ITest" /> for loaded assemblies with test cases that matches the filter.</returns>
        public ITest ExploreTests(ITestFilter filter)
        {
            TestSuite suite = new TestSuite(Name);
            foreach (ITestAssemblyRunner runner in TestAssemblyRunners)
            {
                if (runner.ExploreTests(filter) is Test test)
                {
                    suite.Add(test);
                }
            }

            return suite;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates a new <see cref="ITestAssemblyRunner" />.
        /// </summary>
        /// <returns>A new <see cref="ITestAssemblyRunner" />.</returns>
        protected virtual ITestAssemblyRunner CreateTestAssemblyRunner()
        {
            return new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
        }

        /// <summary>
        ///     Loads the tests found in an Assembly, returning an indication of whether or not the load succeeded.
        /// </summary>
        /// <param name="runner">The test assembly runner to load the test assembly in to.</param>
        /// <param name="assembly">The assembly to load.</param>
        /// <param name="settings">Dictionary of options to use in loading the test.</param>
        /// <returns>A <see cref="Test" /> representing the loaded tests.</returns>
        protected virtual Test LoadTest(ITestAssemblyRunner runner, Assembly assembly,
            IDictionary<string, object> settings)
        {
            return runner?.Load(assembly, settings) as Test;
        }

        #endregion
    }
}