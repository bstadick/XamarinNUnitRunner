using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using XamarinNUnitRunner.Filter;

namespace XamarinNUnitRunner.Test.Filter
{
    [TestFixture]
    public class ExpressionCollectionTest
    {
        #region Tests for Constructor

        [Test]
        public void TestConstructorThrowsArgumentExceptionWhenXmlTagIsNullOrEmpty(
            [Values] bool isNull)
        {
            string xmlTag = isNull ? null : string.Empty;

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message
                    .EqualTo("The xmlTag cannot be null or empty. (Parameter 'xmlTag')"),
                // ReSharper disable once ObjectCreationAsStatement
                () => new ExpressionCollection<INUnitFilterBaseElement>(xmlTag));
        }

        [Test]
        public void TestConstructorWithXmlTag()
        {
            ExpressionCollection<INUnitFilterBaseElement> collection =
                new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

            Assert.AreEqual(NUnitFilterTestHelper.XmlAndTag, collection.XmlTag);
            Assert.AreEqual(0, collection.Count);
        }

        #endregion

        #region Tests for XmlTag Property

        [Test]
        public void TestXmlTagPropertyReturnsXmlTagProvidedWithConstructor()
        {
            ExpressionCollection<INUnitFilterBaseElement> collection =
                new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

            Assert.AreEqual(NUnitFilterTestHelper.XmlAndTag, collection.XmlTag);
        }

        #endregion

        #region Tests for ToXmlString

        [Test]
        public void TestToXmlStringWithCollectionEmptyReturnsEmptyString([Values] bool withXmlTag)
        {
            ExpressionCollection<INUnitFilterBaseElement> collection =
                new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

            Assert.AreEqual(0, collection.Count);
            Assert.AreEqual(string.Empty, collection.ToXmlString(withXmlTag));
        }

        [Test]
        public void TestToXmlStringWithCollectionOfMultipleItemsReturnsCollectionXmlString(
            [Values] bool withXmlTag)
        {
            // Create collection and expected string of xml nodes
            const int count = 3;
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out string innerXml);
            // With tag includes parent xml tag, without is just value
            string expected = withXmlTag
                ? NUnitFilterTestHelper.CreateXmlNode(NUnitFilterTestHelper.XmlAndTag, innerXml)
                : innerXml;

