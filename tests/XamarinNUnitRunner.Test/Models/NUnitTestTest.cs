using System.IO;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using XamarinNUnitRunner.Filter;
using XamarinNUnitRunner.Models;

namespace XamarinNUnitRunner.Test.Models
{
    [TestFixture]
    public class NUnitTestTest
    {
        #region Tests for Constructor

        [Test]
        public void TestConstructorWithITest([Values] bool isNull)
        {
            ITest testFixture = isNull ? null : new TestSuite("suite-name");

            NUnitTest test = new NUnitTest(testFixture);

            if (isNull)
            {
                Assert.IsNull(test.Test);
            }
            else
            {
                Assert.AreSame(testFixture, test.Test);
            }

            Assert.IsNull(test.Result);
        }

        #endregion

        #region Tests for Test Property

        [Test]
        public void TestTestPropertyIsITestProvidedInConstructor([Values] bool isNull)
        {
            ITest testFixture = isNull ? null : new TestSuite("suite-name");

            NUnitTest test = new NUnitTest(testFixture);

            if (isNull)
            {
                Assert.IsNull(test.Test);
            }
            else
            {
                Assert.AreSame(testFixture, test.Test);
            }
        }

        [Test]
        public void TestTestPropertyCanBeSet([Values] bool isNull)
        {
            ITest initialTestFixture = !isNull ? null : new TestSuite("suite-name");
            ITest testFixture = isNull ? null : new TestSuite("suite-name");

            NUnitTest test = new NUnitTest(initialTestFixture);

            Assert.AreSame(initialTestFixture, test.Test);
            Assert.AreNotSame(testFixture, test.Test);

            if (isNull)
            {
                Assert.IsNotNull(test.Test);
                Assert.AreSame(initialTestFixture, test.Test);
            }
            else
            {
                Assert.IsNull(test.Test);
            }

            test.Test = testFixture;

            Assert.AreNotSame(initialTestFixture, test.Test);
            Assert.AreSame(testFixture, test.Test);

            if (isNull)
            {
                Assert.IsNull(test.Test);
            }
            else
            {
                Assert.IsNotNull(test.Test);
                Assert.AreSame(testFixture, test.Test);
            }
        }

        #endregion

        #region Tests for Result Property

        [Test]
        public void TestResultPropertyCanBeSet([Values] bool isNull)
        {
            ITestResult result = isNull ? null : new TestSuiteResult(new TestSuite("suite-name"));

            NUnitTest test = new NUnitTest(null);

            Assert.IsNull(test.Result);

            test.Result = result;

            Assert.AreSame(result, test.Result);
        }

        #endregion

        #region Tests for DisplayName Property

        [Test]
        public void TestDisplayNamePropertyWithTestNullReturnsEmptyString()
        {
            NUnitTest test = new NUnitTest(null);

            Assert.IsNull(test.Test);
            Assert.AreEqual(string.Empty, test.DisplayName);
        }

        [Test]
        public void TestDisplayNamePropertyWithTestFullNameAsDllFilePathReturnsFileNameWithoutExtension()
        {
            const string expected = "suite-name";
            string name = Path.Combine(TestContext.CurrentContext.WorkDirectory, expected + ".dll");
            ITest testFixture = new TestSuite(name);

            NUnitTest test = new NUnitTest(testFixture);

            Assert.IsNotNull(test.Test);
            Assert.AreEqual(expected, test.DisplayName);
        }

        [Test]
        public void TestDisplayNamePropertyWithTestFullNameNotADllFilePathReturnsFullName()
        {
            const string name = "suite-name";
            ITest testFixture = new TestSuite(name);

            NUnitTest test = new NUnitTest(testFixture);

            Assert.IsNotNull(test.Test);
            Assert.AreEqual(name, test.DisplayName);
        }

        #endregion

        #region Tests for TestFilter Property

        [Test]
        public void TestTestFilterPropertyWithTestNullReturnsEmptyFilter()
        {
            NUnitTest test = new NUnitTest(null);

            Assert.IsNull(test.Test);
            Assert.IsNotNull(test.TestFilter);
            Assert.AreSame(NUnitFilter.Empty, test.TestFilter);
        }

        [Test]
        public void TestTestFilterPropertyWithTestNotNullReturnsFilterOnTestId()
        {
            ITest testFixture = new TestSuite("suite-name");
            string expected = NUnitFilter.Where.Id(testFixture.Id).Build().Filter.ToXml(true).OuterXml;

            NUnitTest test = new NUnitTest(testFixture);

            Assert.IsNotNull(test.Test);
            Assert.IsNotNull(test.TestFilter);
            Assert.AreEqual(expected, test.TestFilter.ToXml(true).OuterXml);
        }

        #endregion
    }
}