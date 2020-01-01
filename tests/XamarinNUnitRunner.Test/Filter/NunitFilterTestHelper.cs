using System;
using System.Collections.Generic;
using System.Linq;
using XamarinNUnitRunner.Filter;

namespace XamarinNUnitRunner.Test.Filter
{
    /// <summary>
    ///     Helper methods for NUnit filter tests.
    /// </summary>
    internal static class NUnitFilterTestHelper
    {
        #region Public Members

        /// <summary>
        ///     Constant for And tag.
        /// </summary>
        public const string XmlAndTag = "and";

        /// <summary>
        ///     Constant for Or tag.
        /// </summary>
        public const string XmlOrTag = "or";

        /// <summary>
        ///     Constant for Not tag.
        /// </summary>
        public const string XmlNotTag = "not";

        /// <summary>
        ///     Constant for Filter tag.
        /// </summary>
        public const string XmlFilterTag = "filter";

        #endregion

        #region Helper Methods

        /// <summary>
        ///     Creates an xml node element with a value.
        /// </summary>
        /// <param name="xmlTag">The xml tag name.</param>
        /// <param name="xmlValue">The xml node inner value.</param>
        /// <param name="isRegularExpression">If the value is a regular expression.</param>
        /// <param name="attributes">A dictionary of key-value pair attributes to add to the tag.</param>
        /// <returns>An xml node element with a value.</returns>
        public static string CreateXmlNode(string xmlTag, string xmlValue, bool isRegularExpression = false,
            IDictionary<string, string> attributes = null)
        {
            string regExp = isRegularExpression ? " re=\"1\"" : string.Empty;
            string attrStr = string.Empty;
            if (attributes?.Count > 0)
            {
                attrStr = " " + string.Join(" ", attributes.Select(x => $"{x.Key}=\"{x.Value}\""));
            }

            return $"<{xmlTag}{regExp}{attrStr}>{xmlValue}</{xmlTag}>";
        }

        /// <summary>
        ///     Creates an ExpressionCollection and the expected inner xml.
        /// </summary>
        /// <param name="numElements">The number of elements to add to the collection.</param>
        /// <param name="innerXml">Outputs the expected innerXml of the collection.</param>
        /// <param name="xmlParentTag">The parent tag to use for the collection.</param>
        /// <param name="elementType">The type of element of the contained nodes.</param>
        /// <returns>The ExpressionCollection with elements.</returns>
        public static ExpressionCollection<INUnitFilterBaseElement> CreateCollection(int numElements,
            out string innerXml, string xmlParentTag = XmlAndTag, NUnitElementType elementType = NUnitElementType.And)
        {
            if (numElements < 0)
            {
                throw ExceptionHelper.ThrowArgumentOutOfRangeExceptionForValueLessThanZero(nameof(numElements),
                    numElements);
            }

            innerXml = string.Empty;
            ExpressionCollection<INUnitFilterBaseElement> collection =
                new ExpressionCollection<INUnitFilterBaseElement>(xmlParentTag);

            if (numElements == 0)
            {
                return collection;
            }

            // Create expected string of xml nodes
            const string value = "Value_";
            const string xmlTag = "name_";
            IEnumerable<int> range = Enumerable.Range(1, numElements);
            IEnumerable<string> elements = range.Select(i => CreateXmlNode(xmlTag + i, value + i));
            innerXml = string.Join(string.Empty, elements);

            // Add expressions to collection
            for (int i = 1; i <= numElements; i++)
            {
                collection.Add(new XmlSerializableElementForTest(xmlTag + i, value + i, elementType));
            }

            return collection;
        }

        /// <summary>
        ///     Gets all the NUnitElementTypes for an NUnitFilterElement.
        /// </summary>
        /// <remarks>Excludes: RootFilter, And, Or, Not</remarks>
        /// <returns>All the NUnitElementTypes for an NUnitFilterElement.</returns>
        public static IList<NUnitElementType> GetFilterElements()
        {
            IList<NUnitElementType> values = Enum.GetValues(typeof(NUnitElementType)).Cast<NUnitElementType>().ToList();

            values.Remove(NUnitElementType.RootFilter);
            values.Remove(NUnitElementType.And);
            values.Remove(NUnitElementType.Or);
            values.Remove(NUnitElementType.Not);

            return values;
        }

        /// <summary>
        ///     Gets all the NUnitElementTypes for an NUnitFilterElement plus the <see cref="NUnitElementType.Not" /> element.
        /// </summary>
        /// <remarks>Excludes: RootFilter, And, Or</remarks>
        /// <returns>All the NUnitElementTypes for an NUnitFilterElement element.</returns>
        public static IList<NUnitElementType> GetFilterElementsPlusNot()
        {
            IList<NUnitElementType> values = Enum.GetValues(typeof(NUnitElementType)).Cast<NUnitElementType>().ToList();

            values.Remove(NUnitElementType.RootFilter);
            values.Remove(NUnitElementType.And);
            values.Remove(NUnitElementType.Or);

            return values;
        }

        /// <summary>
        ///     Gets all the NUnitElementTypes for an NUnitFilterElement except for the <see cref="NUnitElementType.Property" />
        ///     element.
        /// </summary>
        /// <remarks>Excludes: RootFilter, And, Or, Not, Property</remarks>
        /// <returns>
        ///     All the NUnitElementTypes for an NUnitFilterElement except for the <see cref="NUnitElementType.Property" />
        ///     element.
        /// </returns>
        public static IList<NUnitElementType> GetFilterElementsExceptProperty()
        {
            IList<NUnitElementType> values = Enum.GetValues(typeof(NUnitElementType)).Cast<NUnitElementType>().ToList();

            values.Remove(NUnitElementType.RootFilter);
            values.Remove(NUnitElementType.And);
            values.Remove(NUnitElementType.Or);
            values.Remove(NUnitElementType.Not);
            values.Remove(NUnitElementType.Property);

            return values;
        }

