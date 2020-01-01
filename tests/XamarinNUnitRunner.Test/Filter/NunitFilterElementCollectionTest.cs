using System;
using NUnit.Framework;
using XamarinNUnitRunner.Filter;

namespace XamarinNUnitRunner.Test.Filter
{
    [TestFixture]
    public class NUnitFilterElementCollectionTest
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
                () => new NUnitFilterElementCollection(parent, NUnitElementType.RootFilter));
        }

        [Test]
        public void
            TestConstructorThrowsArgumentNullExceptionWhenParentNullAndElementTypeNotRootFilter()
        {
            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The parent cannot be null. (Parameter 'parent')"),
                // ReSharper disable once ObjectCreationAsStatement
                () => new NUnitFilterElementCollection(null, NUnitElementType.And));
        }

        [Test]
        public void
            TestConstructorThrowsArgumentOutOfRangeExceptionWhenElementTypeNotSupported(
                [ValueSource(typeof(NUnitFilterTestHelper), nameof(NUnitFilterTestHelper.GetFilterElementsPlusNot))]
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
                () => new NUnitFilterElementCollection(parent, elementType));
        }

        [Test]
        [TestCase(NUnitElementType.And)]
        [TestCase(NUnitElementType.Or)]
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
                default:
                    Assert.Fail($"The type {elementType} is not supported for this test.");
                    break;
            }

            INUnitFilterElementCollectionInternal element = new NUnitFilterElementCollection(parent, elementType);

            Assert.AreSame(parent, element.Parent);
            Assert.IsNull(element.Child);
            Assert.AreEqual(elementType, element.ElementType);
            Assert.AreEqual(expectedXmlTag, element.XmlTag);
        }

        [Test]
        public void TestConstructorWithParentNullAndElementTypeRootFilter()
        {
            INUnitFilterElementCollectionInternal element =
                new NUnitFilterElementCollection(null, NUnitElementType.RootFilter);

            Assert.IsNull(element.Parent);
            Assert.IsNull(element.Child);
            Assert.AreEqual(NUnitElementType.RootFilter, element.ElementType);
            Assert.AreEqual(NUnitFilterTestHelper.XmlFilterTag, element.XmlTag);
        }

        #endregion

        #region Tests for Not Property

        [Test]
        public void TestNotPropertySetsChildAndReturnsNewAndElementWithParent()
        {
            const string value = "Value_1";
            const string xmlTag = "name_1";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

            INUnitFilterElementCollectionInternal element =
                new NUnitFilterElementCollection(parent, NUnitElementType.And);

            INUnitFilterContainerElementInternal not = (INUnitFilterContainerElementInternal) element.Not;

            Assert.IsNotNull(not);
            Assert.AreEqual(NUnitElementType.Not, not.ElementType);
            Assert.AreSame(element, not.Parent);
            Assert.AreSame(not, element.Child);
        }

        [Test]
        public void TestNotPropertyThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
        {
            const string value = "Value_";
            const string xmlTag = "name_";
            XmlSerializableElementForTest parent =
                new XmlSerializableElementForTest(xmlTag + 1, value + 1, NUnitElementType.Test);
            XmlSerializableElementForTest child =
                new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Test);

            NUnitFilterElementCollectionForTest element =
                new NUnitFilterElementCollectionForTest(parent, NUnitElementType.And);
            element.SetChild(child);

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>().And.Message
                    .EqualTo("The child element has already been set for this instance."),
                // ReSharper disable once UnusedVariable
                () =>
                {
                    INUnitFilterContainerElement not = element.Not;
                });
        }

        #endregion

        // Rest of the NUnitFilterElementCollection class is inherited directly from the NUnitFilterContainerElement class,
        // therefore no further testing is needed as testing is covered by NUnitFilterContainerElement tests.
    }
}