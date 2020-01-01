using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using XamarinNUnitRunner.Filter;

namespace XamarinNUnitRunner.Test.Filter
{
    [TestFixture]
    public class NUnitFilterTest
    {
        // Tests for FilterXmlString and Filter Properties are covered by Build method tests.

        #region Tests for Empty Property

        [Test]
        public void TestEmptyPropertyReturnsNewEmptyRootFilterInstance()
        {
            const string expectedFilter = "<filter />";

            ITestFilter firstWhere = NUnitFilter.Empty;

            Assert.IsNotNull(firstWhere);
            Assert.AreEqual(expectedFilter, GetXmlString(firstWhere, false));

            ITestFilter secondWhere = NUnitFilter.Empty;

            Assert.IsNotNull(secondWhere);
            Assert.AreEqual(expectedFilter, GetXmlString(secondWhere, false));

            Assert.AreSame(firstWhere, secondWhere);
        }

        #endregion

        #region Tests for Where Property

        [Test]
        public void TestWherePropertyReturnsNewRootFilterInstanceForEachCall()
        {
            INUnitFilterElementCollectionInternal
                firstWhere = (INUnitFilterElementCollectionInternal) NUnitFilter.Where;

            Assert.IsNotNull(firstWhere);
            Assert.IsNull(firstWhere.Parent);
            Assert.IsNull(firstWhere.Child);
            Assert.AreEqual(NUnitElementType.RootFilter, firstWhere.ElementType);

            INUnitFilterElementCollectionInternal secondWhere =
                (INUnitFilterElementCollectionInternal) NUnitFilter.Where;

            Assert.IsNotNull(secondWhere);
            Assert.IsNull(secondWhere.Parent);
            Assert.IsNull(secondWhere.Child);
            Assert.AreEqual(NUnitElementType.RootFilter, secondWhere.ElementType);

            Assert.AreNotSame(firstWhere, secondWhere);
        }

        #endregion

        #region Tests for Build

        [Test]
        public void TestBuildThrowsArgumentExceptionWhenLeafElementChildIsNotNull()
        {
            const string value = "Value_";
            const string xmlTag = "name_";
            NUnitFilterContainerElementForTest root =
                new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);
            XmlSerializableElementForTest leafChild =
                new XmlSerializableElementForTest(xmlTag + 1, value + 1, NUnitElementType.Test);

            root.SetChild(leafChild);

