using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using Xamarin.Forms;
using XamarinNUnitRunner.Filter;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Resources;
using XamarinNUnitRunner.Services;

namespace XamarinNUnitRunner.ViewModels
{
    /// <summary>
    ///     View model for test views.
    /// </summary>
    public class TestsViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        ///     Holds the backing store for the <see cref="Test" /> property.
        /// </summary>
        private NUnitTest v_Test;

        /// <summary>
        ///     Holds the backing store for the <see cref="Result" /> property.
        /// </summary>
        private ITestResult v_Result;

        #endregion

        #region Public Members

        /// <summary>
        ///     Gets or sets the child <see cref="NUnitTest" /> of <see cref="Test" />.
        /// </summary>
        public ObservableCollection<NUnitTest> Tests { get; } = new ObservableCollection<NUnitTest>();

        /// <summary>
        ///     Loads the child tests of <see cref="Test" />.
        /// </summary>
        public Command LoadTestsCommand { get; }

        /// <summary>
        ///     Runs and loads the child test results of <see cref="Result" />.
        /// </summary>
        public Command RunTestsCommand { get; }

        /// <summary>
        ///     Gets the test runner that stores the tests to run and their results.
        /// </summary>
        public INUnitRunner TestRunner { get; }

        /// <summary>
        ///     Gets or sets the test of the page.
        /// </summary>
        public NUnitTest Test
        {
            get => v_Test;
            set
            {
                SetProperty(ref v_Test, value);
                Title = value?.DisplayName ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets the test results of the page.
        /// </summary>
        public ITestResult Result
        {
            get => v_Result;
            set
            {
                SetProperty(ref v_Result, value);
                if (v_Test != null)
                {
                    v_Test.Result = value;
                }
            }
        }

        /// <summary>
        ///     Gets the result string of the page.
        /// </summary>
        public string ResultString =>
            Result == null ? Resource.TestResultNotExecuted : Result.ResultState.Status.ToString();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="TestsViewModel" />.
        /// </summary>
        /// <param name="runner">The NUnit test runner to use with the view model.</param>
        public TestsViewModel(INUnitRunner runner)
        {
            TestRunner = runner;
            Title = runner == null ? string.Empty : runner.TestSuite.FullName;
            LoadTestsCommand = new Command(async () => await ExecuteLoadTestsCommand().ConfigureAwait(false));
            RunTestsCommand = new Command(async args => await ExecuteRunTestsCommand(args).ConfigureAwait(false));
        }

        /// <summary>
        ///     Initializes a new <see cref="TestsViewModel" />.
        /// </summary>
        /// <param name="runner">The NUnit test runner to use with the view model.</param>
        /// <param name="test">The test to associate with the view model.</param>
        public TestsViewModel(INUnitRunner runner, NUnitTest test) : this(runner)
        {
            Test = test;
            Result = test?.Result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Loads the tests from the associated test.
        /// </summary>
        /// <returns>A task to await.</returns>
        private async Task ExecuteLoadTestsCommand()
        {
            Task RunCommand()
            {
                // Clear existing tests
                Tests.Clear();

                if (Test.Test == null || !Test.Test.HasChildren)
                {
                    return Task.CompletedTask;
                }

                // Create NUnitTests from ITest children and add to the Tests collection
                foreach (NUnitTest test in Test.Test.Tests.Where(test => test != null)
                    .Select(test => new NUnitTest(test)))
                {
                    Tests.Add(test);
                }

                return Task.CompletedTask;
            }

            await ExecuteTestsCommand(RunCommand).ConfigureAwait(false);
        }

        /// <summary>
        ///     Runs and loads the test results from the associated test.
        /// </summary>
        /// <param name="args">The args passed into the command, in this case is an optional ITestFilter instance.</param>
        /// <returns>A task to await.</returns>
        private async Task ExecuteRunTestsCommand(object args)
        {
            ITestFilter filter = args as ITestFilter ?? Test?.TestFilter ?? NUnitFilter.Empty;

            async Task RunCommand()
            {
                // Run selected tests
                Result = await TestRunner.RunTestsAsync(null, filter).ConfigureAwait(false);
            }

            await ExecuteTestsCommand(RunCommand).ConfigureAwait(false);
        }

        /// <summary>
        ///     Execute the command involving tests.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <returns>A task to await.</returns>
        private async Task ExecuteTestsCommand(Func<Task> command)
        {
            // Don't proceed if busy with another command or no TestRunner
            if (IsBusy || TestRunner == null)
            {
                return;
            }

            // Gain exclusive command access
            IsBusy = true;

            try
            {
                // Don't proceed if tests are already running
                if (TestRunner.IsTestRunning)
                {
                    return;
                }

                // Set test to root test suite if null
                if (Test == null)
                {
                    Test = new NUnitTest(TestRunner.TestSuite);
                }

                // Run specific command
                if (command != null)
                {
                    await command.Invoke().ConfigureAwait(false);
                }

                // Load test results
                await LoadResults(Result, Tests).ConfigureAwait(false);
            }
            finally
            {
                // Release exclusive command access
                IsBusy = false;
            }
        }

        /// <summary>
        ///     Loads the test results from the associated test.
        /// </summary>
        /// <param name="result">The test result to explore.</param>
        /// <param name="tests">The list of tests to load from.</param>
        /// <returns>A dictionary of the associated tests and their test ids.</returns>
        private static Task<Dictionary<string, NUnitTest>> LoadResults(ITestResult result, IEnumerable<NUnitTest> tests)
        {
            // Explore any present test results for existing tests
            return Task.Run(() =>
            {
                Dictionary<string, NUnitTest> testDictionary = tests
                    .Where(child => !string.IsNullOrEmpty(child?.Test?.Id)).ToDictionary(child => child.Test.Id);

                int exploreCount = 0;
                ExploreResults(result, ref testDictionary, ref exploreCount);
                return testDictionary;
            });
        }

        /// <summary>
        ///     Recursively explores the test results and updates any matching tests.
        /// </summary>
        /// <param name="result">The top level result to explore.</param>
        /// <param name="tests">The tests and their id to match to.</param>
        /// <param name="exploreCount">The number of tests that have been explored. Initialize to zero on the root call.</param>
        private static void ExploreResults(ITestResult result, ref Dictionary<string, NUnitTest> tests,
            ref int exploreCount)
        {
            // Break recursion if result is null
            if (result == null)
            {
                return;
            }

            // Update result if present in dictionary
            if (tests.ContainsKey(result.Test.Id))
            {
                tests[result.Test.Id].Result = result;
                exploreCount++;
            }

            // Break recursion if result doesn't have children or have already explored all tests present in the dictionary
            if (!result.HasChildren || exploreCount == tests.Count)
            {
                return;
            }

            // Recursively explore children tests
            foreach (ITestResult child in result.Children)
            {
                ExploreResults(child, ref tests, ref exploreCount);
            }
        }

        #endregion
    }
}