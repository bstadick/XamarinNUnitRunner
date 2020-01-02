using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using XamarinNUnitRunner.ViewModels;

namespace XamarinNUnitRunner.Test.ViewModels
{
    [TestFixture]
    public class BaseViewModelTest
    {
        // Tests for PropertyChanged Event Handler is tested by tests for SetProperty method

        #region Tests for IsBusy Property

        [Test]
        public void TestIsBusyPropertyCanBeSetAndInvokesPropertyChangedEventIfSetValueIsNotSameAsCurrentValue(
            [Values] bool isBusy, [Values] bool isChangedEventNull)
        {
            object sender = null;
            PropertyChangedEventArgs args = null;
            int invocationCount = 0;
            int expectedCount = isChangedEventNull ? 0 : 1;

            BaseViewModel model = new BaseViewModel();

            if (!isChangedEventNull)
            {
                model.PropertyChanged += (s, a) =>
                {
                    sender = s;
                    args = a;
                    invocationCount++;
                };
            }

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(0, invocationCount);

            if (model.IsBusy == isBusy)
            {
                model.IsBusy = !isBusy;
                expectedCount = isChangedEventNull ? 0 : 2;
            }

            model.IsBusy = isBusy;

            Assert.AreEqual(isBusy, model.IsBusy);
            Assert.AreEqual(expectedCount, invocationCount);
            if (!isChangedEventNull)
            {
                Assert.IsNotNull(sender);
                Assert.AreSame(model, sender);
                Assert.IsNotNull(args);
                Assert.AreEqual("IsBusy", args.PropertyName);
            }
        }

        [Test]
        public void TestIsBusyPropertyCanBeSetAndDoesNotInvokePropertyChangedEventIfSetValueIsSameAsCurrentValue(
            [Values] bool initialValue)
        {
            int invocationCount = 0;
            int expectedCount = 0;

            BaseViewModel model = new BaseViewModel();

            model.PropertyChanged += (s, a) => { invocationCount++; };

            Assert.IsFalse(model.IsBusy);
            Assert.AreEqual(0, invocationCount);

            if (model.IsBusy != initialValue)
            {
                model.IsBusy = initialValue;

                Assert.AreEqual(initialValue, model.IsBusy);
                Assert.AreEqual(1, invocationCount);
                expectedCount = 1;
            }

            model.IsBusy = initialValue;

            Assert.AreEqual(initialValue, model.IsBusy);
            Assert.AreEqual(expectedCount, invocationCount);
        }

        #endregion

        #region Tests for Title Property

        [Test]
        public void TestTitlePropertyCanBeSetAndInvokesPropertyChangedEventIfSetValueIsNotSameAsCurrentValue(
            [Values("Hello", "", null)] string title, [Values] bool isChangedEventNull)
        {
            object sender = null;
            PropertyChangedEventArgs args = null;
            int invocationCount = 0;
            int expectedCount = isChangedEventNull ? 0 : 1;

            BaseViewModel model = new BaseViewModel();

            if (!isChangedEventNull)
            {
                model.PropertyChanged += (s, a) =>
                {
                    sender = s;
                    args = a;
                    invocationCount++;
                };
            }

            Assert.AreEqual(string.Empty, model.Title);
            Assert.AreEqual(0, invocationCount);

            // If title is same as initial, set it to a different value to invoke change
            if (model.Title == title)
            {
                model.Title = title + "a";
                expectedCount = isChangedEventNull ? 0 : 2;
            }

            model.Title = title;

            Assert.AreEqual(title, model.Title);
            Assert.AreEqual(expectedCount, invocationCount);
            if (!isChangedEventNull)
            {
                Assert.IsNotNull(sender);
                Assert.AreSame(model, sender);
                Assert.IsNotNull(args);
                Assert.AreEqual("Title", args.PropertyName);
            }
        }