        #endregion
    }

    /// <summary>
    ///     Implements INUnitFilterBaseElement for use with tests.
    /// </summary>
    internal class XmlSerializableElementForTest : INUnitFilterBaseElement
    {
        #region Private Members

        /// <summary>
        ///     Holds the element's name.
        /// </summary>
        private readonly string v_XmlName;

        /// <summary>
        ///     Holds the element's value.
        /// </summary>
        private readonly string v_XmlValue;

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructs a new element for test with the given xml tag and value and type.
        /// </summary>
        /// <param name="xmlTag">The Xml string element tag.</param>
        /// <param name="xmlName">The Xml string element name.</param>
        /// <param name="elementType">The type of the NUnit filter.</param>
        /// <param name="xmlValue">The Xml string element value.</param>
        public XmlSerializableElementForTest(string xmlTag, string xmlName,
            NUnitElementType elementType, string xmlValue = null)
        {
            XmlTag = xmlTag;
            v_XmlName = xmlName;
            ElementType = elementType;
            v_XmlValue = xmlValue;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override string ToString()
        {
            string parentString =
                Parent == null ? "Null" : $"{{{Parent.GetType().Name}: {{Type: {Parent.ElementType}}}}}";
            string childString =
                Child == null ? "Null" : $"{{{Child.GetType().Name}: {{Type: {Child.ElementType}}}}}";

            return
                $"{{{GetType().Name}: {{Type: {ElementType}, Parent: {parentString}, Child: {childString}}}}}";
        }

        #endregion

        #region Implementation of INUnitFilterBaseElement

        /// <inheritdoc />
        public string XmlTag { get; }

        /// <inheritdoc />
        public string ToXmlString(bool withXmlTag = true)
        {
            Dictionary<string, string> attributes = null;
            if (v_XmlValue != null)
            {
                attributes = new Dictionary<string, string> {{"name", v_XmlValue}};
            }

            return withXmlTag ? NUnitFilterTestHelper.CreateXmlNode(XmlTag, v_XmlName, false, attributes) : v_XmlName;
        }

        /// <inheritdoc />
        public INUnitFilterBaseElement Parent { get; set; }

        /// <inheritdoc />
        public INUnitFilterBaseElement Child { get; set; }

        /// <inheritdoc />
        public NUnitElementType ElementType { get; }

        #endregion
    }

    /// <summary>
    ///     Implements NUnitFilterContainerElement for use with tests.
    /// </summary>
    internal class NUnitFilterContainerElementForTest : NUnitFilterContainerElement
    {
        #region Constructors

        /// <summary>
        ///     Constructs a new NUnit filter container element with the given parent and type.
        /// </summary>
        /// <param name="parent">The parent of the NUnit element or null if the element is the root.</param>
        /// <param name="elementType">The type of NUnit filter element.</param>
        public NUnitFilterContainerElementForTest(INUnitFilterBaseElement parent, NUnitElementType elementType) :
            base(parent, elementType)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Sets the child element.
        /// </summary>
        /// <param name="child">The element to set.</param>
        public void SetChild(INUnitFilterBaseElement child)
        {
            Child = child;
        }

        #endregion
    }

    /// <summary>
    ///     Implements NUnitFilterElementCollection for use with tests.
    /// </summary>
    internal class NUnitFilterElementCollectionForTest : NUnitFilterElementCollection
    {
        #region Constructors

        /// <summary>
        ///     Constructs a new NUnit filter collection element with the given parent and type.
        /// </summary>
        /// <param name="parent">The parent of the NUnit element or null if the element is the root.</param>
        /// <param name="elementType">The type of NUnit filter element.</param>
        public NUnitFilterElementCollectionForTest(INUnitFilterBaseElement parent, NUnitElementType elementType) :
            base(parent, elementType)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Sets the child element.
        /// </summary>
        /// <param name="child">The element to set.</param>
        public void SetChild(INUnitFilterBaseElement child)
        {
            Child = child;
        }

        #endregion
    }

    /// <summary>
    ///     Implements NUnitFilterElement for use with tests.
    /// </summary>
    internal class NUnitFilterElementForTest : NUnitFilterElement
    {
        #region Constructors

        /// <summary>
        ///     Constructs a new NUnit filter element with the given parent, name, and other attributes.
        /// </summary>
        /// <param name="parent">The parent of the NUnit element or null if the element is the root.</param>
        /// <param name="elementType">The type of NUnit filter element.</param>
        /// <param name="name">The name of the element used in the condition check.</param>
        /// <param name="isRegularExpression">If the filter is a regular expression.</param>
        /// <param name="value">The value of the element used in the condition check, if applicable otherwise is null.</param>
        public NUnitFilterElementForTest(INUnitFilterBaseElement parent, NUnitElementType elementType, string name,
            bool isRegularExpression, string value = null) :
            base(parent, elementType, name, isRegularExpression, value)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Sets the child element.
        /// </summary>
        /// <param name="child">The element to set.</param>
        public void SetChild(INUnitFilterBaseElement child)
        {
            Child = child;
        }

        #endregion
    }
}