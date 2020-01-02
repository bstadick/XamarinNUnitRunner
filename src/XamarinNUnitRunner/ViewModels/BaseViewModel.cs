using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XamarinNUnitRunner.ViewModels
{
    /// <summary>
    ///     Base view model for all other view models.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Private Members

        /// <summary>
        ///     Holds the backing store for the <see cref="IsBusy" /> property.
        /// </summary>
        private bool v_IsBusy;

        /// <summary>
        ///     Holds the backing store for the <see cref="Title" /> property.
        /// </summary>
        private string v_Title = string.Empty;

        #endregion

        #region Public Members

        /// <summary>
        ///     Gets or sets if the view model is busy.
        /// </summary>
        public bool IsBusy
        {
            get => v_IsBusy;
            set => SetProperty(ref v_IsBusy, value);
        }

        /// <summary>
        ///     Gets or sets the title of the page.
        /// </summary>
        public string Title
        {
            get => v_Title;
            set => SetProperty(ref v_Title, value);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Sets the property value, invoking any changed handlers.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="backingStore">The store for the property value.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="propertyName">The name of the property, defaults to the name of the member invoking this method.</param>
        /// <param name="onChanged">An optional callback to invoke when the property is set.</param>
        /// <returns><c>true</c> if the value has changed, otherwise <c>false</c>.</returns>
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            // Do not set value or callbacks and events if set value is equal to current value
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                return false;
            }

            backingStore = value;

            // Call the callback and property changed event when value changes
            onChanged?.Invoke();
            InvokePropertyChanged(propertyName);

            return true;
        }

        #endregion

        #region INotifyPropertyChanged

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Invokes the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}