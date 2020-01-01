namespace XamarinNUnitRunner.Filter
{
    /// <summary>
    ///     Implementation of the INUnitFilterElementCollection interface.
    /// </summary>
    /// <remarks>
    ///     Implemented by: Filter, And, Or
    /// </remarks>
    internal class NUnitFilterElementCollection : NUnitFilterContainerElement, INUnitFilterElementCollectionInternal
    {
        #region Constructors

        /// <inheritdoc />
        public NUnitFilterElementCollection(INUnitFilterBaseElement parent, NUnitElementType elementType) : base(
            parent, elementType)
        {
            if (elementType == NUnitElementType.Not)
            {
                throw ExceptionHelper.ThrowArgumentOutOfRangeExceptionForElementTypeEnum(nameof(elementType),
                    elementType);
            }
        }

        #endregion

        #region Implementation of INUnitFilterElementCollection

        /// <inheritdoc />
        public INUnitFilterContainerElement Not
        {
            get
            {
                if (Child != null)
                {
                    throw ExceptionHelper.ThrowInvalidOperationExceptionForChildAlreadySet();
                }

                INUnitFilterContainerElementInternal element =
                    new NUnitFilterContainerElement(this, NUnitElementType.Not);
                Child = element;
                return element;
            }
        }

        #endregion
    }
}