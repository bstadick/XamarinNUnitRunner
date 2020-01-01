namespace XamarinNUnitRunner.Filter
{
    /// <summary>
    ///     The type of the NUnit filter element.
    /// </summary>
    /// <see href="https://github.com/nunit/docs/wiki/Test-Filters" />
    public enum NUnitElementType
    {
        /// <summary>
        ///     The root filter element. Acts as an And filter.
        /// </summary>
        /// <remarks>Xml tag is "filter"</remarks>
        RootFilter,

        /// <summary>
        ///     An And filter element that combines multiple filter elements together using "and" constructs.
        /// </summary>
        /// <remarks>Xml tag is "and"</remarks>
        And,

        /// <summary>
        ///     An Or filter element that combines multiple filter elements together using "or" constructs.
        /// </summary>
        /// <remarks>Xml tag is "or"</remarks>
        Or,

        /// <summary>
        ///     A Not filter element that inverts the condition of the containing filter element.
        /// </summary>
        /// <remarks>Xml tag is "not"</remarks>
        Not,

        /// <summary>
        ///     An Id filter element that filters based on the NUnit given test id.
        /// </summary>
        /// <remarks>Xml tag is "id"</remarks>
        Id,

        /// <summary>
        ///     A Test name filter element that filters based on the full name of the test.
        /// </summary>
        /// <remarks>Xml tag is "test"</remarks>
        Test,

        /// <summary>
        ///     A Category filter element that filters on the category attribute of the test.
        /// </summary>
        /// <remarks>Xml tag is "cat"</remarks>
        Category,

        /// <summary>
        ///     A Class filter element that filters on the full class name of the test.
        /// </summary>
        /// <remarks>Xml tag is "class"</remarks>
        Class,

        /// <summary>
        ///     A Method filter element that filters on the full method name of the test.
        /// </summary>
        /// <remarks>Xml tag is "method"</remarks>
        Method,

        /// <summary>
        ///     A Namespace filter element that filters on the full namespace of the test.
        /// </summary>
        /// <remarks>Xml tag is "namespace"</remarks>
        Namespace,

        /// <summary>
        ///     A Property filter element that filters on the property attribute name and value of the test.
        /// </summary>
        /// <remarks>Xml tag is "prop"</remarks>
        Property,

        /// <summary>
        ///     A Name filter element that filters on the NUnit given test name.
        /// </summary>
        /// <remarks>Xml tag is "name"</remarks>
        NUnitName
    }
}