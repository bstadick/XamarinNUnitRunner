using XamarinNUnitRunner.Views;

namespace XamarinNUnitRunner.Models
{
    /// <summary>
    ///     The type of the menu items.
    /// </summary>
    public enum MenuItemType
    {
        /// <summary>
        ///     Menu item for <see cref="TestsPage" />.
        /// </summary>
        Tests,

        /// <summary>
        ///     Menu item for <see cref="AboutPage" />.
        /// </summary>
        About
    }

    /// <summary>
    ///     Model for a home menu item.
    /// </summary>
    public class HomeMenuItem
    {
        #region Public Members

        /// <summary>
        ///     Gets or sets the menu item id.
        /// </summary>
        public MenuItemType Id { get; set; }

        /// <summary>
        ///     Gets or sets the menu item title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        #endregion

        #region Public Methods

        /// <inheritdoc />
#pragma warning disable 659
        public override bool Equals(object obj)
#pragma warning restore 659
        {
            if (!(obj is HomeMenuItem item))
            {
                return false;
            }

            return item.Id == Id && item.Title == Title;
        }

        #endregion
    }
}