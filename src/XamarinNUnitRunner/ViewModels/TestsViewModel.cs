using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using Xamarin.Forms;
using XamarinNUnitRunner.Filter;
using XamarinNUnitRunner.Models;
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
        private ITest v_Test;

        /// <summary>
        ///     Holds the backing store for the <see cref="Result" /> property.
        /// </summary>
        private INUnitTestResult v_Result;

        #endregion

        #region Public Members

        /// <summary>
        ///     Gets or sets the child <see cref="TestsViewModel" /> of <see cref="Test" />.
        /// </summary>
        public ObservableCollection<TestsViewModel> Tests { get; } = new ObservableCollection<TestsViewModel>();

        /// <summary>
        ///     Loads the child tests of <see cref="Test" />.
        /// </summary>
        public Command LoadTestsCommand { get; }

        /// <summary>
        ///     Runs and loads the child test results of <see cref="Result" />.
        /// </summary>
        public Command RunTestsCommand { get; }

        /// <summary>
        ///     Loads the child test results of <see cref="Result" />.
        /// </summary>
        public Command ReloadResultsCommand { get; }

        /// <summary>
        ///     Gets the test runner that stores the tests to run and their results.
        /// </summary>
        public INUnitRunner TestRunner { get; }

        /// <summary>
        ///     Gets or sets the test of the page.
        /// </summary>
        public ITest Test
        {
            get => v_Test;
            set
            {
                if (SetProperty(ref v_Test, value))
                {
                    // Update Title when test is updated
                    Title = FullDisplayName;

                    // Notify of the update of other Test dependent properties
                    InvokePropertyChanged(nameof(DisplayName));
                    InvokePropertyChanged(nameof(FullDisplayName));
                    InvokePropertyChanged(nameof(ConditionalDisplayName));
                    InvokePropertyChanged(nameof(TestFilter));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the test results of the page.
        /// </summary>
        public INUnitTestResult Result
        {
            get => v_Result;
            set
            {
                if (SetProperty(ref v_Result, value))
                {
                    // Notify of the update of other Result dependent properties
                    InvokePropertyChanged(nameof(HasResult));
                }
            }
        }

        /// <summary>
        ///     Gets if the test has a result.
        /// </summary>
        public bool HasResult => v_Result?.Result != null;

        /// <summary>
        ///     Gets the display name for the test.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (Test?.Name == null)
                {
                    return string.Empty;
                }

                return Test.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                    ? Path.GetFileNameWithoutExtension(Test.Name)
                    : Test.Name;
            }
        }

        /// <summary>
        ///     Gets the full display name for the test.
        /// </summary>
        public string FullDisplayName
        {
            get
            {
                if (Test?.FullName == null)
                {
                    return string.Empty;
                }

                return Test.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                    ? Path.GetFileNameWithoutExtension(Test.FullName)
                    : Test.FullName;
            }
        }

        /// <summary>
        ///     Gets the <see cref="FullDisplayName" /> for the test if it is a suite and not a class nor a method, otherwise gets
        ///     the <see cref="DisplayName" />.
        /// </summary>
        public string ConditionalDisplayName
        {
            get
            {
                if (Test?.Name == null)
                {
                    return string.Empty;
                }

                return Test.IsSuite && Test.ClassName == null && Test.Method == null ? FullDisplayName : DisplayName;
            }
        }

        /// <summary>
        ///     Gets the test filter to select tests under this test.
        /// </summary>
        public ITestFilter TestFilter =>
            Test == null ? NUnitFilter.Empty : NUnitFilter.Where.Id(Test.Id).Build().Filter;

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
            ReloadResultsCommand = new Command(async () => await ExecuteReloadResultsCommand().ConfigureAwait(false));
        }

        /// <summary>
        ///     Initializes a new <see cref="TestsViewModel" />.
        /// </summary>
        /// <param name="runner">The NUnit test runner to use with the view model.</param>
        /// <param name="test">The test to associate with the view model.</param>
        /// <param name="result">The result for the test.</param>
        public TestsViewModel(INUnitRunner runner, ITest test, ITestResult result = null) : this(runner)
        {
            Test = test;
            Result = new NUnitTestResult(result);
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

                if (Test == null || !Test.HasChildren)
                {
                    return Task.CompletedTask;
                }

                // Create TestsViewModels from ITest children and add to the Tests collection
                foreach (TestsViewModel test in Test.Tests.Where(test => test != null)
                    .Select(test => new TestsViewModel(TestRunner, test)))
                {
                    // Flatten test suites that have only one child test and is not a class or method
                    // This will flatten each namespace into just one page of classes
                    // Leaving class pages with their methods and method pages if they have test cases
                    TestsViewModel testToAdd = test;
                    while (testToAdd.Test != null && testToAdd.Test.HasChildren && testToAdd.Test.Tests.Count == 1 &&
                           testToAdd.Test.IsSuite && testToAdd.Test.ClassName == null && testToAdd.Test.Method == null)
                    {
                        testToAdd = new TestsViewModel(TestRunner, testToAdd.Test.Tests.First());
                    }

                    Tests.Add(testToAdd);
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
            ITestFilter filter = args as ITestFilter ?? TestFilter;

            async Task RunCommand()
            {
                // Run selected tests
                ITestResult result =
                    await TestRunner.RunTestsAsync(TestRunner.TestListener, filter).ConfigureAwait(false);
                Result = new NUnitTestResult(result);
            }

            await ExecuteTestsCommand(RunCommand).ConfigureAwait(false);
        }

        /// <summary>
        ///     Loads the test results from the associated test.
        /// </summary>
        /// <returns>A task to await.</returns>
        private async Task ExecuteReloadResultsCommand()
        {
            await ExecuteTestsCommand(null).ConfigureAwait(false);
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
                    Test = TestRunner.TestSuite;
                }

                // Run specific command
                if (command != null)
                {
                    await command.Invoke().ConfigureAwait(false);
                }

                // Load test results
                await LoadResults(Result?.Result, Tests).ConfigureAwait(false);
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
        private static Task<Dictionary<string, TestsViewModel>> LoadResults(ITestResult result,
            IEnumerable<TestsViewModel> tests)
        {
            // Explore any present test results for existing tests
            return Task.Run(() =>
            {
                Dictionary<string, TestsViewModel> testDictionary = tests
                    .Where(child => !string.IsNullOrEmpty(child?.Test?.Id)).ToDictionary(child => child.Test?.Id);

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
        private static void ExploreResults(ITestResult result, ref Dictionary<string, TestsViewModel> tests,
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
                tests[result.Test.Id].Result = new NUnitTestResult(result);
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