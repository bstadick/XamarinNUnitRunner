using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace XamarinNUnitRunner.Models
{
    /// <summary>
    ///     Class that holds the artifacts of a test run.
    /// </summary>
    public class NUnitTestArtifact
    {
        #region Public Members

        /// <summary>
        ///     The <see cref="ITest" /> that the artifact belongs to.
        /// </summary>
        public ITest Test { get; }

        /// <summary>
        ///     The <see cref="ITestResult" /> of the test.
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

        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="NUnitTestArtifact" /> with the given <see cref="ITest" />.
        /// </summary>
        /// <param name="test">The test to initialize with.</param>
        public NUnitTestArtifact(ITest test)
        {
            Test = test;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override string ToString()
        {
            return Test?.FullName ?? "NUnitTestArtifacts: ITest null";
        }

        #endregion
    }
}