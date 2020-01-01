using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinNUnitRunner.Resources;

namespace XamarinNUnitRunner.ViewModels
{
    /// <summary>
    ///     View model for the about view.
    /// </summary>
    public class AboutViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        ///     Url to the Xamarin website.
        /// </summary>
        private const string c_XamarinUri = "https://xamarin.com/platform";

        /// <summary>
        ///     Url to the NUnit website.
        /// </summary>
        private const string c_NUnitUri = "https://nunit.org/";

        /// <summary>
        ///     Url to this project's website.
        /// </summary>
        private const string c_ProjectUri = "https://github.com/bstadick/XamarinNUnitRunner";

        #endregion

        #region Public Members

        /// <summary>
        ///     Opens the Xamarin website using the default browser.
        /// </summary>
        public ICommand OpenWebXamarinCommand { get; }

        /// <summary>
        ///     Opens the NUnit website using the default browser.
        /// </summary>
        public ICommand OpenWebNUnitCommand { get; }

        /// <summary>
        ///     Opens the XamarinNUnitRunner project website using the default browser.
        /// </summary>
        public ICommand OpenWebProjectCommand { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="AboutViewModel" />.
        /// </summary>
        public AboutViewModel()
        {
            Title = Resource.AboutPageTitle;

            OpenWebXamarinCommand = new Command(async () => await OpenUri(new Uri(c_XamarinUri)).ConfigureAwait(false));
            OpenWebNUnitCommand = new Command(async () => await OpenUri(new Uri(c_NUnitUri)).ConfigureAwait(false));
            OpenWebProjectCommand = new Command(async () => await OpenUri(new Uri(c_ProjectUri)).ConfigureAwait(false));
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Opens the provided Uri using the default web browser.
        /// </summary>
        /// <remarks>This method will not be covered by unit tests as it invokes opening an external application.</remarks>
        /// <param name="uri">The Uri to open.</param>
        /// <returns>A task to await.</returns>
        protected virtual async Task OpenUri(Uri uri)
        {
            await Launcher.OpenAsync(uri).ConfigureAwait(false);
        }

        #endregion
    }
}