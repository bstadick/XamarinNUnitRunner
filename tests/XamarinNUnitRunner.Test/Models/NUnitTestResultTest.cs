using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using Xamarin.Forms;
using XamarinNUnitRunner.Models;

namespace XamarinNUnitRunner.Test.Models
{
    [TestFixture]
    public class NUnitTestResultTest
    {
        #region Tests for Constructor

        [Test]
        public void TestConstructorWithTestResult([Values] bool isNull)
        {
            ITestResult result = isNull ? null : new TestResultForTest();

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreSame(result, test.Result);
        }

        [Test]
        public void TestConstructorWithNUnitTestResult([Values] bool isNull)
        {
            ITestResult result = isNull ? null : new TestResultForTest();
            INUnitTestResult outer = new NUnitTestResult(result);

            INUnitTestResult test = new NUnitTestResult(outer);

            Assert.AreSame(result, test.Result);
        }

        #endregion

        #region Tests for Result Property

        [Test]
        public void TestResultPropertyCanBeSet([Values] bool isNull)
        {
            ITestResult result = !isNull ? null : new TestResultForTest();
            ITestResult expected = isNull ? null : new TestResultForTest();

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreSame(result, test.Result);

            test.Result = expected;

            Assert.AreSame(expected, test.Result);
        }

        #endregion

        #region Tests for ResultStateStatus Property

        [Test]
        public void TestResultStateStatusPropertyWithResultNullReturnsDefaultString([Values] bool hasResult)
        {
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.ResultState = null;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(result, test.Result);
            Assert.AreEqual("Test not executed.", test.ResultStateStatus);
        }

        [Test]
        public void TestResultStateStatusPropertyWithResultReturnsResultStateStatusString()
        {
            ResultState state = ResultState.Success;
            TestResultForTest result = new TestResultForTest();
            result.ResultState = state;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.IsNotNull(test.Result);
            Assert.AreEqual(state.Status.ToString(), test.ResultStateStatus);
        }

        #endregion

        #region Tests for TextColor Property

        [Test]
        public void TestTextColorPropertyWithNullResultReturnsColorBlack([Values] bool hasResult)
        {
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.ResultState = null;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(Color.Black, test.TextColor);
        }

        [Test]
        public void TestTextColorPropertyWithNotSupportedTestStatusReturnsColorBlack()
        {
            TestResultForTest result = new TestResultForTest();
            result.ResultState = new ResultState((TestStatus) (-1));

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(Color.Black, test.TextColor);
        }

        [Test]
        public void TestTextColorPropertyReturnsColorForResultState([Values] TestStatus status)
        {
            TestResultForTest result = new TestResultForTest();
            result.ResultState = new ResultState((TestStatus) (-1));

            Color expected = Color.Black;
            ResultState state = ResultState.NotRunnable;
            switch (status)
            {
                case TestStatus.Inconclusive:
                    state = ResultState.Inconclusive;
                    expected = Color.Purple;
                    break;
                case TestStatus.Skipped:
                    state = ResultState.Ignored;
                    expected = Color.Blue;
                    break;
                case TestStatus.Passed:
                    state = ResultState.Success;
                    expected = Color.Green;
                    break;
                case TestStatus.Warning:
                    state = ResultState.Warning;
                    expected = Color.Orange;
                    break;
                case TestStatus.Failed:
                    state = ResultState.Failure;
                    expected = Color.Red;
                    break;
                default:
                    Assert.Fail($"This status {status} is not supported.");
                    break;
            }

            result.ResultState = state;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.TextColor);
        }

        #endregion

        #region Tests for DurationString Property

