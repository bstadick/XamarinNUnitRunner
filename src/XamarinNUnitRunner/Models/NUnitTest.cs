using System;
using System.IO;
using NUnit.Framework.Interfaces;
using XamarinNUnitRunner.Filter;

namespace XamarinNUnitRunner.Models
{
    /// <summary>
    ///     Model for the data associated with a NUnit test.
    /// </summary>
    public class NUnitTest
    {
        #region Public Members

        /// <summary>
        ///     Gets or sets the underlying test.
        /// </summary>
        public ITest Test { get; set; }

        /// <summary>
        ///     Gets or sets the underlying test result.
        /// </summary>
        public ITestResult Result { get; set; }

        /// <summary>
        ///     Gets the display name for the test.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (Test?.FullName == null)
                {
                    return string.Empty;
                }

                return Test.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                    ? Path.GetFileNameWithoutExtension(Test.FullName)
                    : Test.FullName;
            }
        }

        /// <summary>
        ///     Gets the test filter to select tests under this test.
        /// </summary>
        public ITestFilter TestFilter =>
            Test == null ? NUnitFilter.Empty : NUnitFilter.Where.Id(Test.Id).Build().Filter;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="NUnitTest" />.
        /// </summary>
        /// <param name="test">The <see cref="ITest" /> instance to initialize to.</param>
        public NUnitTest(ITest test)
        {
            Test = test;
        }

        #endregion
    }
}