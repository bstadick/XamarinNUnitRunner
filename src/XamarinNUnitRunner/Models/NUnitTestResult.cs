using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using Xamarin.Forms;
using XamarinNUnitRunner.Resources;

namespace XamarinNUnitRunner.Models
{
    /// <summary>
    ///     Implementation of <see cref="INUnitTestResult" />.
    /// </summary>
    public class NUnitTestResult : INUnitTestResult
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="NUnitTestResult" /> with the given <see cref="ITestResult" />.
        /// </summary>
        /// <param name="result">The test result to initialize with.</param>
        public NUnitTestResult(ITestResult result)
        {
            if (result is NUnitTestResult nResult)
            {
                result = nResult.Result;
            }

            Result = result;
        }

        #endregion

        #region Implementation of INUnitTestResult

        /// <inheritdoc />
        public ITestResult Result { get; set; }

        /// <inheritdoc />
        public string ResultStateStatus =>
            Result?.ResultState == null ? Resource.TestResultNotExecuted : Result.ResultState.Status.ToString();

        /// <inheritdoc />
        public Color TextColor
        {
            get
            {
                if (Result?.ResultState == null)
                {
                    return Color.Black;
                }

                switch (Result.ResultState.Status)
                {
                    case TestStatus.Inconclusive:
                        return Color.Purple;
                    case TestStatus.Skipped:
                        return Color.Blue;
                    case TestStatus.Passed:
                        return Color.Green;
                    case TestStatus.Warning:
                        return Color.Orange;
                    case TestStatus.Failed:
                        return Color.Red;
                    default:
                        return Color.Black;
                }
            }
        }

        /// <inheritdoc />
        public string DurationString
        {
            get
            {
                string unit = Resource.TestsPageUnitSecond;
                if (Result == null)
                {
                    return $"0 {unit}";
                }

                double duration = Result.Duration;
                if (Result.Duration < 1)
                {
                    duration *= 1000;
                    unit = Resource.TestsPageUnitMillisecond;
                }

                return $"{duration:F3} {unit}";
            }
        }

        /// <inheritdoc />
        public bool HasInconclusive => Result?.InconclusiveCount > 0;

        /// <inheritdoc />
        public bool HasWarning => Result?.WarningCount > 0;

        /// <inheritdoc />
        public bool HasSkip => Result?.SkipCount > 0;

        /// <inheritdoc />
        public bool HasOutput => !string.IsNullOrEmpty(Result?.Output);

        /// <inheritdoc />
        public bool HasMessage => !string.IsNullOrEmpty(Result?.Message) && !HasFailedAssertions;

        /// <inheritdoc />
        public bool HasStackTrace => !string.IsNullOrEmpty(Result?.StackTrace) && !HasFailedAssertions;

        /// <inheritdoc />
        public bool HasFailedAssertions => AssertionResults.Any(x => x != null && x.Status != AssertionStatus.Passed);

        /// <inheritdoc />
        public string FailedAssertionsString => string.Join(Environment.NewLine,
            AssertionResults.Where(x => x != null && x.Status != AssertionStatus.Passed).Select(x =>
                $"{Resource.TestsPageAssertionStatus}{x.Status}" + (string.IsNullOrEmpty(x.Message)
                    ? string.Empty
                    : $"{Environment.NewLine}{x.Message}") +
                (string.IsNullOrEmpty(x.StackTrace)
                    ? string.Empty
                    : $"{Environment.NewLine}{Resource.TestsPageTestStackTrace}{Environment.NewLine}{x.StackTrace}")
            ));

        #endregion

        #region Implementation of ITestResult

        /// <inheritdoc />
        public TNode ToXml(bool recursive)
        {
            return Result?.ToXml(recursive);
        }

        /// <inheritdoc />
        public TNode AddToXml(TNode parentNode, bool recursive)
        {
            return Result?.AddToXml(parentNode, recursive);
        }

        /// <inheritdoc />
        public ResultState ResultState => Result?.ResultState ?? ResultState.Inconclusive;

        /// <inheritdoc />
        public string Name => Result?.Name ?? string.Empty;

        /// <inheritdoc />
        public string FullName => Result?.FullName ?? string.Empty;

        /// <inheritdoc />
        public double Duration => Result?.Duration ?? 0;

        /// <inheritdoc />
        public DateTime StartTime => Result?.StartTime ?? DateTime.MinValue;

        /// <inheritdoc />
        public DateTime EndTime => Result?.EndTime ?? DateTime.MaxValue;

        /// <inheritdoc />
        /// <remarks>An exception message due to an uncaught exception.</remarks>
        public string Message => Result?.Message ?? string.Empty;

        /// <inheritdoc />
        /// <remarks>An exception stack trace due to an uncaught exception.</remarks>
        public string StackTrace => Result?.StackTrace ?? string.Empty;

        /// <inheritdoc />
        public int AssertCount => Result?.AssertCount ?? 0;

        /// <inheritdoc />
        public int FailCount => Result?.FailCount ?? 0;

        /// <inheritdoc />
        public int WarningCount => Result?.WarningCount ?? 0;

        /// <inheritdoc />
        public int PassCount => Result?.PassCount ?? 0;

        /// <inheritdoc />
        public int SkipCount => Result?.SkipCount ?? 0;

        /// <inheritdoc />
        public int InconclusiveCount => Result?.InconclusiveCount ?? 0;

        /// <inheritdoc />
        public bool HasChildren => Result?.HasChildren ?? false;

        /// <inheritdoc />
        public IEnumerable<ITestResult> Children => Result?.Children;

        /// <inheritdoc />
        public ITest Test => Result?.Test;

        /// <inheritdoc />
        /// <remarks>An output message such as from <see cref="Console.WriteLine()" />.</remarks>
        public string Output => Result?.Output ?? string.Empty;

        /// <inheritdoc />
        public IList<AssertionResult> AssertionResults => Result?.AssertionResults ?? new List<AssertionResult>();

        /// <inheritdoc />
        public ICollection<TestAttachment> TestAttachments => Result?.TestAttachments ?? new List<TestAttachment>();

        #endregion

        #region Public Methods

        /// <inheritdoc />
#pragma warning disable 659
        public override bool Equals(object obj)
#pragma warning restore 659
        {
            switch (obj)
            {
                case null:
                    return Result == null;
                case NUnitTestResult nResult:
                    return Equals(Result, nResult.Result);
                case ITestResult result:
                    return Equals(Result, result);
                default:
                    return false;
            }
        }

        #endregion
    }
}