        [Test]
        public void TestDurationStringPropertyReturnsFormattedDurationString([Values] bool hasResult,
            [Values] bool inMilliseconds)
        {
            double count = inMilliseconds ? 0.005123456 : 5.123456;
            string unit = inMilliseconds ? "ms" : "sec";
            string expected = hasResult ? "5.123 " + unit : "0 sec";
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.Duration = count;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.DurationString);
        }

        #endregion

        #region Tests for HasInconclusive Property

        [Test]
        public void TestHasInconclusivePropertyWithNotInconclusiveReturnsFalse([Values] bool hasResult)
        {
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.InconclusiveCount = 0;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(0, test.InconclusiveCount);
            Assert.IsFalse(test.HasInconclusive);
        }

        [Test]
        public void TestHasInconclusivePropertyWithInconclusiveReturnsTrue()
        {
            const int count = 5;
            TestResultForTest result = new TestResultForTest();
            result.InconclusiveCount = count;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(count, test.InconclusiveCount);
            Assert.IsTrue(test.HasInconclusive);
        }

        #endregion

        #region Tests for HasWarning Property

        [Test]
        public void TestHasWarningPropertyWithNotInconclusiveReturnsFalse([Values] bool hasResult)
        {
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.InconclusiveCount = 0;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(0, test.WarningCount);
            Assert.IsFalse(test.HasWarning);
        }

        [Test]
        public void TestHasWarningPropertyWithInconclusiveReturnsTrue()
        {
            const int count = 5;
            TestResultForTest result = new TestResultForTest();
            result.WarningCount = count;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(count, test.WarningCount);
            Assert.IsTrue(test.HasWarning);
        }

        #endregion

        #region Tests for HasSkip Property

        [Test]
        public void TestHasSkipPropertyWithNotInconclusiveReturnsFalse([Values] bool hasResult)
        {
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.SkipCount = 0;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(0, test.SkipCount);
            Assert.IsFalse(test.HasSkip);
        }

        [Test]
        public void TestHasSkipPropertyWithInconclusiveReturnsTrue()
        {
            const int count = 5;
            TestResultForTest result = new TestResultForTest();
            result.SkipCount = count;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(count, test.SkipCount);
            Assert.IsTrue(test.HasSkip);
        }

        #endregion

        #region Tests for HasOutput Property

        [Test]
        public void TestHasOutputPropertyWithNotInconclusiveReturnsFalse([Values] bool hasResult, [Values] bool isNull)
        {
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.Output = isNull ? null : string.Empty;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(string.Empty, test.Output);
            Assert.IsFalse(test.HasOutput);
        }

        [Test]
        public void TestHasOutputPropertyWithInconclusiveReturnsTrue()
        {
            const string msg = "This is a test message.";
            TestResultForTest result = new TestResultForTest();
            result.Output = msg;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(msg, test.Output);
            Assert.IsTrue(test.HasOutput);
        }

        #endregion

        #region Tests for HasMessage Property

        [Test]
        public void TestHasMessagePropertyWithNotInconclusiveReturnsFalse([Values] bool hasResult, [Values] bool isNull)
        {
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.Message = isNull ? null : string.Empty;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(string.Empty, test.Message);
            Assert.IsFalse(test.HasMessage);
        }

        [Test]
        public void TestHasMessagePropertyWithInconclusiveReturnsTrue()
        {
            const string msg = "This is a test message.";
            TestResultForTest result = new TestResultForTest();
            result.Message = msg;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(msg, test.Message);
            Assert.IsTrue(test.HasMessage);
        }

        #endregion

        #region Tests for HasStackTrace Property

        [Test]
        public void TestHasStackTracePropertyWithNotInconclusiveReturnsFalse([Values] bool hasResult,
            [Values] bool isNull)
        {
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.StackTrace = isNull ? null : string.Empty;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(string.Empty, test.StackTrace);
            Assert.IsFalse(test.HasStackTrace);
        }

        [Test]
        public void TestHasStackTracePropertyWithInconclusiveReturnsTrue()
        {
            const string msg = "This is a test message.";
            TestResultForTest result = new TestResultForTest();
            result.StackTrace = msg;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(msg, test.StackTrace);
            Assert.IsTrue(test.HasStackTrace);
        }

        #endregion

        #region Tests for ToXml

        [Test]
        public void TestToXml([Values] bool hasResult, [Values] bool recursive)
        {
            ITestResult resultInstance = new TestSuiteResult(new TestSuite("suite-name"));
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            TNode node = test.ToXml(recursive);

            if (hasResult)
            {
                Assert.IsNotNull(node);
            }
            else
            {
                Assert.IsNull(node);
            }
        }

        #endregion

        #region Tests for AddToXml

        [Test]
        public void TestAddToXml([Values] bool hasResult, [Values] bool isParentNull, [Values] bool recursive)
        {
            ITestResult resultInstance = new TestSuiteResult(new TestSuite("suite-name"));
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            TNode parent = isParentNull ? null : new TNode("parent-node");

            // Parent node null is not handled by NUnit implementation of ITestResult nor the thin wrapper being tested here
            if (hasResult && isParentNull)
            {
                // ReSharper disable once ExpressionIsAlwaysNull
                Assert.Throws(Is.TypeOf<NullReferenceException>(), () => test.AddToXml(parent, recursive));
                return;
            }

            TNode node = test.AddToXml(parent, recursive);

            if (hasResult)
            {
                Assert.IsNotNull(node);
            }
            else
            {
                Assert.IsNull(node);
            }
        }

        #endregion

        #region Tests for ResultState Property

        [Test]
        public void TestResultStatePropertyReturnsResultState([Values] bool hasResult, [Values] bool hasState)
        {
            ResultState state = ResultState.Success;
            ResultState expected = hasResult && hasState ? state : ResultState.Inconclusive;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.ResultState = hasState ? state : null;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.ResultState);
        }

        #endregion

        #region Tests for Name Property

        [Test]
        public void TestNamePropertyReturnsResultName([Values] bool hasResult, [Values] bool hasName)
        {
            const string name = "result-name";
            string expected = hasResult && hasName ? name : string.Empty;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.Name = hasName ? name : null;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.Name);
        }

        #endregion

        #region Tests for FullName Property

        [Test]
        public void TestFullNamePropertyReturnsResultFullName([Values] bool hasResult, [Values] bool hasName)
        {
            const string name = "result-name";
            string expected = hasResult && hasName ? name : string.Empty;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.FullName = hasName ? name : null;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.FullName);
        }

        #endregion

        #region Tests for Duration Property

        [Test]
        public void TestDurationPropertyReturnsTestDuration([Values] bool hasResult)
        {
            const double duration = 5.123456;
            double expected = hasResult ? duration : 0;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.Duration = duration;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.Duration);
        }

        #endregion

        #region Tests for StartTime Property

        [Test]
        public void TestStartTimePropertyReturnsTestStartTime([Values] bool hasResult)
        {
            DateTime time = DateTime.Now;
            DateTime expected = hasResult ? time : DateTime.MinValue;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.StartTime = time;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.StartTime);
        }

        #endregion

        #region Tests for EndTime Property

        [Test]
        public void TestEndTimePropertyReturnsTestEndTime([Values] bool hasResult)
        {
            DateTime time = DateTime.Now;
            DateTime expected = hasResult ? time : DateTime.MaxValue;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.EndTime = time;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.EndTime);
        }

        #endregion

        #region Tests for Message Property

        [Test]
        public void TestMessagePropertyReturnsResultExceptionMessage([Values] bool hasResult, [Values] bool hasMsg)
        {
            const string msg = "This is a test message.";
            string expected = hasResult && hasMsg ? msg : string.Empty;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.Message = hasMsg ? msg : null;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.Message);
        }

        #endregion

        #region Tests for StackTrace Property

        [Test]
        public void TestStackTracePropertyReturnsResultExceptionStackTrace([Values] bool hasResult,
            [Values] bool hasTrace)
        {
            const string trace = "This is a test message.";
            string expected = hasResult && hasTrace ? trace : string.Empty;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.StackTrace = hasTrace ? trace : null;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.StackTrace);
        }

        #endregion

        #region Tests for AssertCount Property

        [Test]
        public void TestAssertCountPropertyReturnsAssertCount([Values] bool hasResult)
        {
            const int count = 5;
            int expected = hasResult ? count : 0;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.AssertCount = count;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.AssertCount);
        }

        #endregion

        #region Tests for FailCount Property

        [Test]
        public void TestFailCountPropertyReturnsFailCount([Values] bool hasResult)
        {
            const int count = 5;
            int expected = hasResult ? count : 0;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.FailCount = count;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.FailCount);
        }

        #endregion

        #region Tests for WarningCount Property

        [Test]
        public void TestWarningCountPropertyReturnsWarningCount([Values] bool hasResult)
        {
            const int count = 5;
            int expected = hasResult ? count : 0;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.WarningCount = count;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.WarningCount);
        }

        #endregion

        #region Tests for PassCount Property

        [Test]
        public void TestPassCountPropertyReturnsPassCount([Values] bool hasResult)
        {
            const int count = 5;
            int expected = hasResult ? count : 0;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.PassCount = count;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.PassCount);
        }

        #endregion

        #region Tests for SkipCount Property

        [Test]
        public void TestSkipCountPropertyReturnsSkipCount([Values] bool hasResult)
        {
            const int count = 5;
            int expected = hasResult ? count : 0;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.SkipCount = count;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.SkipCount);
        }

        #endregion

        #region Tests for InconclusiveCount Property

        [Test]
        public void TestInconclusiveCountPropertyReturnsInconclusiveCount([Values] bool hasResult)
        {
            const int count = 5;
            int expected = hasResult ? count : 0;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.InconclusiveCount = count;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.InconclusiveCount);
        }

        #endregion

        #region Tests for HasChildren Property

        [Test]
        public void TestHasChildrenPropertyReturnsIfTestHasChildren([Values] bool hasResult, [Values] bool hasChildren)
        {
            bool expected = hasResult && hasChildren;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.HasChildren = hasChildren;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.HasChildren);
        }

        #endregion

        #region Tests for Children Property

        [Test]
        public void TestChildrenPropertyReturnsChildren([Values] bool hasResult, [Values] bool hasChildren)
        {
            IEnumerable<ITestResult> children = new List<ITestResult>();
            IEnumerable<ITestResult> expected = hasResult && hasChildren ? children : null;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.Children = expected;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreSame(expected, test.Children);
        }

        #endregion

        #region Tests for Test Property

        [Test]
        public void TestTestPropertyReturnsTest([Values] bool hasResult, [Values] bool hasTest)
        {
            ITest testInstance = new TestSuite("suite-name");
            ITest expected = hasResult && hasTest ? testInstance : null;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.Test = expected;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreSame(expected, test.Test);
        }

        #endregion

        #region Tests for Output Property

        [Test]
        public void TestOutputPropertyReturnsResultOutputMessages([Values] bool hasResult, [Values] bool hasOutput)
        {
            const string output = "This is a test message.";
            string expected = hasResult && hasOutput ? output : string.Empty;
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.Output = hasOutput ? output : null;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            Assert.AreEqual(expected, test.Output);
        }

        #endregion

        #region Tests for AssertionResults Property

        [Test]
        public void TestAssertionResultsPropertyReturnsTestAssertionResults([Values] bool hasResult,
            [Values] bool hasAssertions)
        {
            IList<AssertionResult> assertions = new List<AssertionResult>
                {new AssertionResult(AssertionStatus.Passed, "message", "trace")};
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.AssertionResults = hasAssertions ? assertions : null;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            if (hasResult && hasAssertions)
            {
                CollectionAssert.AreEqual(assertions, test.AssertionResults);
            }
            else
            {
                CollectionAssert.IsEmpty(test.AssertionResults);
            }
        }

        #endregion

        #region Tests for TestAttachments Property

        [Test]
        public void TestTestAttachmentsPropertyReturnsTestAttachments([Values] bool hasResult,
            [Values] bool hasAttachments)
        {
            IList<TestAttachment> attachments = new List<TestAttachment> {new TestAttachment("file.txt", "item")};
            TestResultForTest resultInstance = new TestResultForTest();
            resultInstance.TestAttachments = hasAttachments ? attachments : null;
            ITestResult result = hasResult ? resultInstance : null;

            INUnitTestResult test = new NUnitTestResult(result);

            if (hasResult && hasAttachments)
            {
                CollectionAssert.AreEqual(attachments, test.TestAttachments);
            }
            else
            {
                CollectionAssert.IsEmpty(test.TestAttachments);
            }
        }

        #endregion

        #region Tests for Equals

        [Test]
        public void TestEqualsWithSameResultReturnsTrue([Values] bool isNull)
        {
            TestResultForTest resultInstanceOne = new TestResultForTest();
            ITestResult resultOne = isNull ? null : resultInstanceOne;

            INUnitTestResult testOne = new NUnitTestResult(resultOne);

            Assert.IsTrue(testOne.Equals(resultOne));
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(testOne.Equals(testOne));
        }

        [Test]
        public void TestEqualsWithNotSameResultReturnsFalse([Values] bool isNull)
        {
            TestResultForTest resultInstanceOne = new TestResultForTest();
            TestResultForTest resultInstanceTwo = new TestResultForTest();
            resultInstanceTwo.Name = "result-name";
            ITestResult resultOne = isNull ? null : resultInstanceOne;
            ITestResult resultTwo = isNull ? resultInstanceTwo : null;
            object resultWrong = "string";

            INUnitTestResult testOne = new NUnitTestResult(resultOne);
            INUnitTestResult testTwo = new NUnitTestResult(resultTwo);

            Assert.IsFalse(testOne.Equals(resultTwo));
            Assert.IsFalse(testOne.Equals(testTwo));
            Assert.IsFalse(testOne.Equals(resultWrong));
        }

        #endregion

        #region Nested Class: TestResultForTest

        /// <summary>
        ///     Implements ITestResult for tests.
        /// </summary>
        private class TestResultForTest : ITestResult
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

        #endregion
    }
}