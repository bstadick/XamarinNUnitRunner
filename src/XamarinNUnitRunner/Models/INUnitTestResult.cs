using System;
using NUnit.Framework.Interfaces;
using Xamarin.Forms;

namespace XamarinNUnitRunner.Models
{
    /// <summary>
    ///     Extension of the <see cref="ITestResult" /> interface.
    /// </summary>
    public interface INUnitTestResult : ITestResult
    {
        /// <summary>
        ///     Gets or sets the underlying <see cref="ITestResult" />.
        /// </summary>
        ITestResult Result { get; set; }

        /// <summary>
        ///     Gets the result state status string of the test.
        /// </summary>
        string ResultStateStatus { get; }

        /// <summary>
        ///     Gets the color of the test result string.
        /// </summary>
        Color TextColor { get; }

        /// <summary>
        ///     Gets the test formatted duration string.
        /// </summary>
        string DurationString { get; }

        /// <summary>
        ///     Gets if there are inconclusive child tests.
        /// </summary>
        bool HasInconclusive { get; }

        /// <summary>
        ///     Gets if there are warning child tests.
        /// </summary>
        bool HasWarning { get; }

        /// <summary>
        ///     Gets if there are skipped child tests.
        /// </summary>
        bool HasSkip { get; }

        /// <summary>
        ///     Gets if there are output messages of child tests.
        /// </summary>
        /// <remarks>An output message such as from <see cref="Console.WriteLine()" />.</remarks>
        bool HasOutput { get; }

        /// <summary>
        ///     Gets if there are exception messages of child tests.
        /// </summary>
        /// <remarks>An exception message due to an uncaught exception.</remarks>
        bool HasMessage { get; }

        /// <summary>
        ///     Gets if there are stack traces of child tests.
        /// </summary>
        bool HasStackTrace { get; }

        /// <summary>
        ///     Gets if an assertion has failed.
        /// </summary>
        bool HasFailedAssertions { get; }

        /// <summary>
        ///     Gets a string of failed assertions.
        /// </summary>
        string FailedAssertionsString { get; }
    }
}