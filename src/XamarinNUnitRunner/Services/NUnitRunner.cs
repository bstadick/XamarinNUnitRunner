using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using XamarinNUnitRunner.Filter;
using XamarinNUnitRunner.Models;

namespace XamarinNUnitRunner.Services
{
    /// <summary>
    ///     NUnitRunner manages and runs NUnit test assemblies.
    /// </summary>
    public class NUnitRunner : INUnitRunner
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NUnitRunner" /> class.
        /// </summary>
        /// <param name="suite">The name of the suite.</param>
        /// <exception cref="ArgumentNullException"><see cref="suite" /> is <c>null</c>.</exception>
        protected NUnitRunner(NUnitSuite suite)
        {
            TestSuite = suite ?? throw ExceptionHelper.ThrowArgumentNullException(nameof(suite));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NUnitRunner" /> class.
        /// </summary>
        /// <param name="name">The name of the suite.</param>
        /// <exception cref="ArgumentException"><see cref="name" /> is <c>null</c> or empty.</exception>
        public NUnitRunner(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw ExceptionHelper.ThrowArgumentExceptionForNullOrEmpty(nameof(name));
            }

            TestSuite = new NUnitSuite(name);
        }

        #endregion

        #region Implementation of INUnitRunner

        /// <inheritdoc />
        public NUnitSuite TestSuite { get; }

        /// <inheritdoc />
        public bool IsTestRunning => TestSuite.IsTestRunning;

        /// <inheritdoc />
        public bool IsTestComplete => TestSuite.IsTestComplete;

        /// <inheritdoc />
        public ITestListener TestListener { get; set; }

        /// <inheritdoc />
        public ITest AddTestAssembly(Assembly assembly, IDictionary<string, object> settings = null)
        {
            if (assembly == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(assembly));
            }

