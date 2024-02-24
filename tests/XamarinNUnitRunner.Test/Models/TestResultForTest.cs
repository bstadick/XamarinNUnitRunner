using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace XamarinNUnitRunner.Test.Models
{
    /// <summary>
    ///     Implements ITestResult for tests.
    /// </summary>
    public class TestResultForTest : ITestResult
    {
        #region Implementation of ITestResult

        /// <inheritdoc />
        public TNode ToXml(bool recursive)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNode AddToXml(TNode parentNode, bool recursive)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ResultState ResultState { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        public string FullName { get; set; }

        /// <inheritdoc />
        public double Duration { get; set; }

        /// <inheritdoc />
        public DateTime StartTime { get; set; }

        /// <inheritdoc />
        public DateTime EndTime { get; set; }

        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        public string StackTrace { get; set; }

        /// <inheritdoc />
        public int AssertCount { get; set; }

        /// <inheritdoc />
        public int FailCount { get; set; }

        /// <inheritdoc />
        public int WarningCount { get; set; }

        /// <inheritdoc />
        public int PassCount { get; set; }

        /// <inheritdoc />
        public int SkipCount { get; set; }

        /// <inheritdoc />
        public int InconclusiveCount { get; set; }

        /// <inheritdoc />
        public bool HasChildren { get; set; }

        /// <inheritdoc />
        public IEnumerable<ITestResult> Children { get; set; }

        /// <inheritdoc />
        public ITest Test { get; set; }

        /// <inheritdoc />
        public string Output { get; set; }

        /// <inheritdoc />
        public IList<AssertionResult> AssertionResults { get; set; }

        /// <inheritdoc />
        public ICollection<TestAttachment> TestAttachments { get; set; }

        #endregion
    }
}
