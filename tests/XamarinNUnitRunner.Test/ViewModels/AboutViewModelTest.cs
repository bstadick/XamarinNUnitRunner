using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NUnit.Framework;
using XamarinNUnitRunner.ViewModels;

namespace XamarinNUnitRunner.Test.ViewModels
{
    [TestFixture]
    public class AboutViewModelTest
    {
        // Tests for BaseViewModel covered by BaseViewModelTest test fixture

        #region Tests for Constructor

        [Test]
        public void TestConstructorSetsTitleToAboutAndInitializesCommands()
        {
            AboutViewModel model = new AboutViewModel();

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual("About", model.Title);
            Assert.IsNotNull(model.OpenWebXamarinCommand);
            Assert.IsTrue(model.OpenWebXamarinCommand.CanExecute(null));
            Assert.IsNotNull(model.OpenWebNUnitCommand);
            Assert.IsTrue(model.OpenWebNUnitCommand.CanExecute(null));
            Assert.IsNotNull(model.OpenWebProjectCommand);
            Assert.IsTrue(model.OpenWebProjectCommand.CanExecute(null));
        }

        #endregion

        #region Tests for OpenWebXamarinCommand Property

        [Test]
        public void TestOpenWebXamarinCommandPropertyReturnsCommandToOpenXamarinWebsite()
        {
            AboutViewModelForTest model = new AboutViewModelForTest();

            ICommand command = model.OpenWebXamarinCommand;

            Assert.IsNotNull(command);
            Assert.IsTrue(command.CanExecute(null));

            command.Execute(null);

            Assert.AreEqual("https://xamarin.com/platform", model.Uri.AbsoluteUri);
        }

        #endregion

        #region Tests for OpenWebNUnitCommand

        [Test]
        public void TestOpenWebNUnitCommandPropertyReturnsCommandToOpenNUnitWebsite()
        {
            AboutViewModelForTest model = new AboutViewModelForTest();

            ICommand command = model.OpenWebNUnitCommand;

            Assert.IsNotNull(command);
            Assert.IsTrue(command.CanExecute(null));

            command.Execute(null);

            Assert.AreEqual("https://nunit.org/", model.Uri.AbsoluteUri);
        }

        #endregion

        #region Tests for OpenWebProjectCommand

        [Test]
        public void TestOpenWebProjectCommandPropertyReturnsCommandToOpenProjectWebsite()
        {
            AboutViewModelForTest model = new AboutViewModelForTest();

            ICommand command = model.OpenWebProjectCommand;

            Assert.IsNotNull(command);
            Assert.IsTrue(command.CanExecute(null));

            command.Execute(null);

            Assert.AreEqual("https://github.com/bstadick/XamarinNUnitRunner", model.Uri.AbsoluteUri);
        }

        #endregion

        #region Nested Class: AboutViewModelForTest

        /// <summary>
        ///     AboutViewModel instance for test.
        /// </summary>
        private class AboutViewModelForTest : AboutViewModel
        {
            #region Public Members

            /// <summary>
            ///     Gets the Uri set by the call to an Open command.
            /// </summary>
            public Uri Uri { get; private set; }

            #endregion

            #region Protected Methods

            /// <inheritdoc />
            protected override Task OpenUri(Uri uri)
            {
                Uri = uri;
                return Task.CompletedTask;
            }

            #endregion
        }

        #endregion
    }
}