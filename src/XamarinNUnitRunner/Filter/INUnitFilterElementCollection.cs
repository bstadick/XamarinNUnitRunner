using System;

namespace XamarinNUnitRunner.Filter
{
    /// <summary>
    ///     Internal NUnit filter element collection interface.
    /// </summary>
    /// <remarks>
    ///     Implemented by: Filter, And, Or
    ///     Children: Not, Id, Test, Category, Class, Method, Namespace, Property, NUnitName
    ///     Parent: Id, Test, Category, Class, Method, Namespace, Property, NUnitName
    /// </remarks>
    internal interface INUnitFilterElementCollectionInternal : INUnitFilterElementCollection,
        INUnitFilterContainerElementInternal
    {
    }

    /// <summary>
    ///     NUnit filter element collection interface.
    /// </summary>
    /// <remarks>
    ///     Implemented by: Filter, And, Or
    ///     Children: Not, Id, Test, Category, Class, Method, Namespace, Property, NUnitName
    ///     Parent: Id, Test, Category, Class, Method, Namespace, Property, NUnitName
    /// </remarks>
    public interface INUnitFilterElementCollection : INUnitFilterContainerElement
    {
        #region Properties

        /// <summary>
        ///     Gets a Not element to invert the next filter or group of filter elements.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="INUnitFilterBaseElement.Child" /> has already been set.</exception>
        INUnitFilterContainerElement Not { get; }

        #endregion
    }
}