using System.ComponentModel;
using Xamarin.Forms;
using XamarinNUnitRunner.Resources;

namespace XamarinNUnitRunner.Views
{
    [DesignTimeVisible(false)]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class AboutPage : ContentPage
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="AboutPage" />.
        /// </summary>
        /// <param name="initializeComponent">If the <see cref="InitializeComponent" /> should be called.</param>
        internal AboutPage(bool initializeComponent)
        {
            if (initializeComponent)
            {
                InitializeComponent();

                AboutLogo.Source = ImageSource.FromResource(Resource.AboutPageLogoImagePath, GetType().Assembly);
            }
        }

        /// <summary>
        ///     Initializes a new <see cref="AboutPage" />.
        /// </summary>
        public AboutPage() : this(true)
        {
        }

        #endregion
    }
}