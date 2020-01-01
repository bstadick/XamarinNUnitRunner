namespace XamarinNUnitRunner.Filter
{
    /// <summary>
    ///     Base NUnit filter element interface.
    /// </summary>
    /// <see href="http://github.com/nunit/docs/wiki/Test-Filters" />
    internal interface INUnitFilterBaseElement : INUnitFilterXmlSerializableElement
    {
        #region Properties

        /// <summary>
        ///     Gets the parent of the NUnit element or <c>null</c> if the element is the root.
        /// </summary>
        INUnitFilterBaseElement Parent { get; }

        /// <summary>
        ///     Gets the child of the NUnit element or <c>null</c> if the element is the leaf.
        /// </summary>
        INUnitFilterBaseElement Child { get; }

        /// <summary>
        ///     Gets the type of the NUnit element.
        /// </summary>
        NUnitElementType ElementType { get; }

        #endregion
    }
}