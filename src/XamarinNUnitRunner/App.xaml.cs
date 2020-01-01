using Xamarin.Forms;
using XamarinNUnitRunner.Services;
using XamarinNUnitRunner.Views;

namespace XamarinNUnitRunner
{
    // ReSharper disable once RedundantExtendsListEntry
    public partial class App : Application
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="App" /> instance.
        /// </summary>
        /// <param name="runner">The <see cref="INUnitRunner" /> to load and run test from.</param>
        public App(INUnitRunner runner)
        {
            InitializeComponent();

            MainPage = new MainPage(runner);
        }

        #endregion
    }
}