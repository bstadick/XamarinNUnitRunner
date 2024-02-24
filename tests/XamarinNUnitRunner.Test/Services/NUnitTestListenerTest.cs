using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Services;
using XamarinNUnitRunner.Test.Models;

namespace XamarinNUnitRunner.Test.Services
{
    [TestFixture]
    public class NUnitTestListenerTest
    {
        // TODO - NUnitTestListenerTest tests
        // TODO - Fix test concurrency issues

        [Test]
        public void Foo()
        {
            Assert.Fail();
        }

        #region Private Members

        private const int c_WaitTimeout = 500;

        #endregion

        #region Tests for Tests Property

        [Test]
        public void TestTestsPropertyReturnsTestsList()
        {
            ITest test1 = new TestSuite("suite-name1");
            ITest test2 = new TestSuite("suite-name2");
            IDictionary<string, NUnitTestArtifact> expectedMessages = new Dictionary<string, NUnitTestArtifact>
                {{"id1", new NUnitTestArtifact(test1)}, {"id2", new NUnitTestArtifact(test2)}};

            NUnitTestListener listener = new NUnitTestListener();

            IDictionary<string, NUnitTestArtifact> tests = listener.Tests;

            Assert.IsNotNull(tests);
            CollectionAssert.IsEmpty(tests);

            tests.Add(expectedMessages.Single(p => p.Key == "id1"));
            tests.Add(expectedMessages.Single(p => p.Key == "id2"));

            CollectionAssert.AreEqual(expectedMessages, listener.Tests);
        }

        #endregion

        #region Tests for WriteOutput Event

        [Test]
        public void TestWriteOutputEventInvokedOnWriteMessageWhenEventNullReturnsNoMessages()
        {
            NUnitTestListenerForTest listener = new NUnitTestListenerForTest();

            listener.InvokeWriteMessage("First message arg: {0}", "arg1");
            listener.InvokeWriteMessage("Second message arg");
            listener.InvokeWriteMessage("Third message arg: {0} {1}", "arg3", "arg3.5");

            Assert.IsNotNull(listener.Tests);
            CollectionAssert.IsEmpty(listener.Tests);
        }

        [Test]
        public void TestWriteOutputEventInvokedOnWriteMessageReturnsMessage()
        {
            NUnitTestListenerForTest listener = new NUnitTestListenerForTest();

            IList<string> messages = new List<string>();
            listener.WriteOutput += message => messages.Add(message);

            listener.InvokeWriteMessage("First message arg: {0}", "arg1");
            listener.InvokeWriteMessage("Second message arg");
            listener.InvokeWriteMessage("Third message arg: {0} {1}", "arg3", "arg3.5");

            SpinWait.SpinUntil(() => messages.Count >= 3, c_WaitTimeout);

            Assert.IsNotNull(listener.Tests);
            CollectionAssert.IsEmpty(listener.Tests);

            Assert.AreEqual(3, messages.Count);
            CollectionAssert.Contains(messages, "First message arg: arg1");
            CollectionAssert.Contains(messages, "Second message arg");
            CollectionAssert.Contains(messages, "Third message arg: arg3 arg3.5");
        }

        [Test]
        public void TestWriteOutputEventInvokedOnWriteMessageWithExceptionReturnsMessage()
        {
            NUnitTestListenerForTest listener = new NUnitTestListenerForTest();

            IList<string> messages = new List<string>();
            listener.WriteOutput += message =>
            {
                if (message == "Error msg")
                {
                    throw new Exception("Event exception");
                }

                messages.Add(message);
            };

            listener.InvokeWriteMessage("First message arg: {0}", "arg1");
            listener.InvokeWriteMessage("Second message arg");
            listener.InvokeWriteMessage("Error msg");
            listener.InvokeWriteMessage(null);
            listener.InvokeWriteMessage("null", null);
            listener.InvokeWriteMessage("invalid format {1}", "arg");
            listener.InvokeWriteMessage("Third message arg: {0} {1}", "arg3", "arg3.5");

            SpinWait.SpinUntil(() => messages.Count >= 3, c_WaitTimeout);

            Assert.IsNotNull(listener.Tests);
            CollectionAssert.IsEmpty(listener.Tests);

            Assert.AreEqual(3, messages.Count);
            CollectionAssert.Contains(messages, "First message arg: arg1");
            CollectionAssert.Contains(messages, "Second message arg");
            CollectionAssert.Contains(messages, "Third message arg: arg3 arg3.5");
        }

        #endregion

        #region Tests for TestStarted