            Assert.IsNotNull(root.Child);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo(
                        "The leaf element's child is not null thus the provided leaf element is not the true leaf element." +
                        " This may indicate an error in the construction or parsing of the filter. (Parameter 'leafElement')"),
                () => NUnitFilter.Build(root));
        }

        [Test]
        public void TestBuildThrowsArgumentNullExceptionWhenLeafElementIsNull()
        {
            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The leafElement cannot be null. (Parameter 'leafElement')"),
                () => NUnitFilter.Build(null));
        }

        [Test]
        public void TestBuildThrowsArgumentOutOfRangeExceptionWhenTheElementIsNotASupportedType()
        {
            const string value = "Value_";
            const string xmlTag = "name_";
            NUnitFilterContainerElementForTest root =
                new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);
            XmlSerializableElementForTest leafChild =
                new XmlSerializableElementForTest(xmlTag + 1, value + 1, (NUnitElementType) (-1));

            leafChild.Parent = root;
            root.SetChild(leafChild);

            Assert.IsNull(root.Parent);
            Assert.IsNotNull(root.Child);
            Assert.AreSame(root, leafChild.Parent);
            Assert.AreSame(leafChild, root.Child);

            const string currentString =
                "{XmlSerializableElementForTest: {Type: -1, Parent: {NUnitFilterContainerElementForTest: {Type: RootFilter}}, Child: Null}}";
            Assert.Throws(
                Is.TypeOf<ArgumentOutOfRangeException>().And.Message
                    .EqualTo(
                        $"The given element type is not supported. {currentString}" +
                        $" (Parameter 'ElementType'){Environment.NewLine}Actual value was -1."),
                () => NUnitFilter.Build(leafChild));
        }

        [Test]
        public void TestBuildThrowsInvalidOperationExceptionWhenAParentAndChildReferenceAreNotEqual()
        {
            const string value = "Value_";
            const string xmlTag = "name_";
            NUnitFilterContainerElementForTest root =
                new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);
            XmlSerializableElementForTest wrongChild =
                new XmlSerializableElementForTest(xmlTag + 1, value + 1, NUnitElementType.Test);
            wrongChild.Parent = root;
            XmlSerializableElementForTest leafChild =
                new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Id);

            leafChild.Parent = root;
            root.SetChild(wrongChild);

            Assert.AreSame(root, leafChild.Parent);
            Assert.AreNotSame(leafChild, root.Child);

            const string parentString =
                "{NUnitFilterContainerElementForTest: {Type: RootFilter, Parent: Null, Child: {XmlSerializableElementForTest: {Type: Test}}}}";
            Assert.Throws(
                Is.TypeOf<InvalidOperationException>().And.Message
                    .EqualTo(
                        $"The parent element's {parentString} child was not the same reference as the current node." +
                        " Forward traversal will not proceed properly." +
                        " This may indicate an error in the construction or parsing of the filter."),
                () => NUnitFilter.Build(leafChild));
        }

        [Test]
        public void TestBuildThrowsInvalidOperationExceptionWhenAParentElementChildIsNull()
        {
            const string value = "Value_";
            const string xmlTag = "name_";
            NUnitFilterContainerElementForTest root =
                new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);
            XmlSerializableElementForTest leafChild =
                new XmlSerializableElementForTest(xmlTag + 1, value + 1, NUnitElementType.Test);

            leafChild.Parent = root;

            Assert.AreSame(root, leafChild.Parent);
            Assert.IsNull(root.Child);

            const string parentString =
                "{NUnitFilterContainerElementForTest: {Type: RootFilter, Parent: Null, Child: Null}}";
            Assert.Throws(
                Is.TypeOf<InvalidOperationException>().And.Message
                    .EqualTo($"The parent element's {parentString} child was null." +
                             " Forward traversal will not proceed properly." +
                             " This may indicate an error in the construction or parsing of the filter."),
                () => NUnitFilter.Build(leafChild));
        }

        [Test]
        public void TestBuildThrowsInvalidOperationExceptionWhenTheRootIsNotOfTypeRootElement()
        {
            const string value = "Value_";
            const string xmlTag = "name_";
            XmlSerializableElementForTest root =
                new XmlSerializableElementForTest(xmlTag + 1, value + 1, NUnitElementType.Test);
            XmlSerializableElementForTest leafChild =
                new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Id);

            leafChild.Parent = root;
            root.Child = leafChild;

            Assert.AreNotEqual(NUnitElementType.RootFilter, root.ElementType);
            Assert.IsNull(root.Parent);
            Assert.IsNotNull(root.Child);
            Assert.AreSame(root, leafChild.Parent);
            Assert.AreSame(leafChild, root.Child);

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>().And.Message
                    .EqualTo(
                        "The root element type was not the expected type of RootFilter but was instead Test." +
                        " Forward traversal will not proceed properly." +
                        " This may indicate an error in the construction or parsing of the filter."),
                () => NUnitFilter.Build(leafChild));
        }

        [Test]
        public void TestBuildWithBothAndPlusOrPlusFilterElements()
        {
            const string value = "Value_";
            NUnitFilterContainerElementForTest root =
                new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);
            XmlSerializableElementForTest child1 =
                new XmlSerializableElementForTest("name", value + 1, NUnitElementType.Test);
            XmlSerializableElementForTest or1 =
                new XmlSerializableElementForTest(null, null, NUnitElementType.Or);
            XmlSerializableElementForTest child2 =
                new XmlSerializableElementForTest("class", value + 2, NUnitElementType.Test);
            XmlSerializableElementForTest or2 =
                new XmlSerializableElementForTest(null, null, NUnitElementType.Or);
            XmlSerializableElementForTest child3 =
                new XmlSerializableElementForTest("namespace", value + 3, NUnitElementType.Test);
            XmlSerializableElementForTest and3 =
                new XmlSerializableElementForTest(null, null, NUnitElementType.And);
            XmlSerializableElementForTest child4 =
                new XmlSerializableElementForTest("cat", value + 4, NUnitElementType.Test);
            XmlSerializableElementForTest and4 =
                new XmlSerializableElementForTest(null, null, NUnitElementType.And);
            XmlSerializableElementForTest child5 =
                new XmlSerializableElementForTest("method", value + 5, NUnitElementType.Test);
            XmlSerializableElementForTest or5 =
                new XmlSerializableElementForTest(null, null, NUnitElementType.Or);
            XmlSerializableElementForTest child6 =
                new XmlSerializableElementForTest("test", value + 6, NUnitElementType.Test);

            root.SetChild(child1);
            child1.Parent = root;
            child1.Child = or1;
            or1.Parent = child1;
            or1.Child = child2;
            child2.Parent = or1;
            child2.Child = or2;
            or2.Parent = child2;
            or2.Child = child3;
            child3.Parent = or2;
            child3.Child = and3;
            and3.Parent = child3;
            and3.Child = child4;
            child4.Parent = and3;
            child4.Child = and4;
            and4.Parent = child4;
            and4.Child = child5;
            child5.Parent = and4;
            child5.Child = or5;
            or5.Parent = child5;
            or5.Child = child6;
            child6.Parent = or5;

            const string expected =
                "<filter><or><name>Value_1</name><class>Value_2</class><and><namespace>Value_3</namespace><cat>Value_4</cat><method>Value_5</method></and><test>Value_6</test></or></filter>";

            NUnitFilter filter = NUnitFilter.Build(child6);

            Assert.IsNotNull(filter);
            Assert.AreEqual(expected, filter.FilterXmlString);
            Assert.AreEqual(expected, GetXmlString(filter.Filter));
        }

        [Test]
        public void TestBuildWithOnlyAndPlusFilterElements()
        {
            const string value = "Value_";
            NUnitFilterContainerElementForTest root =
                new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);
            XmlSerializableElementForTest child1 =
                new XmlSerializableElementForTest("name", value + 1, NUnitElementType.Test);
            XmlSerializableElementForTest and1 =
                new XmlSerializableElementForTest(null, null, NUnitElementType.And);
            XmlSerializableElementForTest child2 =
                new XmlSerializableElementForTest("class", value + 2, NUnitElementType.Test);
            XmlSerializableElementForTest and2 =
                new XmlSerializableElementForTest(null, null, NUnitElementType.And);
            XmlSerializableElementForTest child3 =
                new XmlSerializableElementForTest("namespace", value + 3, NUnitElementType.Test);
            XmlSerializableElementForTest and3 =
                new XmlSerializableElementForTest(null, null, NUnitElementType.And);
            XmlSerializableElementForTest child4 =
                new XmlSerializableElementForTest("cat", value + 4, NUnitElementType.Test);

            root.SetChild(child1);
            child1.Parent = root;
            child1.Child = and1;
            and1.Parent = child1;
            and1.Child = child2;
            child2.Parent = and1;
            child2.Child = and2;
            and2.Parent = child2;
            and2.Child = child3;
            child3.Parent = and2;
            child3.Child = and3;
            and3.Parent = child3;
            and3.Child = child4;
            child4.Parent = and3;

            const string expectedInner =
                "<name>Value_1</name><class>Value_2</class><namespace>Value_3</namespace><cat>Value_4</cat>";
            string expected = $"<filter>{expectedInner}</filter>";
            string expectedFilter = $"<filter><and>{expectedInner}</and></filter>";

            NUnitFilter filter = NUnitFilter.Build(child4);

            Assert.IsNotNull(filter);
            Assert.AreEqual(expected, filter.FilterXmlString);
            Assert.AreEqual(expectedFilter, GetXmlString(filter.Filter));
        }

        [Test]
        public void TestBuildWithOnlyANotPlusFilterElement(
            [ValueSource(typeof(NUnitFilterTestHelper), nameof(NUnitFilterTestHelper.GetFilterElements))]
            NUnitElementType elementType)
        {
            const string value = "Value_";
            NUnitFilterContainerElementForTest root =
                new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);
            XmlSerializableElementForTest child1 =
                new XmlSerializableElementForTest("name", value + 1, NUnitElementType.Not);
            XmlSerializableElementForTest child2 =
                new XmlSerializableElementForTest("class", value + 2, elementType);

            root.SetChild(child1);
            child1.Parent = root;
            child1.Child = child2;
            child2.Parent = child1;

            // Not will skip its child element
            const string expected = "<filter><name>Value_1</name></filter>";

            NUnitFilter filter = NUnitFilter.Build(child2);

            Assert.IsNotNull(filter);
            Assert.AreEqual(expected, filter.FilterXmlString);
            Assert.AreEqual(expected, GetXmlString(filter.Filter));
        }

        [Test]
        public void TestBuildWithOnlyOneFilterElement(
            [ValueSource(typeof(NUnitFilterTestHelper), nameof(NUnitFilterTestHelper.GetFilterElements))]
            NUnitElementType elementType)
        {
            const string value = "Value_";
            NUnitFilterContainerElementForTest root =
                new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);
            XmlSerializableElementForTest child1 =
                new XmlSerializableElementForTest("name", value + 1, elementType);

            root.SetChild(child1);
            child1.Parent = root;

            const string expected = "<filter><name>Value_1</name></filter>";

            NUnitFilter filter = NUnitFilter.Build(child1);

            Assert.IsNotNull(filter);
            Assert.AreEqual(expected, filter.FilterXmlString);
            Assert.AreEqual(expected, GetXmlString(filter.Filter));
        }

        [Test]
        public void TestBuildWithOnlyOrPlusFilterElements()
        {
            const string value = "Value_";
            NUnitFilterContainerElementForTest root =
                new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);
            XmlSerializableElementForTest child1 =
                new XmlSerializableElementForTest("name", value + 1, NUnitElementType.Test);
            XmlSerializableElementForTest or1 =
                new XmlSerializableElementForTest(null, null, NUnitElementType.Or);
            XmlSerializableElementForTest child2 =
                new XmlSerializableElementForTest("class", value + 2, NUnitElementType.Test);
            XmlSerializableElementForTest or2 =
                new XmlSerializableElementForTest(null, null, NUnitElementType.Or);
            XmlSerializableElementForTest child3 =
                new XmlSerializableElementForTest("namespace", value + 3, NUnitElementType.Test);
            XmlSerializableElementForTest or3 =
                new XmlSerializableElementForTest(null, null, NUnitElementType.Or);
            XmlSerializableElementForTest child4 =
                new XmlSerializableElementForTest("cat", value + 4, NUnitElementType.Test);

            root.SetChild(child1);
            child1.Parent = root;
            child1.Child = or1;
            or1.Parent = child1;
            or1.Child = child2;
            child2.Parent = or1;
            child2.Child = or2;
            or2.Parent = child2;
            or2.Child = child3;
            child3.Parent = or2;
            child3.Child = or3;
            or3.Parent = child3;
            or3.Child = child4;
            child4.Parent = or3;

            const string expected =
                "<filter><or><name>Value_1</name><class>Value_2</class><namespace>Value_3</namespace><cat>Value_4</cat></or></filter>";

            NUnitFilter filter = NUnitFilter.Build(child4);

            Assert.IsNotNull(filter);
            Assert.AreEqual(expected, filter.FilterXmlString);
            Assert.AreEqual(expected, GetXmlString(filter.Filter));
        }

        [Test]
        public void TestBuildWithOnlyTheRootElement()
        {
            NUnitFilterContainerElementForTest root =
                new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);

            const string expected = "<filter></filter>";
            const string expectedFilter = "<filter />";

            NUnitFilter filter = NUnitFilter.Build(root);

            Assert.IsNotNull(filter);
            Assert.AreEqual(expected, filter.FilterXmlString);
            Assert.AreEqual(expectedFilter, GetXmlString(filter.Filter, false));
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the Xml string representation of the ITestFilter instance.
        /// </summary>
        /// <param name="filter">The test filter to get the Xml string of.</param>
        /// <param name="withOuterTags">If to add the outer filter tags to the xml string.</param>
        /// <returns>The Xml string representation of the ITestFilter instance.</returns>
        private static string GetXmlString(ITestFilter filter, bool withOuterTags = true)
        {
            string outerXml = filter.ToXml(true).OuterXml;
            return withOuterTags ? $"<filter>{outerXml}</filter>" : outerXml;
        }

        #endregion
    }
}