            lock (TestSuite)
            {
                return TestSuite.Add(assembly, settings);
            }
        }

        /// <inheritdoc />
        public async Task<ITest> AddTestAssemblyAsync(Assembly assembly, IDictionary<string, object> settings = null)
        {
            if (assembly == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(assembly));
            }

            Task<ITest> task = Task.Run(() =>
            {
                lock (TestSuite)
                {
                    return TestSuite.Add(assembly, settings);
                }
            });

            return await task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public int CountTestCases(ITestFilter filter = null)
        {
            filter = ValidateFilter(filter);

            lock (TestSuite)
            {
                return TestSuite.TestAssemblyRunners.Sum(test => test.CountTestCases(filter));
            }
        }

        /// <inheritdoc />
        public int CountTestCases(Assembly assembly, ITestFilter filter = null)
        {
            if (assembly == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(assembly));
            }

            filter = ValidateFilter(filter);

            lock (TestSuite)
            {
                return TestSuite.GetTestAssemblyRunner(assembly)?.CountTestCases(filter) ?? 0;
            }
        }

        /// <inheritdoc />
        public async Task<int> CountTestCasesAsync(ITestFilter filter = null)
        {
            filter = ValidateFilter(filter);

            Task<int> task = Task.Run(() =>
            {
                lock (TestSuite)
                {
                    return TestSuite.TestAssemblyRunners.Sum(test => test.CountTestCases(filter));
                }
            });

            return await task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<int> CountTestCasesAsync(Assembly assembly, ITestFilter filter = null)
        {
            if (assembly == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(assembly));
            }

            filter = ValidateFilter(filter);

            Task<int> task = Task.Run(() =>
            {
                lock (TestSuite)
                {
                    return TestSuite.GetTestAssemblyRunner(assembly)?.CountTestCases(filter) ?? 0;
                }
            });

            return await task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public ITest ExploreTests(ITestFilter filter = null)
        {
            filter = ValidateFilter(filter);

            lock (TestSuite)
            {
                return TestSuite.ExploreTests(filter);
            }
        }

        /// <inheritdoc />
        public ITest ExploreTests(Assembly assembly, ITestFilter filter = null)
        {
            if (assembly == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(assembly));
            }

            filter = ValidateFilter(filter);

            lock (TestSuite)
            {
                return TestSuite.GetTestAssemblyRunner(assembly)?.ExploreTests(filter);
            }
        }

        /// <inheritdoc />
        public async Task<ITest> ExploreTestsAsync(ITestFilter filter = null)
        {
            filter = ValidateFilter(filter);

            Task<ITest> task = Task.Run(() =>
            {
                lock (TestSuite)
                {
                    return TestSuite.ExploreTests(filter);
                }
            });

            return await task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ITest> ExploreTestsAsync(Assembly assembly, ITestFilter filter = null)
        {
            if (assembly == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(assembly));
            }

            filter = ValidateFilter(filter);

            Task<ITest> task = Task.Run(() =>
            {
                lock (TestSuite)
                {
                    return TestSuite.GetTestAssemblyRunner(assembly)?.ExploreTests(filter);
                }
            });

            return await task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public ITestResult RunTests(ITestListener listener, ITestFilter filter = null)
        {
            IList<ITestAssemblyRunner> assemblies;
            TestSuiteResult results;
            lock (TestSuite)
            {
                assemblies = new List<ITestAssemblyRunner>(TestSuite.TestAssemblyRunners);
                results = new TestSuiteResult(TestSuite);
            }

            RunTests(assemblies, listener, filter, false, ref results);
            return results;
        }

        /// <inheritdoc />
        public ITestResult RunTests(Assembly assembly, ITestListener listener, ITestFilter filter = null)
        {
            if (assembly == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(assembly));
            }

            ITestAssemblyRunner runner;
            TestSuiteResult results;
            lock (TestSuite)
            {
                runner = TestSuite.GetTestAssemblyRunner(assembly);
                results = new TestSuiteResult(TestSuite);
            }

            if (runner == null)
            {
                return null;
            }

            IList<ITestAssemblyRunner> assemblies = new List<ITestAssemblyRunner> {runner};

            RunTests(assemblies, listener, filter, false, ref results);
            return results;
        }

        /// <inheritdoc />
        public async Task<ITestResult> RunTestsAsync(ITestListener listener, ITestFilter filter = null)
        {
            IList<ITestAssemblyRunner> assemblies;
            TestSuiteResult results;
            lock (TestSuite)
            {
                assemblies = new List<ITestAssemblyRunner>(TestSuite.TestAssemblyRunners);
                results = new TestSuiteResult(TestSuite);
            }

            Task<ITestResult> task = Task.Run(() =>
            {
                RunTests(assemblies, listener, filter, true, ref results);
                return (ITestResult) results;
            });

            return await task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ITestResult> RunTestsAsync(Assembly assembly, ITestListener listener,
            ITestFilter filter = null)
        {
            if (assembly == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(assembly));
            }

            ITestAssemblyRunner runner;
            TestSuiteResult results;
            lock (TestSuite)
            {
                runner = TestSuite.GetTestAssemblyRunner(assembly);
                results = new TestSuiteResult(TestSuite);
            }

            if (runner == null)
            {
                return await Task.FromResult((ITestResult) null).ConfigureAwait(false);
            }

            IList<ITestAssemblyRunner> assemblies = new List<ITestAssemblyRunner> {runner};

            Task<ITestResult> task = Task.Run(() =>
            {
                RunTests(assemblies, listener, filter, true, ref results);
                return (ITestResult) results;
            });
            return await task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public ITestResult GetTestResults()
        {
            lock (TestSuite)
            {
                TestSuiteResult results = new TestSuiteResult(TestSuite);
                foreach (ITestAssemblyRunner runner in TestSuite.TestAssemblyRunners)
                {
                    if (runner.Result != null)
                    {
                        results.AddResult(runner.Result);
                    }
                }

                return results;
            }
        }

        /// <inheritdoc />
        public ITestResult GetTestResults(Assembly assembly)
        {
            if (assembly == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(assembly));
            }

            lock (TestSuite)
            {
                TestSuiteResult results = new TestSuiteResult(TestSuite);
                ITestAssemblyRunner runner = TestSuite.GetTestAssemblyRunner(assembly);
                if (runner?.Result != null)
                {
                    results.AddResult(runner.Result);
                }

                return results;
            }
        }

        /// <inheritdoc />
        public async Task<ITestResult> GetTestResultsAsync()
        {
            Task<ITestResult> task = Task.Run(() =>
            {
                lock (TestSuite)
                {
                    TestSuiteResult results = new TestSuiteResult(TestSuite);
                    foreach (ITestAssemblyRunner runner in TestSuite.TestAssemblyRunners)
                    {
                        if (runner.Result != null)
                        {
                            results.AddResult(runner.Result);
                        }
                    }

                    return (ITestResult) results;
                }
            });

            return await task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ITestResult> GetTestResultsAsync(Assembly assembly)
        {
            if (assembly == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(assembly));
            }

            Task<ITestResult> task = Task.Run(() =>
            {
                lock (TestSuite)
                {
                    TestSuiteResult results = new TestSuiteResult(TestSuite);
                    ITestAssemblyRunner runner = TestSuite.GetTestAssemblyRunner(assembly);
                    if (runner?.Result != null)
                    {
                        results.AddResult(runner.Result);
                    }

                    return (ITestResult) results;
                }
            });

            return await task.ConfigureAwait(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Validates the filter.
        /// </summary>
        /// <param name="filter">The filter to validate.</param>
        /// <returns>The filter, or an empty filter if null.</returns>
        private static ITestFilter ValidateFilter(ITestFilter filter)
        {
            return filter ?? NUnitFilter.Empty;
        }

        /// <summary>
        ///     Run selected tests synchronously or asynchronously, notifying the listener interface as it progresses.
        /// </summary>
        /// <param name="assemblies">The list of test assembly runners to run.</param>
        /// <param name="listener">Interface to receive EventListener notifications.</param>
        /// <param name="filter">A test filter used to select tests to be run.</param>
        /// <param name="runAsync"><c>true</c> to run tests asynchronously, otherwise <c>false</c> to run synchronously.</param>
        /// <param name="results">The <see cref="ITestResult" /> object to add the test results to.</param>
        private static void RunTests(IList<ITestAssemblyRunner> assemblies,
            ITestListener listener, ITestFilter filter, bool runAsync, ref TestSuiteResult results)
        {
            filter = ValidateFilter(filter);
            Queue<ITestAssemblyRunner> testQueue = new Queue<ITestAssemblyRunner>(assemblies);

            // Check each test not ran when first encountered to see if it is ready to be run
            while (testQueue.Count > 0)
            {
                // Run test if not currently running, otherwise queue up to be checked again later
                ITestAssemblyRunner test = testQueue.Dequeue();
                if (!test.IsTestRunning)
                {
                    if (runAsync)
                    {
                        test.RunAsync(listener, filter);
                    }
                    else
                    {
                        test.Run(listener, filter);
                    }
                }
                else
                {
                    // Test not ready to run so re-enqueue
                    testQueue.Enqueue(test);
                }

                // Slow down the polling loop to give time for tests to complete
                if (runAsync)
                {
                    Thread.Sleep(10);
                }
            }

            // Wait for tests to complete
            while (assemblies.Any(test => test.IsTestRunning || !test.IsTestComplete))
                // Slow down the polling loop to give time for tests to complete
            {
                Thread.Sleep(10);
            }

            // Add individual test runner results to overall result
            foreach (ITestAssemblyRunner test in assemblies)
            {
                if (test.Result != null)
                {
                    results.AddResult(test.Result);
                }
            }
        }

        #endregion
    }
}