            Assert.AreEqual(count, collection.Count);
            Assert.AreEqual(expected, collection.ToXmlString(withXmlTag));
        }

        [Test]
        public void TestToXmlStringWithCollectionOfOneItemReturnsItemXmlString(
            [Values] bool withXmlTag)
        {
            // Create expected string of xml nodes
            const string value = "Value_1";
            const string xmlTag = "name_1";
            // With tag includes parent xml tag, without is just value
            string expected = withXmlTag ? NUnitFilterTestHelper.CreateXmlNode(xmlTag, value) : value;

            ExpressionCollection<INUnitFilterBaseElement> collection =
                // ReSharper disable once UseObjectOrCollectionInitializer
                new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

            // Add expression to collection
            collection.Add(new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.And));

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(expected, collection.ToXmlString(withXmlTag));
        }

        #endregion

        #region Tests for GetEnumerator

        [Test]
        public void TestGetEnumeratorWithItemsInCollection()
        {
            // Create collection
            const int count = 3;
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out _);

            IEnumerator<INUnitFilterBaseElement> enumerator = collection.GetEnumerator();

            Assert.AreEqual(count, collection.Count);
            Assert.IsNotNull(enumerator);

            // Copy enumerator to list
            IList<INUnitFilterBaseElement> copy = new List<INUnitFilterBaseElement>();
            while (enumerator.MoveNext())
            {
                copy.Add(enumerator.Current);
            }

            CollectionAssert.AreEqual(collection, copy);

            enumerator.Dispose();
        }

        [Test]
        public void TestGetEnumeratorWithNoItemsInCollection()
        {
            ExpressionCollection<INUnitFilterBaseElement> collection =
                new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

            IEnumerator<INUnitFilterBaseElement> enumerator = collection.GetEnumerator();

            Assert.AreEqual(0, collection.Count);
            Assert.IsNotNull(enumerator);

            // Copy enumerator to list
            IList<INUnitFilterBaseElement> copy = new List<INUnitFilterBaseElement>();
            while (enumerator.MoveNext())
            {
                copy.Add(enumerator.Current);
            }

            CollectionAssert.AreEqual(collection, copy);

            enumerator.Dispose();
        }

        #endregion

        #region Tests for Add

        [Test]
        public void TestAddWithNonNullItemAddsItem()
        {
            // Create collection and item to add
            const int count = 3;
            const string value = "Value_new";
            const string xmlTag = "name_new";
            XmlSerializableElementForTest item = new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.And);
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out _);

            collection.Add(item);

            Assert.AreEqual(count + 1, collection.Count);
            Assert.AreSame(item, collection.Last());
        }

        [Test]
        public void TestAddWithNullItemDoesNotAddItem()
        {
            // Create collection
            const int count = 3;
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out _);

            collection.Add(null);

            Assert.AreEqual(count, collection.Count);
        }

        #endregion

        #region Tests for Clear

        [Test]
        public void TestClearWithItemsInCollection()
        {
            // Create collection
            const int count = 3;
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out _);

            Assert.AreEqual(count, collection.Count);

            collection.Clear();

            Assert.AreEqual(0, collection.Count);
        }

        [Test]
        public void TestClearWithoutItemsInCollection()
        {
            ExpressionCollection<INUnitFilterBaseElement> collection =
                new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

            Assert.AreEqual(0, collection.Count);

            collection.Clear();

            Assert.AreEqual(0, collection.Count);
        }

        #endregion

        #region Tests for Contains

        [Test]
        public void TestContainsWhenItemIsNotPresentReturnsFalse([Values] bool isNull)
        {
            // Create collection and item to search
            const int count = 3;
            const string value = "Value_new";
            const string xmlTag = "name_new";
            XmlSerializableElementForTest item =
                isNull ? null : new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.And);
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out _);

            Assert.AreEqual(count, collection.Count);

            bool contains = collection.Contains(item);

            Assert.IsFalse(contains);
        }

        [Test]
        public void TestContainsWhenItemIsPresentReturnsTrue()
        {
            // Create collection
            const int count = 3;
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out _);

            Assert.AreEqual(count, collection.Count);

            bool contains = collection.Contains(collection.First());

            Assert.IsTrue(contains);
        }

        #endregion

        #region Test for CopyTo

        [Test]
        public void TestCopyToThrowsArgumentExceptionWhenCollectionDoesNotFitInArrayLength(
            [Values] bool indexOutOfRange)
        {
            // Create collection
            const int count = 3;
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out _);

            // If indexOutOfRange then the array index plus collection length is longer than the array,
            // otherwise the array is just not long enough to hold collection
            int arrayLength = indexOutOfRange ? count : 0;
            int arrayIndex = indexOutOfRange ? 1 : 0;
            INUnitFilterBaseElement[] array = new INUnitFilterBaseElement[arrayLength];

            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message.EqualTo(
                    "Destination array was not long enough." +
                    " Check the destination index, length, and the array's lower bounds." +
                    " (Parameter 'destinationArray')"),
                () => collection.CopyTo(array, arrayIndex));
        }

        [Test]
        public void TestCopyToThrowsArgumentNullExceptionWhenArrayIsNull()
        {
            ExpressionCollection<INUnitFilterBaseElement> collection =
                new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("Value cannot be null. (Parameter 'destinationArray')"),
                // ReSharper disable once AssignNullToNotNullAttribute
                () => collection.CopyTo(null, 0));
        }

        [Test]
        public void TestCopyToThrowsArgumentOutOfRangeExceptionWhenArrayIndexLessThanZero()
        {
            ExpressionCollection<INUnitFilterBaseElement> collection =
                new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);
            INUnitFilterBaseElement[] array = new INUnitFilterBaseElement[1];

            Assert.Throws(
                Is.TypeOf<ArgumentOutOfRangeException>().And.Message.EqualTo(
                    "Number was less than the array's lower bound in the first dimension. (Parameter 'destinationIndex')"),
                () => collection.CopyTo(array, -1));
        }

        [Test]
        public void TestCopyToWithItemsInCollectionCopiesToArray([Range(0, 2)] int arrayIndex)
        {
            // Create collection and expected
            const int count = 3;
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out _);
            IList<INUnitFilterBaseElement> collectionList = collection.ToList();
            INUnitFilterBaseElement[] expected = new INUnitFilterBaseElement[count + arrayIndex];
            for (int i = arrayIndex, j = 0; i < expected.Length; i++, j++)
            {
                expected[i] = collectionList[j];
            }

            INUnitFilterBaseElement[] array = new INUnitFilterBaseElement[count + arrayIndex];

            Assert.AreEqual(count, collection.Count);

            collection.CopyTo(array, arrayIndex);

            CollectionAssert.AreEqual(expected, array);
        }

        [Test]
        public void TestCopyToWithoutItemsInCollectionCopiesNoneToArray(
            [Range(0, 2)] int arrayIndex)
        {
            // Create collection and expected
            ExpressionCollection<INUnitFilterBaseElement> collection =
                new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);
            INUnitFilterBaseElement[] expected = new INUnitFilterBaseElement[arrayIndex];
            INUnitFilterBaseElement[] array = new INUnitFilterBaseElement[arrayIndex];

            Assert.AreEqual(0, collection.Count);

            collection.CopyTo(array, arrayIndex);

            CollectionAssert.AreEqual(expected, array);
        }

        #endregion

        #region Tests for Remove

        [Test]
        public void TestRemoveWhenItemIsNotPresentReturnsFalse([Values] bool isNull)
        {
            // Create collection and item to search
            const int count = 3;
            const string value = "Value_new";
            const string xmlTag = "name_new";
            XmlSerializableElementForTest item =
                isNull ? null : new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.And);
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out _);
            IList<INUnitFilterBaseElement> expected = new List<INUnitFilterBaseElement>(collection);

            Assert.AreEqual(count, collection.Count);

            bool removed = collection.Remove(item);

            Assert.IsFalse(removed);
            Assert.AreEqual(count, collection.Count);
            CollectionAssert.AreEqual(expected, collection);
        }

        [Test]
        public void TestRemoveWhenItemIsPresentRemovesItemAndReturnsTrue()
        {
            // Create collection
            const int count = 3;
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out _);
            IList<INUnitFilterBaseElement> expected = new List<INUnitFilterBaseElement>(collection);
            expected.RemoveAt(0);

            Assert.AreEqual(count, collection.Count);

            bool removed = collection.Remove(collection.First());

            Assert.IsTrue(removed);
            Assert.AreEqual(count - 1, collection.Count);
            CollectionAssert.AreEqual(expected, collection);
        }

        #endregion

        #region Tests for Count Property

        [Test]
        public void TestCountReturnsNumberOfItems([Range(0, 3)] int count)
        {
            // Create collection
            ExpressionCollection<INUnitFilterBaseElement> collection =
                NUnitFilterTestHelper.CreateCollection(count, out _);

            Assert.AreEqual(count, collection.Count);
        }

        #endregion

        #region Tests for IsReadOnly Property

        [Test]
        public void TestIsReadOnlyReturnsFalse()
        {
            // ReSharper disable once CollectionNeverQueried.Local
            ExpressionCollection<INUnitFilterBaseElement> collection =
                new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

            Assert.IsFalse(collection.IsReadOnly);
        }

        #endregion
    }
}