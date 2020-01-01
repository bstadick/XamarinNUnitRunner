using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Resources;
using XamarinNUnitRunner.Services;

namespace XamarinNUnitRunner.Views
{
    [DesignTimeVisible(false)]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainPage : MasterDetailPage
    {
        #region Private Members

        /// <summary>
        ///     Holds a cache of pages for menu navigation.
        /// </summary>
        private readonly Dictionary<MenuItemType, NavigationPage> v_MenuPages =
            new Dictionary<MenuItemType, NavigationPage>();

        /// <summary>
        ///     Holds the test runner to load the tests from.
        /// </summary>
        private readonly INUnitRunner v_TestRunner;

        /// <summary>
        ///     Holds if the <see cref="InitializeComponent" /> should be called.
        /// </summary>
        private readonly bool v_InitializeComponent;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="MainPage" />.
        /// </summary>
        /// <param name="runner">The NUnit test runner to associate with this master and detail pages.</param>
        /// <param name="initializeComponent">If the <see cref="InitializeComponent" /> should be called.</param>
        internal MainPage(INUnitRunner runner, bool initializeComponent)
        {
            v_InitializeComponent = initializeComponent;

            // Under normal operation the InitializeComponent should always be called
            // But for unit testing InitializeComponent should not be called as it invokes the Xamarin.Forms context.
            if (initializeComponent)
            {
                InitializeComponent();

                // Set tab feed icon on iOS
                if (string.Equals(Device.RuntimePlatform, Device.iOS, StringComparison.Ordinal))
                {
                    IconImageSource = ImageSource.FromResource(Resource.TabFeedImagePath, GetType().Assembly);
                }

                MasterBehavior = MasterBehavior.Popover;
            }

            v_TestRunner = runner;
            Detail = new NavigationPage(new TestsPage(runner, null, initializeComponent));
            Master = new MenuPage(this, initializeComponent);

            // Set Detail (current test page) in menu navigation cache
            v_MenuPages.Add(MenuItemType.Tests, (NavigationPage) Detail);
        }

        /// <summary>
        ///     Initializes a new <see cref="MainPage" />.
        /// </summary>
        /// <param name="runner">The NUnit test runner to associate with this master and detail pages.</param>
        public MainPage(INUnitRunner runner) : this(runner, true)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Navigate to the page from the menu.
        /// </summary>
        /// <param name="type">The type of the page in the menu selection</param>
        /// <returns>A task to await.</returns>
        public async Task NavigateFromMenu(MenuItemType type)
        {
            // Cache page if not already cached
            if (!v_MenuPages.ContainsKey(type))
            {
                switch (type)
                {
                    case MenuItemType.Tests:
                        v_MenuPages.Add(type,
                            new NavigationPage(new TestsPage(v_TestRunner, null, v_InitializeComponent)));
                        break;
                    case MenuItemType.About:
                        v_MenuPages.Add(type, new NavigationPage(new AboutPage(v_InitializeComponent)));
                        break;
                    default:
                        return;
                }
            }

            NavigationPage newPage = v_MenuPages[type];

            // Navigate to new menu selected page if available and not the current page
            if (newPage != null && newPage != Detail)
            {
                Detail = newPage;

                // Delay on Android for unknown reason (was part of sample)
                if (IsDevicePlatform(Device.Android))
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }

                SetIsPresented(false);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets if the given device platform string matches the current device runtime platform.
        /// </summary>
        /// <remarks>This method will not be covered by unit tests as it invokes the Xamarin.Forms context.</remarks>
        /// <param name="platform">The device platform string to check for.</param>
        /// <returns><c>true</c> if the given device platform string matches the current device platform, otherwise <c>false</c>.</returns>
        protected virtual bool IsDevicePlatform(string platform)
        {
            return string.Equals(Device.RuntimePlatform, platform, StringComparison.Ordinal);
        }

        /// <summary>
        ///     Sets <see cref="MasterDetailPage.IsPresented" />.
        /// </summary>
        /// <remarks>This method will not be covered by unit tests as it invokes the Xamarin.Forms context.</remarks>
        /// <param name="presented">If the visual element is presented.</param>
        protected virtual void SetIsPresented(bool presented)
        {
            IsPresented = presented;
        }

        #endregion
    }
}