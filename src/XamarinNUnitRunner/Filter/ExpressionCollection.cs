using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XamarinNUnitRunner.Filter
{
    /// <summary>
    ///     Represents NUnit filter collection for generic serializable elements.
    /// </summary>
    internal class ExpressionCollection<T> : INUnitFilterXmlSerializableElement, ICollection<T>
        where T : INUnitFilterXmlSerializableElement
    {
        #region Private Members

        /// <summary>
        ///     Holds the collection of elements in the collection expression.
        /// </summary>
        private readonly IList<T> v_Elements = new List<T>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructs a new NUnit filter expression collection with the given <see cref="XmlTag" /> name.
        /// </summary>
        /// <param name="xmlTag">The <see cref="XmlTag" /> to use for the collection.</param>
        /// <exception cref="ArgumentException"><see cref="xmlTag" /> is null or empty.</exception>
        public ExpressionCollection(string xmlTag)
        {
            if (string.IsNullOrEmpty(xmlTag))
            {
                throw ExceptionHelper.ThrowArgumentExceptionForNullOrEmpty(nameof(xmlTag));
            }

            XmlTag = xmlTag;
        }

        #endregion

        #region Implementation of INUnitFilterXmlSerializableElement

        /// <inheritdoc />
        public string XmlTag { get; }

        /// <inheritdoc />
        public string ToXmlString(bool withXmlTag = true)
        {
            switch (v_Elements.Count)
            {
                case 0:
                    // No elements so element string is empty
                    return string.Empty;
                case 1:
                    // One element so element string is just the contained element
                    return v_Elements.First().ToXmlString(withXmlTag);
                default:
                {
                    // Join contained elements together and optionally group inside an Xml tag
                    string elements = string.Join(string.Empty, v_Elements.Select(x => x.ToXmlString()));
                    return withXmlTag ? $"<{XmlTag}>{elements}</{XmlTag}>" : elements;
                }
            }
        }

        #endregion

        #region Implementation of ICollection

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return v_Elements.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            if (item == null)
            {
                return;
            }

            v_Elements.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            v_Elements.Clear();
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return v_Elements.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            v_Elements.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            return v_Elements.Remove(item);
        }

        /// <inheritdoc />
        public int Count => v_Elements.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        #endregion
    }
}