        [Test]
        public void TestTitlePropertyCanBeSetAndDoesNotInvokePropertyChangedEventIfSetValueIsSameAsCurrentValue(
            [Values("Hello", "", null)] string title)
        {
            int invocationCount = 0;
            int expectedCount = 0;

            BaseViewModel model = new BaseViewModel();

            model.PropertyChanged += (s, a) => { invocationCount++; };

            Assert.AreEqual(string.Empty, model.Title);
            Assert.AreEqual(0, invocationCount);

            // If title is not same as initial, set it to initial value
            if (model.Title != title)
            {
                model.Title = title;

                Assert.AreEqual(title, model.Title);
                Assert.AreEqual(1, invocationCount);
                expectedCount = 1;
            }

            model.Title = title;

            Assert.AreEqual(title, model.Title);
            Assert.AreEqual(expectedCount, invocationCount);
        }

        #endregion

        #region Tests for SetProperty

        [Test]
        public void
            TestSetPropertyWithBackStoreValueEqualToValueDoesNotSetStoreNorInvokesPropertyChangedEventAndReturnsFalse(
                [Values("Hello", "", null)] string currentValue)
        {
            int invocationCount = 0;
            int callbackCount = 0;

            BaseViewModelForTest model = new BaseViewModelForTest();

            model.PropertyChanged += (s, a) => { invocationCount++; };

            void Callback()
            {
                callbackCount++;
            }

            string backingStore = currentValue;
            string value = currentValue;

            bool valueSet = model.InvokeSetProperty(ref backingStore, value, string.Empty, Callback);

            Assert.IsFalse(valueSet);
            Assert.AreEqual(currentValue, backingStore);
            Assert.AreEqual(0, invocationCount);
            Assert.AreEqual(0, callbackCount);
        }

        [Test]
        public void
            TestSetPropertyWithBackStoreValueNotEqualToValueSetsStoreAndInvokesPropertyChangedEventAndReturnsTrue(
                [Values("Hello", "", null)] string currentValue, [Values("Hello", "", null)] string newValue,
                [Values] bool isChangedEventNull, [Values] bool isCallbackNull)
        {
            // Not valid test case as expecting values to be different
            if (currentValue == newValue)
            {
                return;
            }

            const string propertyName = "Title";
            object sender = null;
            PropertyChangedEventArgs args = null;
            int invocationCount = 0;
            int callbackCount = 0;
            int expectedCount = isChangedEventNull ? 0 : 1;
            int expectedCallbackCount = isCallbackNull ? 0 : 1;

            BaseViewModelForTest model = new BaseViewModelForTest();

            if (!isChangedEventNull)
            {
                model.PropertyChanged += (s, a) =>
                {
                    sender = s;
                    args = a;
                    invocationCount++;
                };
            }

            void Callback()
            {
                callbackCount++;
            }

            Action callback = isCallbackNull ? (Action) null : Callback;

            string backingStore = currentValue;

            bool valueSet = model.InvokeSetProperty(ref backingStore, newValue, propertyName, callback);

            Assert.IsTrue(valueSet);
            Assert.AreEqual(newValue, backingStore);
            Assert.AreEqual(expectedCount, invocationCount);
            Assert.AreEqual(expectedCallbackCount, callbackCount);
            if (!isChangedEventNull)
            {
                Assert.IsNotNull(sender);
                Assert.AreSame(model, sender);
                Assert.IsNotNull(args);
                Assert.AreEqual(propertyName, args.PropertyName);
            }
        }

        #endregion

        #region Nested Class: BaseViewModelForTest

        /// <summary>
        ///     BaseViewModel for tests.
        /// </summary>
        private class BaseViewModelForTest : BaseViewModel
        {
            #region Public Methods

            /// <summary>
            ///     Invokes SetProperty method.
            /// </summary>
            /// <typeparam name="T">The type of the property.</typeparam>
            /// <param name="backingStore">The store for the property value.</param>
            /// <param name="value">The value to set the property to.</param>
            /// <param name="propertyName">The name of the property, defaults to the name of the member invoking this method.</param>
            /// <param name="onChanged">An optional callback to invoke when the property is set.</param>
            /// <returns>true if the value has changed, otherwise false.</returns>
            public bool InvokeSetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "",
                Action onChanged = null)
            {
                return SetProperty(ref backingStore, value, propertyName, onChanged);
            }

            #endregion
        }

        #endregion
    }
}