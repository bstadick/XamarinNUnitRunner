using NUnit.Framework;
using XamarinNUnitRunner.Models;

namespace XamarinNUnitRunner.Test.Models
{
    [TestFixture]
    public class HomeMenuItemTest
    {
        #region Tests for Id Property

        [Test]
        public void TestIdPropertyCanBeSet()
        {
            HomeMenuItem item = new HomeMenuItem();

            Assert.AreEqual(0, (int) item.Id);

            item.Id = MenuItemType.About;

            Assert.AreEqual(MenuItemType.About, item.Id);
        }

        #endregion

        #region Tests for Title

        [Test]
        public void TestTitlePropertyCanBeSet()
        {
            const string expected = "Hello";

            HomeMenuItem item = new HomeMenuItem();

            Assert.AreEqual(string.Empty, item.Title);

            item.Title = expected;

            Assert.AreEqual(expected, item.Title);

            item.Title = null;

            Assert.IsNull(item.Title);
        }

        #endregion

        #region Tests for Equals

        [Test]
        public void TestEqualsWhenObjectIsNullReturnsFalse()
        {
            HomeMenuItem item = new HomeMenuItem();

            Assert.IsFalse(item.Equals(null));
        }

        [Test]
        public void TestEqualsWhenObjectIsNotHomeMenuItemReturnsFalse()
        {
            HomeMenuItem item = new HomeMenuItem();

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsFalse(item.Equals("Hello"));
        }

        [Test]
        [TestCase(0, "hello", 1, "hello")]
        [TestCase(0, "hello", 0, "goodbye")]
        [TestCase(0, "", 0, "hello")]
        [TestCase(0, null, 0, "hello")]
        public void TestEqualsWhenObjectDoesNotMatchReturnsFalse(int id1, string title1, int id2, string title2)
        {
            HomeMenuItem item1 = new HomeMenuItem {Id = (MenuItemType) id1, Title = title1};
            HomeMenuItem item2 = new HomeMenuItem {Id = (MenuItemType) id2, Title = title2};

            Assert.IsFalse(item1.Equals(item2));
        }

        [Test]
        [TestCase(0, "hello")]
        [TestCase(0, "")]
        [TestCase(0, null)]
        [TestCase(-1, null)]
        public void TestEqualsWhenObjectDoMatchReturnsTrue(int id, string title)
        {
            HomeMenuItem item1 = new HomeMenuItem {Id = (MenuItemType) id, Title = title};
            HomeMenuItem item2 = new HomeMenuItem {Id = (MenuItemType) id, Title = title};

            Assert.IsTrue(item1.Equals(item2));
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(item1.Equals(item1));
        }

        #endregion
    }
}