using System;
using System.ComponentModel;
using Xamarin.Forms;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Services;
using XamarinNUnitRunner.ViewModels;

namespace XamarinNUnitRunner.Views
{
    [DesignTimeVisible(false)]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class TestsPage : ContentPage
    {
        #region Private Members

        /// <summary>
        ///     Holds the test runner to load the tests from.
        /// </summary>
        private readonly INUnitRunner v_TestRunner;

        /// <summary>
        ///     Holds if the <see cref="InitializeComponent" /> should be called.
        /// </summary>
        private readonly bool v_InitializeComponent;

        #endregion

        #region Public Members

        /// <summary>
        ///     Gets the view model associated with this page.
        /// </summary>
        public TestsViewModel ViewModel { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="TestsPage" />.
        /// </summary>
        /// <param name="runner">The NUnit test runner to associate with this master and detail pages.</param>
        /// <param name="test">The NUnit test to associate with this item page.</param>
        /// <param name="initializeComponent">If the <see cref="InitializeComponent" /> should be called.</param>
        internal TestsPage(INUnitRunner runner, NUnitTest test, bool initializeComponent)
        {
            v_InitializeComponent = initializeComponent;
            v_TestRunner = runner;

            // Set view model to default (top-level) if test is null or as a sub-top test
            ViewModel = test == null ? new TestsViewModel(runner) : new TestsViewModel(runner, test);

            if (initializeComponent)
            {
                InitializeComponent();

                BindingContext = ViewModel;
            }
        }

        /// <summary>
        ///     Initializes a new <see cref="TestsPage" />.
        /// </summary>
        /// <param name="runner">The NUnit test runner to associate with this master and detail pages.</param>
        /// <param name="test">The NUnit test to associate with this item page.</param>
        public TestsPage(INUnitRunner runner, NUnitTest test = null) : this(runner, test, true)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Handle the page rendering in the foreground.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Load items if none are loaded
            if (ViewModel.Tests.Count == 0)
            {
                ViewModel.LoadTestsCommand.Execute(null);
            }
        }

        /// <summary>
        ///     Handle a test item being selected.
        /// </summary>
        /// <param name="sender">The control that sent the event.</param>
        /// <param name="args">The event arguments.</param>
        protected void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            // Do nothing if item is null or not of expected type
            if (!(args?.SelectedItem is NUnitTest test) || test.Test == null)
            {
                return;
            }

            // Navigate to appropriate page depending on if test has child tests
            if (test.Test.HasChildren)
            {
                NavigatePushAsync(new TestsPage(v_TestRunner, test, v_InitializeComponent));
            }
            else
            {
                NavigatePushAsync(new TestDetailPage(v_TestRunner, test, v_InitializeComponent));
            }

            // Manually deselect item.
            if (sender is ListView view)
            {
                view.SelectedItem = null;
            }
        }

        /// <summary>
        ///     Handle the run tests button being clicked.
        /// </summary>
        /// <param name="sender">The control that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        protected void RunTestsClicked(object sender, EventArgs e)
        {
            ViewModel.RunTestsCommand.Execute(null);
        }

        /// <summary>
        ///     Push the given <see cref="Page" /> onto the navigation stack.
        /// </summary>
        /// <remarks>This method will not be covered by unit tests as it invokes the Xamarin.Forms context.</remarks>
        /// <param name="page">The page to push onto the stack.</param>
        /// <returns>A task to await.</returns>
        protected virtual async void NavigatePushAsync(Page page)
        {
            await Navigation.PushAsync(page).ConfigureAwait(false);
        }

        #endregion
    }
}