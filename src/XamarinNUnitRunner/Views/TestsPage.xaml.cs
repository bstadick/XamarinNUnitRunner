using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
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

        /// <summary>
        ///     Holds a cache of child test pages.
        /// </summary>
        private readonly Dictionary<string, Page> v_TestPages = new Dictionary<string, Page>();

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
        internal TestsPage(INUnitRunner runner, TestsViewModel test, bool initializeComponent)
        {
            v_InitializeComponent = initializeComponent;
            v_TestRunner = runner;

            // Set view model to default (top-level) if test is null or as a sub-top test
            ViewModel = test ?? new TestsViewModel(runner);

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
        public TestsPage(INUnitRunner runner, TestsViewModel test = null) : this(runner, test, true)
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

            // Load items if none are loaded, otherwise just reload results
            if (ViewModel.Tests.Count == 0)
            {
                ViewModel.LoadTestsCommand.Execute(null);
            }
            else
            {
                ViewModel.ReloadResultsCommand.Execute(null);
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
            if (!(args?.SelectedItem is TestsViewModel test) || test.Test == null || string.IsNullOrEmpty(test.Test.Id))
            {
                DeselectListViewItem(sender);
                return;
            }

            // Do not navigate if no children and is a suite
            if (!test.Test.HasChildren && test.Test.IsSuite)
            {
                DeselectListViewItem(sender);
                return;
            }

            // Navigate to appropriate page depending on if it is already been cached and if the test has child tests
            Page page;
            string id = test.Test.Id;
            if (v_TestPages.ContainsKey(id))
            {
                page = v_TestPages[id];
            }
            else
            {
                if (test.Test.HasChildren)
                {
                    page = new TestsPage(v_TestRunner, test, v_InitializeComponent);
                }
                else
                {
                    page = new TestDetailPage(test, v_InitializeComponent);
                }

                v_TestPages.Add(test.Test.Id, page);
            }

            NavigatePushAsync(page);

            // Manually deselect item.
            DeselectListViewItem(sender);
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

        #region Private Methods

        /// <summary>
        ///     Deselects currently selected item if sender is a <see cref="ListView" />.
        /// </summary>
        /// <param name="sender">The sender to deselect.</param>
        private static void DeselectListViewItem(object sender)
        {
            if (sender is ListView view)
            {
                view.SelectedItem = null;
            }
        }

        #endregion
    }
}