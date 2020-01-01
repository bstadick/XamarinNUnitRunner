using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace XamarinNUnitRunner.Test.Services
{
    /// <summary>
    ///     Implements ITestListener for tests.
    /// </summary>
    public class TestListenerForTest : ITestListener
    {
        #region Public Members

        /// <summary>
        ///     The dictionary of test artifacts produced by the listener.
        /// </summary>
        public Dictionary<string, TestArtifacts> Tests { get; } = new Dictionary<string, TestArtifacts>();

        #endregion

        #region Private Methods

        /// <summary>
        ///     Adds the test to the dictionary if not already present.
        /// </summary>
        /// <param name="id">The id of the test to add.</param>
        private void AddTest(string id)
        {
            if (!Tests.ContainsKey(id))
            {
                Tests.Add(id, new TestArtifacts());
            }
        }

        #endregion

        #region Implementation of ITestListener

        /// <inheritdoc />
        public void TestStarted(ITest test)
        {
            AddTest(test.Id);
            Tests[test.Id].Test = test;
        }

        /// <inheritdoc />
        public void TestFinished(ITestResult result)
        {
            AddTest(result.Test.Id);
            Tests[result.Test.Id].Result = result;
        }

        /// <inheritdoc />
        public void TestOutput(TestOutput output)
        {
            AddTest(output.TestId);
            Tests[output.TestId].Outputs.Add(output);
        }

        /// <inheritdoc />
        public void SendMessage(TestMessage message)
        {
            AddTest(message.TestId);
            Tests[message.TestId].Messages.Add(message);
        }

        #endregion
    }

    /// <summary>
    ///     Class that holds the artifacts of a test run.
    /// </summary>
    public class TestArtifacts
    {
        #region Public Members

        /// <summary>
        ///     The test the artifacts belongs to.
        /// </summary>
        public ITest Test { get; set; }

        /// <summary>
        ///     The result of the test.
        /// </summary>
        public ITestResult Result { get; set; }

        /// <summary>
        ///     The outputs of the test.
        /// </summary>
        public IList<TestOutput> Outputs { get; } = new List<TestOutput>();

        /// <summary>
        ///     The messages produced by the test.
        /// </summary>
        public IList<TestMessage> Messages { get; } = new List<TestMessage>();

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override string ToString()
        {
            return Test?.FullName ?? "TestArtifacts: ITest null";
        }

        #endregion
    }
}