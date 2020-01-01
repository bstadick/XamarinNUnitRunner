using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Resources;

namespace XamarinNUnitRunner.Views
{
    [DesignTimeVisible(false)]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MenuPage : ContentPage
    {
        #region Public Members

        /// <summary>
        ///     Gets the application's <see cref="MainPage" />.
        /// </summary>
        public MainPage RootPage { get; }

        /// <summary>
        ///     Gets the menu items.
        /// </summary>
        public IList<HomeMenuItem> MenuItems { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="MainPage" />.
        /// </summary>
        /// <param name="mainPage">The <see cref="MainPage" /> to associate with this page.</param>
        /// <param name="initializeComponent">If the <see cref="InitializeComponent" /> should be called.</param>
        internal MenuPage(MainPage mainPage, bool initializeComponent)
        {
            Title = Resource.MenuPageTitle;
            RootPage = mainPage;

            // Create list of menu items
            MenuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Tests, Title = Resource.TestsPageTitle},
                new HomeMenuItem {Id = MenuItemType.About, Title = Resource.AboutPageTitle}
            };

            if (initializeComponent)
            {
                InitializeComponent();

                // Set list source to that of the menu items
                ListViewMenu.ItemsSource = MenuItems;

                // Set initial item and selection changed callback
                ListViewMenu.SelectedItem = MenuItems[0];
                ListViewMenu.ItemSelected += async (sender, e) =>
                {
                    await ListViewMenuOnItemSelected(sender, e).ConfigureAwait(false);
                };
            }
        }

        /// <summary>
        ///     Initializes a new <see cref="MenuPage" />.
        /// </summary>
        public MenuPage(MainPage mainPage) : this(mainPage, true)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Handles the ListViewMenu selection changed event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The selected item changed event arguments.</param>
        /// <returns>A task to await.</returns>
        // ReSharper disable once UnusedParameter.Local
        protected async Task ListViewMenuOnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e?.SelectedItem is HomeMenuItem item))
            {
                return;
            }

            if (RootPage != null)
            {
                await RootPage.NavigateFromMenu(item.Id).ConfigureAwait(false);
            }
        }

        #endregion
    }
}