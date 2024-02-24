using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using NUnit.Framework.Interfaces;
using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Resources;

namespace XamarinNUnitRunner.Services
{
    /// <summary>
    ///     Listens to and reports results of running tests.
    /// </summary>
    public class NUnitTestListener : ITestListener, IDisposable
    {
        #region Private Members

        private bool v_Listen;

        private readonly Thread v_LoggingThread;

        private readonly ConcurrentQueue<string> v_LogQueue = new ConcurrentQueue<string>();

        #endregion

        #region Public Members

        /// <summary>
        ///     The dictionary of test ID's and their artifacts produced by the listener.
        /// </summary>
        public IDictionary<string, NUnitTestArtifact> Tests { get; } = new Dictionary<string, NUnitTestArtifact>();

        /// <summary>
        ///     Delegate to write messages to an output stream.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public delegate void WriteOutputDelegate(string message);

        /// <summary>
        ///     Event called to write messages.
        /// </summary>
        public event WriteOutputDelegate WriteOutput;

        #endregion

        public NUnitTestListener()
        {
            v_LoggingThread = new Thread(LogListener);
            v_LoggingThread.Name = "NUnitTestListenerLogListener";
            v_Listen = true;
            v_LoggingThread.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Protected Methods

        protected void LogListener()
        {
            while (v_Listen)
            {
                if (v_LogQueue.Count > 0 && WriteOutput != null && v_LogQueue.TryDequeue(out string message))
                {
                    try
                    {
                        WriteOutput.Invoke(message);
                    }
                    catch
                    {
                        // Ignore
                    }
                }
            }
        }

        /// <summary>
        ///     Write message to <see cref="WriteOutput" /> event handler.
        /// </summary>
        /// <param name="msg">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        protected void WriteMessage(string msg, params object[] args)
        {
            try
            {
                // Only queue messages for logging if output is attached
                if (WriteOutput != null)
                {
                    v_LogQueue.Enqueue(string.Format(CultureInfo.InvariantCulture, msg, args));
                }
            }
            catch
            {
                // Ignore
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (v_Listen)
            {
                v_Listen = false;
                v_LoggingThread.Join();
            }
        }

        #endregion

        #region Implementation of ITestListener

        /// <inheritdoc />
        public virtual void TestStarted(ITest test)
        {
            if (test?.Id != null && !Tests.ContainsKey(test.Id))
            {
                Tests.Add(test.Id, new NUnitTestArtifact(test));
            }

            WriteMessage("{0} {1}", Resource.TestListenerStarted, test?.FullName);
        }

        /// <inheritdoc />
        public virtual void TestFinished(ITestResult result)
        {
            if (result?.Test?.Id != null && Tests.ContainsKey(result.Test.Id))
            {
                Tests[result.Test.Id].Result = result;
            }

            WriteMessage("{0} {1}: {2}", Resource.TestListenerFinished, result?.Test?.FullName,
                result?.ResultState);
        }

        /// <inheritdoc />
        public virtual void TestOutput(TestOutput output)
        {
            string fullName = string.Empty;
            if (output?.TestId != null && Tests.ContainsKey(output.TestId))
            {
                Tests[output.TestId].Outputs.Add(output);
                fullName = Tests[output.TestId].Test.FullName;
            }

            WriteMessage("{0} {1}:{2}", Resource.TestListenerOutput, fullName, output?.Text);
        }

        /// <inheritdoc />
        public virtual void SendMessage(TestMessage message)
        {
            string fullName = string.Empty;
            if (message?.TestId != null && Tests.ContainsKey(message.TestId))
            {
                Tests[message.TestId].Messages.Add(message);
                fullName = Tests[message.TestId].Test.FullName;
            }

            WriteMessage("{0} {1}[{2}]: {3}", Resource.TestListenerMessage, fullName, message?.Destination,
                message?.Message);
        }

        #endregion
    }
}