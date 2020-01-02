using System;
using System.Collections.Generic;
using System.Linq;

namespace XamarinNUnitRunner.Test.Stub
{
    /// <summary>
    ///     Interface for test fixture stubs.
    /// </summary>
    public interface ITestFixtureStub
    {
        #region Properties

        /// <summary>
        ///     Gets the number of test cases in the fixture.
        /// </summary>
        int TestCount { get; }

        /// <summary>
        ///     Gets the number of test case (methods) results including parent results (method) when multiple cases per method.
        /// </summary>
        int ResultCount { get; }

        /// <summary>
        ///     Gets the depth of the test result. One level for the DLL and each namespace including the class name.
        /// </summary>
        int ResultsDepth { get; }

        #endregion
    }

    /// <summary>
    ///     Helper class for test fixture stubs
    /// </summary>
    public class TestFixtureStubHelper : ITestFixtureStub
    {
        #region Private Members

        /// <summary>
        ///     Holds the singleton instance.
        /// </summary>
        private static ITestFixtureStub v_Singleton;

        #endregion

        #region Public Members

        /// <summary>
        ///     Gets the list of test fixtures.
        /// </summary>
        public IList<ITestFixtureStub> TestFixtures { get; } = new List<ITestFixtureStub>();

        #endregion

        #region Implementation of ITestFixtureStub

        /// <inheritdoc />
        public int TestCount => TestFixtures.Sum(x => x.TestCount);

        /// <inheritdoc />
        public int ResultCount => TestFixtures.Sum(x => x.ResultCount) + TestFixtures.Count;

        /// <inheritdoc />
        public int ResultsDepth => TestFixtures.Max(x => x.ResultsDepth);

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a new instance of TestFixtureStubHelper.
        /// </summary>
        public TestFixtureStubHelper()
        {
            TestFixtures.Add(new TestFixtureStubOne());
            TestFixtures.Add(new TestFixtureStubTwo());
            TestFixtures.Add(new TestFixtureStubThree());
            TestFixtures.Add(new TestFixtureStubFour());
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the test fixture stub for the assembly.
        /// </summary>
        /// <returns></returns>
        public static ITestFixtureStub GeTestFixtureStub()
        {
            return v_Singleton ?? (v_Singleton = new TestFixtureStubHelper());
        }

        /// <summary>
        ///     Counts the results depth based off of the type's namespace.
        /// </summary>
        /// <param name="type">The type of the class.</param>
        /// <returns>The depth of the result for the class.</returns>
        public static int CountResultsDepth(Type type)
        {
            string ns = type?.Namespace;
            if (string.IsNullOrEmpty(ns))
            {
                return 0;
            }

            // Result depth is number of segments in namespace = number of . plus one
            // plus one more for the result of the runner itself
            return ns.ToCharArray().Count(c => c == '.') + 2;
        }

        #endregion
    }
}