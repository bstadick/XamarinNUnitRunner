using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace XamarinNUnitRunner.Test
{
    [TestFixture]
    public class ExceptionHelperTest
    {
        #region Tests for ThrowArgumentNullException

        [Test]
        public void TestThrowArgumentNullExceptionReturnsArgumentNullExceptionWithMessageAndParamName(
            [ValueSource(nameof(GetParamNames))] string paramName)
        {
            string expected = $"The {paramName} cannot be null." + (string.IsNullOrEmpty(paramName)
                                  ? string.Empty
                                  : $" (Parameter '{paramName}')");

            ArgumentNullException exception = ExceptionHelper.ThrowArgumentNullException(paramName);

            Assert.IsNotNull(exception);
            Assert.AreEqual(paramName, exception.ParamName);
            Assert.AreEqual(expected, exception.Message);
        }

        #endregion

        #region Tests for ThrowArgumentExceptionForNullOrEmpty

        [Test]
        public void TestThrowArgumentExceptionForNullOrEmptyReturnsArgumentExceptionWithMessageAndParamName(
            [ValueSource(nameof(GetParamNames))] string paramName)
        {
            string expected = $"The {paramName} cannot be null or empty." + (string.IsNullOrEmpty(paramName)
                                  ? string.Empty
                                  : $" (Parameter '{paramName}')");

            ArgumentException exception = ExceptionHelper.ThrowArgumentExceptionForNullOrEmpty(paramName);

            Assert.IsNotNull(exception);
            Assert.AreEqual(paramName, exception.ParamName);
            Assert.AreEqual(expected, exception.Message);
        }

        #endregion

        #region Tests for ThrowArgumentException

        [Test]
        public void TestThrowArgumentExceptionReturnsArgumentExceptionWithMessageAndParamName(
            [ValueSource(nameof(GetParamNames))] string paramName)
        {
            const string msg = "This is a test message.";
            string expected = msg + (string.IsNullOrEmpty(paramName) ? string.Empty : $" (Parameter '{paramName}')");

            ArgumentException exception = ExceptionHelper.ThrowArgumentException(msg, paramName);

            Assert.IsNotNull(exception);
            Assert.AreEqual(paramName, exception.ParamName);
            Assert.AreEqual(expected, exception.Message);
        }

        #endregion

        #region Tests for ThrowArgumentOutOfRangeExceptionForElementTypeEnum

        [Test]
        public void
            TestThrowArgumentOutOfRangeExceptionForElementTypeEnumReturnsArgumentOutOfRangeExceptionWithMessageAndParamNameAndValue(
                [ValueSource(nameof(GetParamNames))] string paramName, [ValueSource(nameof(GetArgValues))] string arg,
                [Values] bool isValueNull)
        {
            object actualValue = 0;
            if (isValueNull)
            {
                actualValue = null;
            }

            string expectedArg = arg != null && arg.Equals("parameter") ? " parameter" : arg;
            string expected = $"The given element type is not supported.{expectedArg}" +
                              (string.IsNullOrEmpty(paramName) ? string.Empty : $" (Parameter '{paramName}')") +
                              (isValueNull ? string.Empty : $"\r\nActual value was {actualValue}.");

            ArgumentOutOfRangeException exception =
                ExceptionHelper.ThrowArgumentOutOfRangeExceptionForElementTypeEnum(paramName, actualValue, arg);

            Assert.IsNotNull(exception);
            Assert.AreEqual(paramName, exception.ParamName);
            Assert.AreEqual(actualValue, exception.ActualValue);
            Assert.AreEqual(expected, exception.Message);
        }

        #endregion

        #region Tests for ThrowArgumentOutOfRangeExceptionForValueLessThanZero

        [Test]
        public void
            TestThrowArgumentOutOfRangeExceptionForValueLessThanZeroReturnsArgumentOutOfRangeExceptionWithMessageAndParamName(
                [ValueSource(nameof(GetParamNames))] string paramName, [Values] bool isValueNull)
        {
            object actualValue = 0;
            if (isValueNull)
            {
                actualValue = null;
            }

            string expected = $"The {paramName} must be greater than or equal to 0." +
                              (string.IsNullOrEmpty(paramName) ? string.Empty : $" (Parameter '{paramName}')") +
                              (isValueNull ? string.Empty : $"\r\nActual value was {actualValue}.");

            ArgumentOutOfRangeException exception =
                ExceptionHelper.ThrowArgumentOutOfRangeExceptionForValueLessThanZero(paramName, actualValue);

            Assert.IsNotNull(exception);
            Assert.AreEqual(paramName, exception.ParamName);
            Assert.AreEqual(actualValue, exception.ActualValue);
            Assert.AreEqual(expected, exception.Message);
        }

        #endregion

        #region Tests for ThrowInvalidOperationExceptionForChildAlreadySet

        [Test]
        public void TestThrowInvalidOperationExceptionForChildAlreadySetReturnsInvalidOperationExceptionWithMessage(
            [ValueSource(nameof(GetParamNames))] string paramName)
        {
            const string expected = "The child element has already been set for this instance.";

            InvalidOperationException exception = ExceptionHelper.ThrowInvalidOperationExceptionForChildAlreadySet();

            Assert.IsNotNull(exception);
            Assert.AreEqual(expected, exception.Message);
        }

        #endregion

        #region Tests for ThrowInvalidOperationExceptionForFilterBuild

        [Test]
        public void TestThrowInvalidOperationExceptionForFilterBuildReturnsInvalidOperationExceptionWithMessage(
            [ValueSource(nameof(GetFormatMessages))]
            string msg, [ValueSource(nameof(GetParamNames))] string paramName)
        {
            string expectedMsg = msg ?? string.Empty;
            string expected = string.Format(expectedMsg, paramName) + " Forward traversal will not proceed properly." +
                              " This may indicate an error in the construction or parsing of the filter.";

            InvalidOperationException exception =
                ExceptionHelper.ThrowInvalidOperationExceptionForFilterBuild(msg, paramName);

            Assert.IsNotNull(exception);
            Assert.AreEqual(expected, exception.Message);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets test strings for paramName.
        /// </summary>
        /// <returns>Test strings for paramName..</returns>
        public static IList<string> GetParamNames()
        {
            return new List<string> {"parameter", " ", string.Empty, null};
        }

        /// <summary>
        ///     Gets test strings for format arg.
        /// </summary>
        /// <returns>Test strings for format arg.</returns>
        public static IList<string> GetArgValues()
        {
            return new List<string>
                {"parameter", " parameter", "\tparameter", "\rparameter", "\nparameter", " ", string.Empty, null};
        }

        /// <summary>
        ///     Gets test strings for format message.
        /// </summary>
        /// <returns>Test strings for format message.</returns>
        public static IList<string> GetFormatMessages()
        {
            return new List<string>
                {"This is a test message for {0}.", "This is a test message for.", string.Empty, null};
        }

        #endregion
    }
}