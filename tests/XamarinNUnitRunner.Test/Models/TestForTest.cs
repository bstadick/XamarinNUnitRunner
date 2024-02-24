using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace XamarinNUnitRunner.Test.Models
{
    /// <summary>
    ///     Implementation of ITest for test.
    /// </summary>
    // ReSharper disable UnassignedGetOnlyAutoProperty
    public class TestForTest : ITest
    {
        #region Implementation if ITest

        /// <inheritdoc />
        public TNode ToXml(bool recursive)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TNode AddToXml(TNode parentNode, bool recursive)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string TestType { get; }

        /// <inheritdoc />
        public string FullName { get; set; }

        /// <inheritdoc />
        public string ClassName { get; }

        /// <inheritdoc />
        public string MethodName { get; }

        /// <inheritdoc />
        public ITypeInfo TypeInfo { get; }

        /// <inheritdoc />
        public IMethodInfo Method { get; }

        /// <inheritdoc />
        public RunState RunState { get; }

        /// <inheritdoc />
        public int TestCaseCount { get; }

        /// <inheritdoc />
        public IPropertyBag Properties { get; }

        /// <inheritdoc />
        public ITest Parent { get; }

        /// <inheritdoc />
        public bool IsSuite { get; }

        /// <inheritdoc />
        public bool HasChildren { get; }

        /// <inheritdoc />
        public IList<ITest> Tests { get; }

        /// <inheritdoc />
        public object Fixture { get; }

        /// <inheritdoc />
        public object[] Arguments { get; }

        #endregion
    }
    // ReSharper restore UnassignedGetOnlyAutoProperty
}