        [Test]
        public void TestTestStartedWithTestOrTestIdNullWritesMessageAndDoesNotLogTest([Values] bool isTestNull)
        {
            ITest test1 = isTestNull ? null : new TestForTest();
            ITest test2 = new TestForTest {FullName = "suite-name2"};

            NUnitTestListener listener = new NUnitTestListener();

            IList<string> messages = new List<string>();
            listener.WriteOutput += message => messages.Add(message);

            listener.TestStarted(test1);
            listener.TestStarted(test2);

            SpinWait.SpinUntil(() => messages.Count >= 2, c_WaitTimeout);

            Assert.IsNotNull(listener.Tests);
            CollectionAssert.IsEmpty(listener.Tests);

            Assert.AreEqual(2, messages.Count);
            CollectionAssert.Contains(messages, "Started ");
            CollectionAssert.Contains(messages, "Started suite-name2");
        }

        [Test]
        public void TestTestStartedWithTestWritesMessageAndLogsTest()
        {
            ITest test1 = new TestSuite("suite-name1");
            ITest test2 = new TestSuite("suite-name2");
            ITest test3 = new TestForTest {Id = "id3"};

            NUnitTestListener listener = new NUnitTestListener();

            IList<string> messages = new List<string>();
            listener.WriteOutput += message => messages.Add(message);

            listener.TestStarted(test1);
            listener.TestStarted(test2);
            listener.TestStarted(test3);

            Assert.IsNotNull(listener.Tests);
            Assert.AreEqual(3, listener.Tests.Count);
            Assert.IsTrue(listener.Tests.ContainsKey(test1.Id));
            Assert.IsTrue(listener.Tests.ContainsKey(test2.Id));
            Assert.IsTrue(listener.Tests.ContainsKey(test3.Id));

            // Test adding copy of same test isn't added to list but message logged
            listener.TestStarted(test2);
            listener.TestStarted(test1);

            SpinWait.SpinUntil(() => messages.Count >= 5, c_WaitTimeout);

            Assert.IsNotNull(listener.Tests);
            Assert.AreEqual(3, listener.Tests.Count);

            Assert.IsTrue(listener.Tests.ContainsKey(test1.Id));
            NUnitTestArtifact testArtifact1 = listener.Tests[test1.Id];
            Assert.IsNotNull(testArtifact1);
            Assert.AreSame(test1, testArtifact1.Test);
            Assert.IsNull(testArtifact1.Result);
            Assert.IsNotNull(testArtifact1.Outputs);
            CollectionAssert.IsEmpty(testArtifact1.Outputs);
            Assert.IsNotNull(testArtifact1.Messages);
            CollectionAssert.IsEmpty(testArtifact1.Messages);

            Assert.IsTrue(listener.Tests.ContainsKey(test2.Id));
            NUnitTestArtifact testArtifact2 = listener.Tests[test2.Id];
            Assert.IsNotNull(testArtifact2);
            Assert.AreSame(test2, testArtifact2.Test);
            Assert.IsNull(testArtifact2.Result);
            Assert.IsNotNull(testArtifact2.Outputs);
            CollectionAssert.IsEmpty(testArtifact2.Outputs);
            Assert.IsNotNull(testArtifact2.Messages);
            CollectionAssert.IsEmpty(testArtifact2.Messages);

            Assert.IsTrue(listener.Tests.ContainsKey(test3.Id));
            NUnitTestArtifact testArtifact3 = listener.Tests[test3.Id];
            Assert.IsNotNull(testArtifact3);
            Assert.AreSame(test3, testArtifact3.Test);
            Assert.IsNull(testArtifact3.Result);
            Assert.IsNotNull(testArtifact3.Outputs);
            CollectionAssert.IsEmpty(testArtifact3.Outputs);
            Assert.IsNotNull(testArtifact3.Messages);
            CollectionAssert.IsEmpty(testArtifact3.Messages);

            Assert.AreEqual(5, messages.Count);
            Assert.AreEqual(2, messages.Count(m => m == "Started suite-name1"));
            Assert.AreEqual(2, messages.Count(m => m == "Started suite-name2"));
            Assert.AreEqual(1, messages.Count(m => m == "Started "));
        }

        #endregion

        #region Nested Class: NUnitTestListenerForTest

        /// <summary>
        ///     Extends NUnitTestListener for use with tests.
        /// </summary>
        public class NUnitTestListenerForTest : NUnitTestListener
        {
            /// <summary>
            ///     Write message to <see cref="NUnitTestListener.WriteOutput" /> event handler.
            /// </summary>
            /// <param name="msg">A composite format string.</param>
            /// <param name="args">An object array that contains zero or more objects to format.</param>
            public async void InvokeWriteMessage(string msg, params object[] args)
            {
                //await WriteMessage(msg, args).ConfigureAwait(false);
            }
        }

        #endregion
    }
}