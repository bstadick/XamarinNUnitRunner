using XamarinNUnitRunner.Models;
using XamarinNUnitRunner.Services;

namespace XamarinNUnitRunner.Test.Services
{
    /// <summary>
    ///     Implements a NUnitRunner for tests.
    /// </summary>
    public class NUnitRunnerForTest : NUnitRunner
    {
        #region Constructors

        /// <inheritdoc />
        public NUnitRunnerForTest(NUnitSuite suite) : base(suite)
        {
        }

        #endregion
    }
}