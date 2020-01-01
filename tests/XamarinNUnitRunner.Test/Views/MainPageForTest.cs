using System;
using XamarinNUnitRunner.Services;
using XamarinNUnitRunner.Views;

namespace XamarinNUnitRunner.Test.Views
{
    /// <summary>
    ///     Implements a MainPage for test.
    /// </summary>
    public class MainPageForTest : MainPage
    {
        #region Public Members

        /// <summary>
        ///     Gets or sets the current device platform string.
        /// </summary>
        public string DevicePlatform { get; set; } = "UWP";

        #endregion

        #region Constructors

        /// <inheritdoc />
        public MainPageForTest(INUnitRunner runner) : base(runner, false)
        {
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override bool IsDevicePlatform(string platform)
        {
            return string.Equals(DevicePlatform, platform, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        protected override void SetIsPresented(bool presented)
        {
        }

        #endregion
    }
}