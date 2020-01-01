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
    public partial class TestDetailPage : ContentPage
    {
        #region Public Members

        /// <summary>
        ///     Gets the view model associated with this page.
        /// </summary>
        public TestsViewModel ViewModel { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="TestDetailPage" />.
        /// </summary>
        /// <param name="runner">The NUnit test runner to associate with this master and detail pages.</param>
        /// <param name="test">The NUnit test to associate with this detail pages.</param>
        /// <param name="initializeComponent">If the <see cref="InitializeComponent" /> should be called.</param>
        internal TestDetailPage(INUnitRunner runner, NUnitTest test, bool initializeComponent)
        {
            ViewModel = new TestsViewModel(runner, test);

            if (initializeComponent)
            {
                InitializeComponent();

                BindingContext = ViewModel;
            }
        }

        /// <summary>
        ///     Initializes a new <see cref="TestDetailPage" />.
        /// </summary>
        /// <param name="runner">The NUnit test runner to associate with this master and detail pages.</param>
        /// <param name="test">The NUnit test to associate with this detail pages.</param>
        public TestDetailPage(INUnitRunner runner, NUnitTest test) : this(runner, test, true)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Handle the run tests button being clicked.
        /// </summary>
        /// <param name="sender">The control that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        protected void RunTestsClicked(object sender, EventArgs e)
        {
            ViewModel.RunTestsCommand.Execute(null);
        }

        #endregion
    }
}