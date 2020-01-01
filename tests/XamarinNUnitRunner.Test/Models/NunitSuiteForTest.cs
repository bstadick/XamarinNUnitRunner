using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using XamarinNUnitRunner.Models;

namespace XamarinNUnitRunner.Test.Models
{
    /// <summary>
    ///     Implements NUnitSuite for use with tests.
    /// </summary>
    public class NUnitSuiteForTest : NUnitSuite
    {
        #region Public Members

        /// <summary>
        ///     Controls if the loaded test is to be null.
        /// </summary>
        public bool IsLoadedTestNull { get; set; }

        /// <summary>
        ///     Controls if the loaded test is to be invalid.
        /// </summary>
        public bool IsLoadedTestInvalid { get; set; }

        /// <summary>
        ///     Overrides base loading and loads the provided test, or null for base functionality.
        /// </summary>
        public NUnit.Framework.Internal.Test TestToLoad { get; set; }

        /// <summary>
        ///     Overrides base test assembly runner creation with the provided runner, or null for base functionality.
        /// </summary>
        public ITestAssemblyRunner RunnerToLoad { get; set; }

        #endregion

        #region Constructors

        /// <inheritdoc />
        public NUnitSuiteForTest(string name) : base(name)
        {
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override ITestAssemblyRunner CreateTestAssemblyRunner()
        {
            // Return base functionality
            if (RunnerToLoad == null)
            {
                return base.CreateTestAssemblyRunner();
            }

            // Return custom runner
            ITestAssemblyRunner runner = RunnerToLoad;
            RunnerToLoad = null;
            return runner;
        }

        /// <inheritdoc />
        protected override NUnit.Framework.Internal.Test LoadTest(ITestAssemblyRunner runner, Assembly assembly,
            IDictionary<string, object> settings)
        {
            // Return null test
            if (IsLoadedTestNull)
            {
                IsLoadedTestNull = false;
                return null;
            }

            // Return test with RunState as NotRunnable
            if (IsLoadedTestInvalid)
            {
                IsLoadedTestInvalid = false;
                TestSuite suite = new TestSuite(assembly.FullName);
                suite.RunState = RunState.NotRunnable;
                return suite;
            }

            // Return base functionality
            if (TestToLoad == null)
            {
                return base.LoadTest(runner, assembly, settings);
            }

            // Return custom test
            NUnit.Framework.Internal.Test test = TestToLoad;
            TestToLoad = null;
            return test;
        }

        #endregion
    }
}