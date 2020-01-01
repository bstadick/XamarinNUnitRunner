using NUnit.Framework;
using XamarinNUnitRunner.Views;

namespace XamarinNUnitRunner.Test.Views
{
    [TestFixture]
    public class AboutPageTest
    {
        #region Tests for Constructors

        [Test]
        public void TestConstructor()
        {
            AboutPage page = new AboutPage(false);

            Assert.IsNotNull(page);
        }

        #endregion
    }
}