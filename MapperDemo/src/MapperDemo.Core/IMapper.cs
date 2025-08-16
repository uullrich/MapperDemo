namespace MapperDemo.Core
{
    /// <summary>
    /// Defines the contract for a mapper that can map between specific source and target types.
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Determines if this mapper can handle the given source and target types.
        /// </summary>
        /// <param name="sourceType">The name/identifier of the source type.</param>
        /// <param name="targetType">The name/identifier of the target type.</param>
        /// <returns>True if this mapper can handle the mapping, false otherwise.</returns>
        bool CanMap(string sourceType, string targetType);

        /// <summary>
        /// Maps the given source object to a target object.
        /// </summary>
        /// <param name="source">The source object to map from.</param>
        /// <param name="sourceType">The name/identifier of the source type.</param>
        /// <param name="targetType">The name/identifier of the target type.</param>
        /// <returns>The mapped object.</returns>
        object Map(object source, string sourceType, string targetType);
    }
}