using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using XamarinNUnitRunner.Models;

namespace XamarinNUnitRunner.Test.Models
{
    [TestFixture]
    public class NUnitTestArtifactTest
    {
        #region Tests for Constructor

        [Test]
        public void TestConstructorWithTest([Values] bool isNull)
        {
            ITest test = isNull ? null : new TestSuite("suite-name");

            NUnitTestArtifact artifact = new NUnitTestArtifact(test);

            Assert.AreSame(test, artifact.Test);
        }

        #endregion

        #region Tests for Test Property

        [Test]
        public void TestTestPropertyReturnsTest([Values] bool isNull)
        {
            ITest test = isNull ? null : new TestSuite("suite-name");

            NUnitTestArtifact artifact = new NUnitTestArtifact(test);

            Assert.AreSame(test, artifact.Test);
        }

        #endregion

        #region Tests for Result Property

        [Test]
        public void TestResultPropertyCanBeSet()
        {
            TestSuite test = new TestSuite("suite-name");
            ITestResult result = new TestSuiteResult(test);

            NUnitTestArtifact artifact = new NUnitTestArtifact(test);

            Assert.IsNull(artifact.Result);

            artifact.Result = result;

            Assert.AreSame(result, artifact.Result);
        }

        #endregion

        #region Tests for Outputs Property

        [Test]
        public void TestOutputsPropertyReturnsTestOutputList()
        {
            ITest test = new TestSuite("suite-name");
            TestOutput testOutput = new TestOutput("text", "stream", "id", "name");
            IList<TestOutput> expectedOutputs = new List<TestOutput> {testOutput, null};

            NUnitTestArtifact artifact = new NUnitTestArtifact(test);

            IList<TestOutput> outputs = artifact.Outputs;

            Assert.IsNotNull(outputs);
            CollectionAssert.IsEmpty(outputs);

            outputs.Add(testOutput);
            outputs.Add(null);

            CollectionAssert.AreEqual(expectedOutputs, artifact.Outputs);
        }

        #endregion

        #region Tests for Messages Property

        [Test]
        public void TestMessagesPropertyReturnsTestMessageList()
        {
            ITest test = new TestSuite("suite-name");
            TestMessage testMessage = new TestMessage("destination", "text", "id");
            IList<TestMessage> expectedMessages = new List<TestMessage> {testMessage, null};

            NUnitTestArtifact artifact = new NUnitTestArtifact(test);

            IList<TestMessage> messages = artifact.Messages;

            Assert.IsNotNull(messages);
            CollectionAssert.IsEmpty(messages);

            messages.Add(testMessage);
            messages.Add(null);

            CollectionAssert.AreEqual(expectedMessages, artifact.Messages);
        }

        #endregion

        #region Tests for ToString

        [Test]
        public void TestToStringReturnsTestFullNameWhenTestNotNull()
        {
            ITest test = new TestSuite("suite-name");

            NUnitTestArtifact artifact = new NUnitTestArtifact(test);

            Assert.AreEqual(test.FullName, artifact.ToString());
        }

        [Test]
        public void TestToStringReturnsTestNullMessageWhenTestIsNull()
        {
            NUnitTestArtifact artifact = new NUnitTestArtifact(null);

            Assert.AreEqual("NUnitTestArtifacts: ITest null", artifact.ToString());
        }

        #endregion
    }
}