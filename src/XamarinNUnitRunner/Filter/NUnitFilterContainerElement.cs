using System;
using System.Linq;
using XamarinNUnitRunner.Resources;

namespace XamarinNUnitRunner.Filter
{
    /// <summary>
    ///     Implementation of the INUnitFilterContainerElement interface.
    /// </summary>
    /// <remarks>
    ///     Implemented by: Not
    ///     Extended by: Filter, And, Or
    /// </remarks>
    internal class NUnitFilterContainerElement : INUnitFilterContainerElementInternal
    {
        #region Private Members

        /// <summary>
        ///     Holds the element's child element.
        /// </summary>
        private INUnitFilterBaseElement v_Child;

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructs a new NUnit filter container element with the given parent and type.
        /// </summary>
        /// <param name="parent">The parent of the NUnit element or <c>null</c> if the element is the root.</param>
        /// <param name="elementType">The type of NUnit filter element.</param>
        /// <exception cref="ArgumentNullException">
        ///     <see cref="parent" /> is <c>null</c> and <see cref="elementType" /> is not
        ///     <see cref="NUnitElementType.RootFilter" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <see cref="parent" /> is not <c>null</c> and <see cref="elementType" /> is
        ///     <see cref="NUnitElementType.RootFilter" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="elementType" /> is not supported by this class.</exception>
        public NUnitFilterContainerElement(INUnitFilterBaseElement parent, NUnitElementType elementType)
        {
            // Parent element can be null for root filter as the root has no parent
            if (parent == null && elementType != NUnitElementType.RootFilter)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(parent));
            }

            if (parent != null && elementType == NUnitElementType.RootFilter)
            {
                throw ExceptionHelper.ThrowArgumentException(Resource.ArgumentExceptionForRootFilterElementNullMessage,
                    nameof(parent));
            }

            Parent = parent;
            XmlTag = MapXmlTag(elementType);
            ElementType = elementType;
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

        #region Private Methods

        /// <summary>
        ///     Maps the element type to the expected Xml tag string.
        /// </summary>
        /// <param name="elementType">The type of NUnit filter element.</param>
        /// <returns>The mapped type of the Xml tag string.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="elementType" /> is not supported by this class.</exception>
        private static string MapXmlTag(NUnitElementType elementType)
        {
            switch (elementType)
            {
                case NUnitElementType.RootFilter:
                    return "filter";
                case NUnitElementType.And:
                    return "and";
                case NUnitElementType.Or:
                    return "or";
                case NUnitElementType.Not:
                    return "not";
                default:
                    throw ExceptionHelper.ThrowArgumentOutOfRangeExceptionForElementTypeEnum(nameof(elementType),
                        elementType);
            }
        }

        #endregion

        #region Implementation of INUnitFilterContainerElement

        /// <inheritdoc />
        public INUnitFilterBaseElement Parent { get; }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><see cref="Child" /> is set to <c>null</c>.</exception>
        public INUnitFilterBaseElement Child
        {
            get => v_Child;
            protected set => v_Child = value ?? throw ExceptionHelper.ThrowArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        public NUnitElementType ElementType { get; }

        /// <inheritdoc />
        public string XmlTag { get; }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><see cref="Child" /> is <c>null</c>.</exception>
        public string ToXmlString(bool withXmlTag = true)
        {
            if (Child == null)
            {
                throw ExceptionHelper.ThrowArgumentNullException(nameof(Child));
            }

            // Element value is the child or the parent and the child concatenated
            string element = Child.ToXmlString();
            if (ElementType != NUnitElementType.Not)
            {
                element = Parent?.ToXmlString() + element;
            }

            return withXmlTag ? $"<{XmlTag}>{element}</{XmlTag}>" : element;
        }

        /// <inheritdoc />
        public INUnitFilterElement Id(params string[] testIds)
        {
            if (testIds == null || testIds.Length == 0)
            {
                throw ExceptionHelper.ThrowArgumentExceptionForNullOrEmpty(nameof(testIds));
            }

            if (Child != null)
            {
                throw ExceptionHelper.ThrowInvalidOperationExceptionForChildAlreadySet();
            }

            // Filter out empty or null strings in array and join into comma separated string
            // If array only contains null and/or empty strings, then filtered array will be empty, thus joined string will also be empty
            testIds = testIds.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            string joinedIds = string.Join(",", testIds);

            INUnitFilterElementInternal element = new NUnitFilterElement(this, NUnitElementType.Id, joinedIds, false);
            Child = element;
            return element;
        }

        /// <inheritdoc />
        public INUnitFilterElement Test(string testName, bool isRegularExpression = false)
        {
            if (Child != null)
            {
                throw ExceptionHelper.ThrowInvalidOperationExceptionForChildAlreadySet();
            }

            INUnitFilterElementInternal element =
                new NUnitFilterElement(this, NUnitElementType.Test, testName, isRegularExpression);
            Child = element;
            return element;
        }

        /// <inheritdoc />
        public INUnitFilterElement Category(string testCategory, bool isRegularExpression = false)
        {
            if (Child != null)
            {
                throw ExceptionHelper.ThrowInvalidOperationExceptionForChildAlreadySet();
            }

            INUnitFilterElementInternal element =
                new NUnitFilterElement(this, NUnitElementType.Category, testCategory, isRegularExpression);
            Child = element;
            return element;
        }

        /// <inheritdoc />
        public INUnitFilterElement Class(string className, bool isRegularExpression = false)
        {
            if (Child != null)
            {
                throw ExceptionHelper.ThrowInvalidOperationExceptionForChildAlreadySet();
            }

            INUnitFilterElementInternal element =
                new NUnitFilterElement(this, NUnitElementType.Class, className, isRegularExpression);
            Child = element;
            return element;
        }

        /// <inheritdoc />
        public INUnitFilterElement Method(string methodName, bool isRegularExpression = false)
        {
            if (Child != null)
            {
                throw ExceptionHelper.ThrowInvalidOperationExceptionForChildAlreadySet();
            }

            INUnitFilterElementInternal element =
                new NUnitFilterElement(this, NUnitElementType.Method, methodName, isRegularExpression);
            Child = element;
            return element;
        }

        /// <inheritdoc />
        public INUnitFilterElement Namespace(string namespaceName, bool isRegularExpression = false)
        {
            if (Child != null)
            {
                throw ExceptionHelper.ThrowInvalidOperationExceptionForChildAlreadySet();
            }

            INUnitFilterElementInternal element =
                new NUnitFilterElement(this, NUnitElementType.Namespace, namespaceName, isRegularExpression);
            Child = element;
            return element;
        }

        /// <inheritdoc />
        public INUnitFilterElement Property(string propertyName, string propertyValue, bool isRegularExpression = false)
        {
            if (Child != null)
            {
                throw ExceptionHelper.ThrowInvalidOperationExceptionForChildAlreadySet();
            }

            INUnitFilterElementInternal element =
                new NUnitFilterElement(this, NUnitElementType.Property, propertyName, isRegularExpression,
                    propertyValue);
            Child = element;
            return element;
        }

        /// <inheritdoc />
        public INUnitFilterElement Name(string nunitTestName, bool isRegularExpression = false)
        {
            if (Child != null)
            {
                throw ExceptionHelper.ThrowInvalidOperationExceptionForChildAlreadySet();
            }

            INUnitFilterElementInternal element =
                new NUnitFilterElement(this, NUnitElementType.NUnitName, nunitTestName, isRegularExpression);
            Child = element;
            return element;
        }

        #endregion
    }
}