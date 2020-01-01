using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.Forms;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Services;
using XamarinNUnitRunner.Views;

namespace XamarinNUnitRunner.Test.Views
{
    [TestFixture]
    public class MainPageTest
    {
        #region Tests for Constructor

        [Test]
        public void TestConstructorWithNUnitRunner([Values] bool isRunnerNull)
        {
            NUnitRunner runner = isRunnerNull ? null : new NUnitRunner("runner-name");

            MainPage page = new MainPage(runner, false);

            Assert.IsNotNull(page.Detail);
            Page detailPage = (page.Detail as NavigationPage)?.RootPage;
            Assert.IsNotNull(detailPage);
            TestsPage testsPage = detailPage as TestsPage;
            Assert.IsNotNull(testsPage);
            Assert.IsNotNull(page.Master);
            Assert.IsTrue(page.Master is MenuPage);
        }

        #endregion

        #region Tests for NavigateFromMenu

        [Test]
        public void TestNavigateFromMenuWithMenuItemTypeNotSupportedReturnsImmediately()
        {
            MainPage page = new MainPage(null, false);

            Page currentDetail = page.Detail;
            Page navPage = (currentDetail as NavigationPage)?.RootPage;
            Assert.IsNotNull(navPage);
            Assert.IsTrue(navPage is TestsPage);

            Task task = page.NavigateFromMenu((MenuItemType) (-1));
            Assert.IsNotNull(task);
            task.Wait();

            Assert.AreSame(currentDetail, page.Detail);
        }

        [Test]
        public void TestNavigateFromMenuNavigatesToNewPage([Values] MenuItemType start,
            [Values] MenuItemType end, [Values] bool isAndroid)
        {
            MainPageForTest page = new MainPageForTest(null);

            if (isAndroid)
            {
                page.DevicePlatform = "Android";
            }

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

            page.Detail = new NavigationPage(startPage);

            Task task = page.NavigateFromMenu(end);
            Assert.IsNotNull(task);
            task.Wait();

            Page currentPage = page.Detail;
            Page navPage = (page.Detail as NavigationPage)?.RootPage;
            Assert.IsNotNull(navPage);
            Assert.AreEqual(expected, navPage.GetType());

            task = page.NavigateFromMenu(end);
            Assert.IsNotNull(task);
            task.Wait();

            navPage = (page.Detail as NavigationPage)?.RootPage;
            Assert.IsNotNull(navPage);
            Assert.AreEqual(expected, navPage.GetType());
            Assert.AreSame(currentPage, page.Detail);
        }

        #endregion
    }
}