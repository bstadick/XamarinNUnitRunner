namespace XamarinNUnitRunner.Filter
{
    /// <summary>
    ///     NUnit Xml serializable element
    /// </summary>
    internal interface INUnitFilterXmlSerializableElement
    {
        #region Properties

        /// <summary>
        ///     Gets the Xml string element tag.
        /// </summary>
        string XmlTag { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Formats the NUnit element as its Xml string representation.
        /// </summary>
        /// <param name="withXmlTag">
        ///     <c>true</c> if the <see cref="XmlTag" /> should be included in the string, otherwise
        ///     <c>false</c> to exclude the <see cref="XmlTag" />.
        /// </param>
        /// <returns>The NUnit element Xml string representation.</returns>
        string ToXmlString(bool withXmlTag = true);

        #endregion
    }
}