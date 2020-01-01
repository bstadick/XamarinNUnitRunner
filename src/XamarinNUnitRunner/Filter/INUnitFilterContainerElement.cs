using System;

namespace XamarinNUnitRunner.Filter
{
    /// <summary>
    ///     Internal NUnit filter container element interface.
    /// </summary>
    /// <remarks>
    ///     Implemented by: Filter, And, Or, Not
    ///     Children: Id, Test, Category, Class, Method, Namespace, Property, NUnitName
    ///     Parent: Filter, And, Or, Id, Test, Category, Class, Method, Namespace, Property, NUnitName
    /// </remarks>
    internal interface INUnitFilterContainerElementInternal : INUnitFilterContainerElement, INUnitFilterBaseElement
    {
    }

    /// <summary>
    ///     NUnit filter container element interface.
    /// </summary>
    /// <remarks>
    ///     Implemented by: Filter, And, Or, Not
    ///     Children: Id, Test, Category, Class, Method, Namespace, Property, NUnitName
    ///     Parent: Filter, And, Or, Id, Test, Category, Class, Method, Namespace, Property, NUnitName
    /// </remarks>
    public interface INUnitFilterContainerElement
    {
        #region Methods

        /// <summary>
        ///     Filter by the NUnit test Ids.
        /// </summary>
        /// <param name="testIds">The NUnit test Ids.</param>
        /// <returns>The NUnit test Id filter.</returns>
        /// <exception cref="ArgumentException">
        ///     <see cref="testIds" /> is <c>null</c> or empty or only contains <c>null</c> and
        ///     empty strings.
        /// </exception>
        /// <exception cref="InvalidOperationException">The <see cref="INUnitFilterBaseElement.Child" /> has already been set.</exception>
        INUnitFilterElement Id(params string[] testIds);

        /// <summary>
        ///     Filter by the NUnit test name.
        /// </summary>
        /// <param name="testName">The NUnit test name.</param>
        /// <param name="isRegularExpression">If the filter is a regular expression.</param>
        /// <returns>The NUnit test name filter.</returns>
        /// <exception cref="ArgumentException"><see cref="testName" /> is <c>null</c> or empty.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="INUnitFilterBaseElement.Child" /> has already been set.</exception>
        INUnitFilterElement Test(string testName, bool isRegularExpression = false);

        /// <summary>
        ///     Filter by the NUnit test category.
        /// </summary>
        /// <param name="testCategory">The NUnit test category.</param>
        /// <param name="isRegularExpression">If the filter is a regular expression.</param>
        /// <returns>The NUnit test category filter.</returns>
        /// <exception cref="ArgumentException"><see cref="testCategory" /> is <c>null</c> or empty.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="INUnitFilterBaseElement.Child" /> has already been set.</exception>
        INUnitFilterElement Category(string testCategory, bool isRegularExpression = false);

        /// <summary>
        ///     Filter by the NUnit test class name.
        /// </summary>
        /// <param name="className">The NUnit test class name.</param>
        /// <param name="isRegularExpression">If the filter is a regular expression.</param>
        /// <returns>The NUnit test class name filter.</returns>
        /// <exception cref="ArgumentException"><see cref="className" /> is <c>null</c> or empty.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="INUnitFilterBaseElement.Child" /> has already been set.</exception>
        INUnitFilterElement Class(string className, bool isRegularExpression = false);

        /// <summary>
        ///     Filter by the NUnit test method name.
        /// </summary>
        /// <param name="methodName">The NUnit test method name.</param>
        /// <param name="isRegularExpression">If the filter is a regular expression.</param>
        /// <returns>The NUnit test method name filter.</returns>
        /// <exception cref="ArgumentException"><see cref="methodName" /> is <c>null</c> or empty.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="INUnitFilterBaseElement.Child" /> has already been set.</exception>
        INUnitFilterElement Method(string methodName, bool isRegularExpression = false);

        /// <summary>
        ///     Filter by the NUnit test namespace name.
        /// </summary>
        /// <param name="namespaceName">The NUnit test namespace name.</param>
        /// <param name="isRegularExpression">If the filter is a regular expression.</param>
        /// <returns>The NUnit test namespace name filter.</returns>
        /// <exception cref="ArgumentException"><see cref="namespaceName" /> is <c>null</c> or empty.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="INUnitFilterBaseElement.Child" /> has already been set.</exception>
        INUnitFilterElement Namespace(string namespaceName, bool isRegularExpression = false);

        /// <summary>
        ///     Filter by the NUnit test property name and value.
        /// </summary>
        /// <param name="propertyName">The NUnit test property name.</param>
        /// <param name="propertyValue">The NUnit test property value.</param>
        /// <param name="isRegularExpression">If the filter is a regular expression.</param>
        /// <returns>The NUnit test property name and value filter.</returns>
        /// <exception cref="ArgumentException">
        ///     <see cref="propertyName" /> or <see cref="propertyValue" /> is <c>null</c> or
        ///     empty.
        /// </exception>
        /// <exception cref="InvalidOperationException">The <see cref="INUnitFilterBaseElement.Child" /> has already been set.</exception>
        INUnitFilterElement Property(string propertyName, string propertyValue, bool isRegularExpression = false);

        /// <summary>
        ///     Filter by the NUnit given test name.
        /// </summary>
        /// <param name="nunitTestName">The NUnit given test name.</param>
        /// <param name="isRegularExpression">If the filter is a regular expression.</param>
        /// <returns>The NUnit given test name filter.</returns>
        /// <exception cref="ArgumentException"><see cref="nunitTestName" /> is <c>null</c> or empty.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="INUnitFilterBaseElement.Child" /> has already been set.</exception>
        INUnitFilterElement Name(string nunitTestName, bool isRegularExpression = false);

        #endregion
    }
}