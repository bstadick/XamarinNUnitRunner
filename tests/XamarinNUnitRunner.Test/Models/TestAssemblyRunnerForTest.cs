using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

namespace XamarinNUnitRunner.Test.Models
{
    /// <summary>
    ///     Implements ITestAssemblyRunner for use with tests.
    /// </summary>
    public class TestAssemblyRunnerForTest : ITestAssemblyRunner
    {
        #region Private Members

        /// <summary>
        ///     Holds the underlying ITestAssemblyRunner.
        /// </summary>
        private readonly ITestAssemblyRunner v_Runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

        /// <summary>
        ///     Holds the overloaded loaded test.
        /// </summary>
        private ITest v_LoadedTest;

        /// <summary>
        ///     If the LoadedTest Property should return the overloaded value.
        /// </summary>
        private bool v_LoadedTestOverride;

        /// <summary>
        ///     Holds the overloaded test result.
        /// </summary>
        private ITestResult v_Result;

        /// <summary>
        ///     If the Result Property should return the overloaded value.
        /// </summary>
        private bool v_ResultOverride;

        /// <summary>
        ///     Holds the overloaded is test loaded.
        /// </summary>
        private bool v_IsTestLoaded;

        /// <summary>
        ///     If the IsTestLoaded Property should return the overloaded value.
        /// </summary>
        private bool v_IsTestLoadedOverride;

        /// <summary>
        ///     Holds the overloaded is test running.
        /// </summary>
        private bool v_IsTestRunning;

        /// <summary>
        ///     If the IsTestRunning Property should return the overloaded value.
        /// </summary>
        private bool v_IsTestRunningOverride;

        /// <summary>
        ///     Holds the overloaded is test complete.
        /// </summary>
        private bool v_IsTestComplete;

        /// <summary>
        ///     If the IsTestComplete Property should return the overloaded value.
        /// </summary>
        private bool v_IsTestCompleteOverride;

        #endregion

        #region Implementation of ITestAssemblyRunner

        /// <inheritdoc />
        public ITest Load(string assemblyName, IDictionary<string, object> settings)
        {
            v_IsTestLoadedOverride = false;
            return v_Runner.Load(assemblyName, settings);
        }

        /// <inheritdoc />
        public ITest Load(Assembly assembly, IDictionary<string, object> settings)
        {
            v_IsTestLoadedOverride = false;
            return v_Runner.Load(assembly, settings);
        }

        /// <inheritdoc />
        public int CountTestCases(ITestFilter filter)
        {
            return v_Runner.CountTestCases(filter);
        }

        /// <inheritdoc />
        public ITest ExploreTests(ITestFilter filter)
        {
            return v_Runner.ExploreTests(filter);
        }

        /// <inheritdoc />
        public ITestResult Run(ITestListener listener, ITestFilter filter)
        {
            v_IsTestCompleteOverride = false;
            v_IsTestRunningOverride = false;
            return v_Runner.Run(listener, filter);
        }

        /// <inheritdoc />
        public void RunAsync(ITestListener listener, ITestFilter filter)
        {
            v_IsTestCompleteOverride = false;
            v_IsTestRunningOverride = false;
            v_Runner.RunAsync(listener, filter);
        }

        /// <inheritdoc />
        public bool WaitForCompletion(int timeout)
        {
            return v_Runner.WaitForCompletion(timeout);
        }

        /// <inheritdoc />
        public void StopRun(bool force)
        {
            v_Runner.StopRun(force);
        }

        /// <inheritdoc />
        public ITest LoadedTest
        {
            get => v_LoadedTestOverride ? v_LoadedTest : v_Runner.LoadedTest;
            set
            {
                v_LoadedTestOverride = true;
                v_LoadedTest = value;
            }
        }

        /// <inheritdoc />
        public ITestResult Result
        {
            get => v_ResultOverride ? v_Result : v_Runner.Result;
            set
            {
                v_ResultOverride = true;
                v_Result = value;
            }
        }

        /// <inheritdoc />
        public bool IsTestLoaded
        {
            get => v_IsTestLoadedOverride ? v_IsTestLoaded : v_Runner.IsTestLoaded;
            set
            {
                v_IsTestLoadedOverride = true;
                v_IsTestLoaded = value;
            }
        }

        /// <inheritdoc />
        public bool IsTestRunning
        {
            get => v_IsTestRunningOverride ? v_IsTestRunning : v_Runner.IsTestRunning;
            set
            {
                v_IsTestRunningOverride = true;
                v_IsTestRunning = value;
            }
        }

        /// <inheritdoc />
        public bool IsTestComplete
        {
            get => v_IsTestCompleteOverride ? v_IsTestComplete : v_Runner.IsTestComplete;
            set
            {
                v_IsTestCompleteOverride = true;
                v_IsTestComplete = value;
            }
        }

        #endregion
    }
}