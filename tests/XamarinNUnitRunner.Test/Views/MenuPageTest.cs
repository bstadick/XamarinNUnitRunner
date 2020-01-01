using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.Forms;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Views;

namespace XamarinNUnitRunner.Test.Views
{
    [TestFixture]
    public class MenuPageTest
    {
        #region Tests for Constructor

        [Test]
        public void TestConstructorWithMainPage([Values] bool isMainPageNull)
        {
            IList<HomeMenuItem> menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Tests, Title = "Tests"},
                new HomeMenuItem {Id = MenuItemType.About, Title = "About"}
            };

            MainPage mainPage = isMainPageNull ? null : new MainPage(null, false);

            MenuPage page = new MenuPage(mainPage, false);

            Assert.AreEqual("Menu", page.Title);
            Assert.AreEqual(mainPage, page.RootPage);
            CollectionAssert.AreEquivalent(menuItems, page.MenuItems);
        }

        #endregion

        #region Tests for ListViewMenuOnItemSelected

        [Test]
        public void TestListViewMenuOnItemSelectedWithArgsNullReturnsImmediately([Values] bool isArgNull)
        {
            SelectedItemChangedEventArgs arg =
                isArgNull ? null : new SelectedItemChangedEventArgs(null, 0);

            MainPage mainPage = new MainPage(null, false);
            Page currentDetail = mainPage.Detail;
            Page navPage = (currentDetail as NavigationPage)?.RootPage;
            Assert.IsNotNull(navPage);
            Assert.IsTrue(navPage is TestsPage);

            MenuPageForTest page = new MenuPageForTest(mainPage);

            Task task = page.InvokeListViewMenuOnItemSelected(this, arg);
            Assert.IsNotNull(task);
            task.Wait();

            Assert.AreSame(currentDetail, mainPage.Detail);
        }

        [Test]
        public void TestListViewMenuOnItemSelectedWithArgsNotHomeMenuItemReturnsImmediately()
        {
            SelectedItemChangedEventArgs arg = new SelectedItemChangedEventArgs("Title", 0);

            MainPage mainPage = new MainPage(null, false);
            Page currentDetail = mainPage.Detail;
            Page navPage = (currentDetail as NavigationPage)?.RootPage;
            Assert.IsNotNull(navPage);
            Assert.IsTrue(navPage is TestsPage);

            MenuPageForTest page = new MenuPageForTest(mainPage);

            Task task = page.InvokeListViewMenuOnItemSelected(this, arg);
            Assert.IsNotNull(task);
            task.Wait();

            Assert.AreSame(currentDetail, mainPage.Detail);
        }

        [Test]
        public void TestListViewMenuOnItemSelectedWithRootPageNullReturnsImmediately()
        {
            SelectedItemChangedEventArgs arg =
                new SelectedItemChangedEventArgs(new HomeMenuItem {Id = MenuItemType.About, Title = "Title"}, 0);

            MenuPageForTest page = new MenuPageForTest(null);

            Task task = page.InvokeListViewMenuOnItemSelected(this, arg);
            Assert.IsNotNull(task);
            task.Wait();
        }

        [Test]
        public void TestListViewMenuOnItemSelectedNavigatesToNewPage([Values] MenuItemType start,
            [Values] MenuItemType end)
        {
            SelectedItemChangedEventArgs arg =
                new SelectedItemChangedEventArgs(new HomeMenuItem {Id = end, Title = "Title"}, 0);

            MainPage mainPage = new MainPageForTest(null);
            Page startPage = null;
            Type expected = null;
            switch (start)
            {
                case MenuItemType.Tests:
                    startPage = new TestsPage(null, null, false);
                    expected = typeof(TestsPage);
                    break;
                case MenuItemType.About:
                    startPage = new AboutPage(false);
                    expected = typeof(AboutPage);
                    break;
                default:
                    Assert.Fail($" MenuItemType {start} is not supported by this test.");
                    break;
            }

            switch (end)
            {
                case MenuItemType.Tests:
                    expected = typeof(TestsPage);
                    break;
                case MenuItemType.About:
                    expected = typeof(AboutPage);
                    break;
                default:
                    Assert.Fail($" MenuItemType {end} is not supported by this test.");
                    break;
            }

            mainPage.Detail = new NavigationPage(startPage);

            MenuPageForTest page = new MenuPageForTest(mainPage);

            Task task = page.InvokeListViewMenuOnItemSelected(this, arg);
            Assert.IsNotNull(task);
            task.Wait();

            Page navPage = (mainPage.Detail as NavigationPage)?.RootPage;
            Assert.IsNotNull(navPage);
            Assert.AreEqual(expected, navPage.GetType());
        }

        #endregion

        #region Nested Class: MenuPageForTest

        /// <summary>
        ///     Implements MenuPage for test.
        /// </summary>
        private class MenuPageForTest : MenuPage
        {
            #region Constructor

            /// <inheritdoc />
            public MenuPageForTest(MainPage mainPage) : base(mainPage, false)
            {
            }

            #endregion

            #region Public Methods

            /// <inheritdoc cref="MenuPage.ListViewMenuOnItemSelected" />
            public async Task InvokeListViewMenuOnItemSelected(object sender, SelectedItemChangedEventArgs e)
            {
                await ListViewMenuOnItemSelected(sender, e);
            }

            #endregion
        }

        #endregion
    }
}