using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using XamarinNUnitRunner.Models;

namespace XamarinNUnitRunner.Services
{
    /// <summary>
    ///     Interface to manage and run NUnit test assemblies.
    /// </summary>
    public interface INUnitRunner
    {
        #region Properties

        /// <summary>
        ///     Gets the underlying test suite to load tests.
        /// </summary>
        NUnitSuite TestSuite { get; }

        /// <summary>
        ///     Indicates whether a test is currently running.
        /// </summary>
        bool IsTestRunning { get; }

        /// <summary>
        ///     Indicates whether all test runs are complete.
        /// </summary>
        bool IsTestComplete { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds a test assembly to the runner session.
        /// </summary>
        /// <param name="assembly">The assembly to load and add.</param>
        /// <param name="settings">Dictionary of options to use in loading the test.</param>
        /// <returns>
        ///     An <see cref="ITest" /> representing the loaded tests, or <c>null</c> if the assembly is already loaded or the
        ///     load failed.
        /// </returns>
        /// <exception cref="ArgumentNullException"><see cref="assembly" /> is <c>null</c>.</exception>
        ITest AddTestAssembly(Assembly assembly, IDictionary<string, object> settings = null);

        /// <summary>
        ///     Adds a test assembly to the runner session.
        /// </summary>
        /// <param name="assembly">The assembly to load and add.</param>
        /// <param name="settings">Dictionary of options to use in loading the test.</param>
        /// <returns>
        ///     An <see cref="ITest" /> representing the loaded tests, or <c>null</c> if the assembly is already loaded or the
        ///     load failed.
        /// </returns>
        /// <exception cref="ArgumentNullException"><see cref="assembly" /> is <c>null</c>.</exception>
        Task<ITest> AddTestAssemblyAsync(Assembly assembly, IDictionary<string, object> settings = null);

        /// <summary>
        ///     Count Test Cases using a filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The number of test cases found in the loaded assemblies that matches the filter.</returns>
        int CountTestCases(ITestFilter filter = null);

        /// <summary>
        ///     Count Test Cases using a filter.
        /// </summary>
        /// <param name="assembly">The assembly to count test case in.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The number of test cases found in the given assembly that matches the filter.</returns>
        /// <exception cref="ArgumentNullException"><see cref="assembly" /> is <c>null</c>.</exception>
        int CountTestCases(Assembly assembly, ITestFilter filter = null);

        /// <summary>
        ///     Count Test Cases using a filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The number of test cases found in the loaded assemblies that matches the filter.</returns>
        Task<int> CountTestCasesAsync(ITestFilter filter = null);

        /// <summary>
        ///     Count Test Cases using a filter.
        /// </summary>
        /// <param name="assembly">The assembly to count test case in.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The number of test cases found in the given assembly that matches the filter.</returns>
        /// <exception cref="ArgumentNullException"><see cref="assembly" /> is <c>null</c>.</exception>
        Task<int> CountTestCasesAsync(Assembly assembly, ITestFilter filter = null);

        /// <summary>
        ///     Explore the test cases using a filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The <see cref="ITest" /> for loaded assemblies with test cases that matches the filter.</returns>
        ITest ExploreTests(ITestFilter filter = null);

        /// <summary>
        ///     Explore the test cases using a filter.
        /// </summary>
        /// <param name="assembly">The assembly to explore test case in.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The <see cref="ITest" /> for the given assembly with test cases that matches the filter.</returns>
        /// <exception cref="ArgumentNullException"><see cref="assembly" /> is <c>null</c>.</exception>
        ITest ExploreTests(Assembly assembly, ITestFilter filter = null);

        /// <summary>
        ///     Explore the test cases using a filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The <see cref="ITest" /> for loaded assemblies with test cases that matches the filter.</returns>
        Task<ITest> ExploreTestsAsync(ITestFilter filter = null);

        /// <summary>
        ///     Explore the test cases using a filter.
        /// </summary>
        /// <param name="assembly">The assembly to explore test case in.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The <see cref="ITest" /> for the given assembly with test cases that matches the filter.</returns>
        /// <exception cref="ArgumentNullException"><see cref="assembly" /> is <c>null</c>.</exception>
        Task<ITest> ExploreTestsAsync(Assembly assembly, ITestFilter filter = null);

        /// <summary>
        ///     Run selected tests and return a test result. The test is run synchronously, and the listener interface is notified
        ///     as it progresses.
        /// </summary>
        /// <param name="listener">Interface to receive ITestListener notifications.</param>
        /// <param name="filter">A test filter used to select tests to be run.</param>
        /// <returns>The list of tests that were ran.</returns>
        ITestResult RunTests(ITestListener listener, ITestFilter filter = null);

        /// <summary>
        ///     Run selected tests and return a test result. The test is run synchronously, and the listener interface is notified
        ///     as it progresses.
        /// </summary>
        /// <param name="assembly">The assembly to run test cases from.</param>
        /// <param name="listener">Interface to receive ITestListener notifications.</param>
        /// <param name="filter">A test filter used to select tests to be run.</param>
        /// <returns>The tests that were ran.</returns>
        ITestResult RunTests(Assembly assembly, ITestListener listener, ITestFilter filter = null);

        /// <summary>
        ///     Run selected tests asynchronously, notifying the listener interface as it progresses.
        /// </summary>
        /// <param name="listener">Interface to receive EventListener notifications.</param>
        /// <param name="filter">A test filter used to select tests to be run.</param>
        /// <returns>The list of tests that were ran.</returns>
        Task<ITestResult> RunTestsAsync(ITestListener listener, ITestFilter filter = null);

        /// <summary>
        ///     Run selected tests asynchronously, notifying the listener interface as it progresses.
        /// </summary>
        /// <param name="assembly">The assembly to run test cases from.</param>
        /// <param name="listener">Interface to receive EventListener notifications.</param>
        /// <param name="filter">A test filter used to select tests to be run.</param>
        /// <returns>The tests that were ran.</returns>
        Task<ITestResult> RunTestsAsync(Assembly assembly, ITestListener listener,
            ITestFilter filter = null);

        /// <summary>
        ///     Gets the latest test results.
        /// </summary>
        /// <returns>The latest test results.</returns>
        ITestResult GetTestResults();

        /// <summary>
        ///     Gets the latest test results.
        /// </summary>
        /// <param name="assembly">The assembly to get the test results from.</param>
        /// <returns>The latest test results.</returns>
        ITestResult GetTestResults(Assembly assembly);

        /// <summary>
        ///     Gets the latest test results.
        /// </summary>
        /// <returns>The latest test results.</returns>
        Task<ITestResult> GetTestResultsAsync();

        /// <summary>
        ///     Gets the latest test results.
        /// </summary>
        /// <param name="assembly">The assembly to get the test results from.</param>
        /// <returns>The latest test results.</returns>
        Task<ITestResult> GetTestResultsAsync(Assembly assembly);

        #endregion
    }
}