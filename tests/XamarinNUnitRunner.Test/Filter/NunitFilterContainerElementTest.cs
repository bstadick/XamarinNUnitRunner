using System;
using NUnit.Framework;
using XamarinNUnitRunner.Filter;

namespace XamarinNUnitRunner.Test.Filter
{
    [TestFixture]
    public class NUnitFilterContainerElementTest
    {
        #region Tests for Constructor

        [Test]
        public void
            TestConstructorThrowsArgumentExceptionWhenParentNotNullAndElementTypeRootFilter()
        {
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message.EqualTo(
                    "The parent element cannot be non-null if the element type is RootFilter. (Parameter 'parent')"),
                // ReSharper disable once ObjectCreationAsStatement
                () => new NUnitFilterContainerElement(parent, NUnitElementType.RootFilter));
        }

        [Test]
        public void
            TestConstructorThrowsArgumentNullExceptionWhenParentNullAndElementTypeNotRootFilter()
        {
            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The parent cannot be null. (Parameter 'parent')"),
                // ReSharper disable once ObjectCreationAsStatement
                () => new NUnitFilterContainerElement(null, NUnitElementType.And));
        }

        [Test]
        public void
            TestConstructorThrowsArgumentOutOfRangeExceptionWhenElementTypeNotSupported(
                [ValueSource(typeof(NUnitFilterTestHelper), nameof(NUnitFilterTestHelper.GetFilterElements))]
                NUnitElementType elementType)
        {
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            Assert.Throws(
                Is.TypeOf<ArgumentOutOfRangeException>().And.Message
                    .EqualTo(
                        $"The given element type is not supported. (Parameter 'elementType'){Environment.NewLine}" +
                        $"Actual value was {elementType}."),
                // ReSharper disable once ObjectCreationAsStatement
                () => new NUnitFilterContainerElement(parent, elementType));
        }

        [Test]
        [TestCase(NUnitElementType.And)]
        [TestCase(NUnitElementType.Or)]
        [TestCase(NUnitElementType.Not)]
        public void TestConstructorWithParentNotNullAndElementTypeSupportedButNotRootFilter(
            NUnitElementType elementType)
        {
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            string expectedXmlTag = null;
            switch (elementType)
            {
                // NUnitElementType.RootFilter covered in a dedicated test case
                case NUnitElementType.And:
                    expectedXmlTag = NUnitFilterTestHelper.XmlAndTag;
                    break;
                case NUnitElementType.Or:
                    expectedXmlTag = NUnitFilterTestHelper.XmlOrTag;
                    break;
                case NUnitElementType.Not:
                    expectedXmlTag = NUnitFilterTestHelper.XmlNotTag;
                    break;
                default:
                    Assert.Fail($"The type {elementType} is not supported for this test.");
                    break;
            }

            INUnitFilterContainerElementInternal element = new NUnitFilterContainerElement(parent, elementType);

            Assert.AreSame(parent, element.Parent);
            Assert.IsNull(element.Child);
            Assert.AreEqual(elementType, element.ElementType);
            Assert.AreEqual(expectedXmlTag, element.XmlTag);
        }

        [Test]
        public void TestConstructorWithParentNullAndElementTypeRootFilter()
        {
            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

            Assert.IsNull(element.Parent);
            Assert.IsNull(element.Child);
            Assert.AreEqual(NUnitElementType.RootFilter, element.ElementType);
            Assert.AreEqual(NUnitFilterTestHelper.XmlFilterTag, element.XmlTag);
        }

        #endregion

        #region Tests for ToString

        [Test]
        public void TestToStringReturnsStringRepresentation([Values] bool isParentNull,
            [Values] bool isChildNull)
        {
            // Create expected string of xml nodes
            const string valueParent = "Value_1";
            const string xmlTagParent = "name_1";
            XmlSerializableElementForTest parent = isParentNull
                ? null
                : new XmlSerializableElementForTest(xmlTagParent, valueParent, NUnitElementType.Test);

            const string valueChild = "Value_2";
            const string xmlTagChild = "name_2";
            XmlSerializableElementForTest child =
                new XmlSerializableElementForTest(xmlTagChild, valueChild, NUnitElementType.Id);

            NUnitElementType elementType = isParentNull ? NUnitElementType.RootFilter : NUnitElementType.And;
            string parentString = isParentNull ? "Null" : "{XmlSerializableElementForTest: {Type: Test}}";
            string childString = isChildNull ? "Null" : "{XmlSerializableElementForTest: {Type: Id}}";
            string expected =
                $"{{NUnitFilterContainerElementForTest: {{Type: {elementType}, Parent: {parentString}, Child: {childString}}}}}";

            NUnitFilterContainerElementForTest element =
                new NUnitFilterContainerElementForTest(parent, elementType);
            if (!isChildNull)
            {
                element.SetChild(child);
            }

            string actual = element.ToString();

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Tests for Parent Property

        [Test]
        public void TestParentPropertyReturnsParentProvidedWithConstructor(
            [Values] bool isNull)
        {
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                isNull ? null : new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);
            NUnitElementType elementType = isNull ? NUnitElementType.RootFilter : NUnitElementType.And;

            INUnitFilterContainerElementInternal element = new NUnitFilterContainerElement(parent, elementType);

            if (isNull)
            {
                Assert.IsNull(element.Parent);
            }
            else
            {
                Assert.AreSame(parent, element.Parent);
            }
        }

        #endregion

        #region Tests for Child Property

        [Test]
        public void TestChildPropertyReturnsSetChild()
        {
            const string value = "Value_";
            const string xmlTag = "name_";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag + 1, value + 1, NUnitElementType.Test);
            XmlSerializableElementForTest child =
                new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Test);

            NUnitFilterContainerElementForTest element =
                new NUnitFilterContainerElementForTest(parent, NUnitElementType.And);

            Assert.IsNull(element.Child);

            element.SetChild(child);

            Assert.AreSame(child, element.Child);
        }

        [Test]
        public void TestChildPropertyThrowsArgumentNullExceptionWhenChildSetToNull()
        {
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            NUnitFilterContainerElementForTest element =
                new NUnitFilterContainerElementForTest(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The value cannot be null. (Parameter 'value')"), () => element.SetChild(null));
        }

        #endregion

        #region Tests for ElementType Property

        [Test]
        public void TestElementTypePropertyReturnsElementTypeProvidedWithConstructor()
        {
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.AreEqual(NUnitElementType.And, element.ElementType);
        }

        #endregion

        #region Tests for XmlTag Property

        [Test]
        public void TestXmlTagPropertyReturnsXmlTagProvidedWithConstructor()
        {
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.AreEqual(NUnitFilterTestHelper.XmlAndTag, element.XmlTag);
        }

        #endregion

        #region Tests for ToXmlString

        [Test]
        public void TestToXmlStringThrowsNullArgumentExceptionWhenChildNull(
            [Values] bool withXmlTag)
        {
            const string valueParent = "Value_1";
            const string xmlTagParent = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTagParent, valueParent, NUnitElementType.Test);

            NUnitFilterContainerElementForTest element =
                new NUnitFilterContainerElementForTest(parent, NUnitElementType.And);

            Assert.IsNull(element.Child);

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The Child cannot be null. (Parameter 'Child')"),
                () => element.ToXmlString(withXmlTag));
        }

        [Test]
        public void
            TestToXmlStringWithParentNotNullAndElementTypeNotNotReturnsParentAndChildXmlStrings(
                [Values] bool withXmlTag)
        {
            // Create expected string of xml nodes
            const string valueParent = "Value_1";
            const string xmlTagParent = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTagParent, valueParent, NUnitElementType.Test);
            string expectedValueParent = NUnitFilterTestHelper.CreateXmlNode(xmlTagParent, valueParent);

            const string valueChild = "Value_2";
            const string xmlTagChild = "name_2";
            XmlSerializableElementForTest child =
                new XmlSerializableElementForTest(xmlTagChild, valueChild, NUnitElementType.Test);
            string expectedValueChild = NUnitFilterTestHelper.CreateXmlNode(xmlTagChild, valueChild);

            // With tag includes parent xml tag, without is just value
            string valueParentAndChild = expectedValueParent + expectedValueChild;
            string expected = withXmlTag
                ? NUnitFilterTestHelper.CreateXmlNode(NUnitFilterTestHelper.XmlAndTag, valueParentAndChild)
                : valueParentAndChild;

            NUnitFilterContainerElementForTest element =
                new NUnitFilterContainerElementForTest(parent, NUnitElementType.And);
            element.SetChild(child);

            string actual = element.ToXmlString(withXmlTag);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestToXmlStringWithParentNotNullAndElementTypeNotReturnsChildXmlStrings(
            [Values] bool withXmlTag)
        {
            const string valueParent = "Value_1";
            const string xmlTagParent = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTagParent, valueParent, NUnitElementType.Test);

            const string valueChild = "Value_2";
            const string xmlTagChild = "name_2";
            XmlSerializableElementForTest child =
                new XmlSerializableElementForTest(xmlTagChild, valueChild, NUnitElementType.Test);
            string expectedValueChild = NUnitFilterTestHelper.CreateXmlNode(xmlTagChild, valueChild);

            // With tag includes parent xml tag, without is just value
            string expected = withXmlTag
                ? NUnitFilterTestHelper.CreateXmlNode(NUnitFilterTestHelper.XmlNotTag, expectedValueChild)
                : expectedValueChild;

            NUnitFilterContainerElementForTest element =
                new NUnitFilterContainerElementForTest(parent, NUnitElementType.Not);
            element.SetChild(child);

            string actual = element.ToXmlString(withXmlTag);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestToXmlStringWithParentNullAndElementTypeNotNotReturnsChildXmlString(
            [Values] bool withXmlTag)
        {
            // Create expected string of xml nodes
            const string valueChild = "Value_1";
            const string xmlTagChild = "name_1";
            XmlSerializableElementForTest child =
                new XmlSerializableElementForTest(xmlTagChild, valueChild, NUnitElementType.Test);
            string expectedValueChild = NUnitFilterTestHelper.CreateXmlNode(xmlTagChild, valueChild);

            // With tag includes parent xml tag, without is just value
            string expected = withXmlTag
                ? NUnitFilterTestHelper.CreateXmlNode(NUnitFilterTestHelper.XmlFilterTag, expectedValueChild)
                : expectedValueChild;

            NUnitFilterContainerElementForTest element =
                new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);
            element.SetChild(child);

            string actual = element.ToXmlString(withXmlTag);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Tests for Id

        [Test]
        public void
            TestIdThrowsArgumentExceptionWhenMultipleTestIdsContainsOnlyNullOrEmptyValues()
        {
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement id = element.Id(null, string.Empty);
                });
        }

        [Test]
        public void TestIdThrowsArgumentExceptionWhenSingleTestIdNullOrEmpty(
            [Values] bool isNull)
        {
            string name = isNull ? null : string.Empty;
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement id = element.Id(name);
                });
        }

        [Test]
        public void
            TestIdThrowsArgumentExceptionWhenTestIdArrayContainsOnlyNullOrEmptyValues()
        {
            string[] names = {null, string.Empty};
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement id = element.Id(names);
                });
        }

        [Test]
        public void TestIdThrowsArgumentExceptionWhenTestIdArrayNullOrEmpty(
            [Values] bool isNull)
        {
            string[] names = isNull ? null : Array.Empty<string>();
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The testIds cannot be null or empty. (Parameter 'testIds')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement id = element.Id(names);
                });
        }

        [Test]
        public void TestIdThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
        {
            TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement id = e.Id(s);
            });
        }

        [Test]
        public void TestIdWithMultipleTestIdsSetsChildAndReturnsNewAndElementWithParent()
        {
            const string name1 = "name_1";
            const string name2 = "name_2";
            const string name3 = "name_3";
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);
            const string expectedName = name1 + "," + name2 + "," + name3;

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            INUnitFilterElementInternal id =
                (INUnitFilterElementInternal) element.Id(name1, null, name2, string.Empty, name3);

            Assert.IsNotNull(id);
            Assert.AreEqual(NUnitElementType.Id, id.ElementType);
            Assert.AreSame(element, id.Parent);
            Assert.AreSame(id, element.Child);
            Assert.AreEqual(expectedName, id.ElementName);
            Assert.IsNull(id.ElementValue);
            Assert.IsFalse(id.IsRegularExpression);
        }

        [Test]
        public void TestIdWithSingleTestIdSetsChildAndReturnsNewAndElementWithParent()
        {
            const string name = "name_1";
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            INUnitFilterElementInternal id = (INUnitFilterElementInternal) element.Id(name);

            Assert.IsNotNull(id);
            Assert.AreEqual(NUnitElementType.Id, id.ElementType);
            Assert.AreSame(element, id.Parent);
            Assert.AreSame(id, element.Child);
            Assert.AreEqual(name, id.ElementName);
            Assert.IsNull(id.ElementValue);
            Assert.IsFalse(id.IsRegularExpression);
        }

        [Test]
        public void TestIdWithTestIdArraySetsChildAndReturnsNewAndElementWithParent()
        {
            const string name1 = "name_1";
            const string name2 = "name_2";
            const string name3 = "name_3";
            string[] names = {name1, null, name2, string.Empty, name3};
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);
            const string expectedName = name1 + "," + name2 + "," + name3;

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            INUnitFilterElementInternal id = (INUnitFilterElementInternal) element.Id(names);

            Assert.IsNotNull(id);
            Assert.AreEqual(NUnitElementType.Id, id.ElementType);
            Assert.AreSame(element, id.Parent);
            Assert.AreSame(id, element.Child);
            Assert.AreEqual(expectedName, id.ElementName);
            Assert.IsNull(id.ElementValue);
            Assert.IsFalse(id.IsRegularExpression);
        }

        #endregion

        #region Tests for Test

        [Test]
        public void TestTestSetsChildAndReturnsNewAndElementWithParent(
            [Values] bool isRegularExpression)
        {
            const string name = "name_1";
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            INUnitFilterElementInternal test = (INUnitFilterElementInternal) element.Test(name, isRegularExpression);

            Assert.IsNotNull(test);
            Assert.AreEqual(NUnitElementType.Test, test.ElementType);
            Assert.AreSame(element, test.Parent);
            Assert.AreSame(test, element.Child);
            Assert.AreEqual(name, test.ElementName);
            Assert.IsNull(test.ElementValue);
            Assert.AreEqual(isRegularExpression, test.IsRegularExpression);
        }

        [Test]
        public void TestTestThrowsArgumentExceptionWhenTestNameIsNullOrEmpty(
            [Values] bool isNull)
        {
            string name = isNull ? null : string.Empty;
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement test = element.Test(name);
                });
        }

        [Test]
        public void TestTestThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
        {
            TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement test = e.Test(s, b);
            });
        }

        #endregion

        #region Tests for Category

        [Test]
        public void TestCategorySetsChildAndReturnsNewAndElementWithParent(
            [Values] bool isRegularExpression)
        {
            const string name = "name_1";
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            INUnitFilterElementInternal category =
                (INUnitFilterElementInternal) element.Category(name, isRegularExpression);

            Assert.IsNotNull(category);
            Assert.AreEqual(NUnitElementType.Category, category.ElementType);
            Assert.AreSame(element, category.Parent);
            Assert.AreSame(category, element.Child);
            Assert.AreEqual(name, category.ElementName);
            Assert.IsNull(category.ElementValue);
            Assert.AreEqual(isRegularExpression, category.IsRegularExpression);
        }

        [Test]
        public void TestCategoryThrowsArgumentExceptionWhenCategoryNameIsNullOrEmpty(
            [Values] bool isNull)
        {
            string name = isNull ? null : string.Empty;
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement category = element.Category(name);
                });
        }

        [Test]
        public void TestCategoryThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
        {
            TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement category = e.Category(s, b);
            });
        }

        #endregion

        #region Tests for Class

        [Test]
        public void TestClassSetsChildAndReturnsNewAndElementWithParent(
            [Values] bool isRegularExpression)
        {
            const string name = "name_1";
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            INUnitFilterElementInternal classElement =
                (INUnitFilterElementInternal) element.Class(name, isRegularExpression);

            Assert.IsNotNull(classElement);
            Assert.AreEqual(NUnitElementType.Class, classElement.ElementType);
            Assert.AreSame(element, classElement.Parent);
            Assert.AreSame(classElement, element.Child);
            Assert.AreEqual(name, classElement.ElementName);
            Assert.IsNull(classElement.ElementValue);
            Assert.AreEqual(isRegularExpression, classElement.IsRegularExpression);
        }

        [Test]
        public void TestClassThrowsArgumentExceptionWhenClassNameIsNullOrEmpty(
            [Values] bool isNull)
        {
            string name = isNull ? null : string.Empty;
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement classElement = element.Class(name);
                });
        }

        [Test]
        public void TestClassThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
        {
            TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement classElement = e.Class(s, b);
            });
        }

        #endregion

        #region Tests for Method

        [Test]
        public void TestMethodSetsChildAndReturnsNewAndElementWithParent(
            [Values] bool isRegularExpression)
        {
            const string name = "name_1";
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            INUnitFilterElementInternal
                method = (INUnitFilterElementInternal) element.Method(name, isRegularExpression);

            Assert.IsNotNull(method);
            Assert.AreEqual(NUnitElementType.Method, method.ElementType);
            Assert.AreSame(element, method.Parent);
            Assert.AreSame(method, element.Child);
            Assert.AreEqual(name, method.ElementName);
            Assert.IsNull(method.ElementValue);
            Assert.AreEqual(isRegularExpression, method.IsRegularExpression);
        }

        [Test]
        public void TestMethodThrowsArgumentExceptionWhenMethodNameIsNullOrEmpty(
            [Values] bool isNull)
        {
            string name = isNull ? null : string.Empty;
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement method = element.Method(name);
                });
        }

        [Test]
        public void TestMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
        {
            TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement method = e.Method(s, b);
            });
        }

        [Test]
        public void TestNameSetsChildAndReturnsNewAndElementWithParent(
            [Values] bool isRegularExpression)
        {
            const string name = "name_1";
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            INUnitFilterElementInternal nameElement =
                (INUnitFilterElementInternal) element.Name(name, isRegularExpression);

            Assert.IsNotNull(nameElement);
            Assert.AreEqual(NUnitElementType.NUnitName, nameElement.ElementType);
            Assert.AreSame(element, nameElement.Parent);
            Assert.AreSame(nameElement, element.Child);
            Assert.AreEqual(name, nameElement.ElementName);
            Assert.IsNull(nameElement.ElementValue);
            Assert.AreEqual(isRegularExpression, nameElement.IsRegularExpression);
        }

        #endregion

        #region Tests for Namespace

        [Test]
        public void TestNamespaceSetsChildAndReturnsNewAndElementWithParent(
            [Values] bool isRegularExpression)
        {
            const string name = "name_1";
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            INUnitFilterElementInternal namespaceElement =
                (INUnitFilterElementInternal) element.Namespace(name, isRegularExpression);

            Assert.IsNotNull(namespaceElement);
            Assert.AreEqual(NUnitElementType.Namespace, namespaceElement.ElementType);
            Assert.AreSame(element, namespaceElement.Parent);
            Assert.AreSame(namespaceElement, element.Child);
            Assert.AreEqual(name, namespaceElement.ElementName);
            Assert.IsNull(namespaceElement.ElementValue);
            Assert.AreEqual(isRegularExpression, namespaceElement.IsRegularExpression);
        }

        [Test]
        public void TestNamespaceThrowsArgumentExceptionWhenNamespaceNameIsNullOrEmpty(
            [Values] bool isNull)
        {
            string name = isNull ? null : string.Empty;
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement namespaceElement = element.Namespace(name);
                });
        }

        [Test]
        public void TestNamespaceThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
        {
            TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement namespaceElement = e.Namespace(s, b);
            });
        }

        #endregion

        #region Tests for Property

        [Test]
        public void TestPropertySetsChildAndReturnsNewAndElementWithParent(
            [Values] bool isRegularExpression)
        {
            const string name = "name_1";
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            INUnitFilterElementInternal property =
                (INUnitFilterElementInternal) element.Property(name, value, isRegularExpression);

            Assert.IsNotNull(property);
            Assert.AreEqual(NUnitElementType.Property, property.ElementType);
            Assert.AreSame(element, property.Parent);
            Assert.AreSame(property, element.Child);
            Assert.AreEqual(name, property.ElementName);
            Assert.AreEqual(value, property.ElementValue);
            Assert.AreEqual(isRegularExpression, property.IsRegularExpression);
        }

        [Test]
        public void TestPropertyThrowsArgumentExceptionWhenPropertyNameIsNullOrEmpty(
            [Values] bool isNull)
        {
            string name = isNull ? null : string.Empty;
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement property = element.Property(name, value);
                });
        }

        [Test]
        public void TestPropertyThrowsArgumentExceptionWhenPropertyValueIsNullOrEmpty(
            [Values] bool isNull)
        {
            const string name = "name_1";
            string value = isNull ? null : string.Empty;
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The value cannot be null or empty. (Parameter 'value')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement property = element.Property(name, value);
                });
        }

        [Test]
        public void TestPropertyThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
        {
            TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement property = e.Property(s, s, b);
            });
        }

        #endregion

        #region Tests for Name

        [Test]
        public void TestNameThrowsArgumentExceptionWhenNameElementNameIsNullOrEmpty(
            [Values] bool isNull)
        {
            string name = isNull ? null : string.Empty;
            const string value = "Value_1";
            const string xmlTag = "parent_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterContainerElementInternal element =
                new NUnitFilterContainerElement(parent, NUnitElementType.And);

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
                {
                    // ReSharper disable once UnusedVariable
                    INUnitFilterElement nameElement = element.Name(name);
                });
        }

        [Test]
        public void TestNameThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
        {
            TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement nameElement = e.Name(s, b);
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Generic test for Test(Method)ThrowsInvalidOperationExceptionWhenChildIsAlreadySet.
        /// </summary>
        /// <param name="testFunction">
        ///     The function under test provided with the <see cref="NUnitFilterContainerElement" />,
        ///     <see cref="INUnitFilterElementInternal.ElementName" />, and
        ///     <see cref="INUnitFilterElementInternal.IsRegularExpression" />
        /// </param>
        private static void TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet(
            Action<NUnitFilterContainerElement, string, bool> testFunction)
        {
            const string name = "element_1";
            const string value = "Value_";
            const string xmlTag = "name_";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag + 1, value + 1, NUnitElementType.Test);
            XmlSerializableElementForTest child =
                new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Test);

            NUnitFilterContainerElementForTest element =
                new NUnitFilterContainerElementForTest(parent, NUnitElementType.And);
            element.SetChild(child);

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>().And.Message
                    .EqualTo("The child element has already been set for this instance."),
                () => { testFunction(element, name, false); });
        }

        #endregion
    }
}