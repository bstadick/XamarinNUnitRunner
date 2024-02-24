using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.Tests.Droid1
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestPass()
        {
            Assert.Pass("Your first passing test");
        }

        [Test]
        public void TestFail()
        {
            Assert.Fail("Your first failing test");
        }

        [Test]
        public void TestCombinatorial([Values] bool value)
        {
            Assert.Pass("Your first combinatorial test: " + value);
        }
    }
}
