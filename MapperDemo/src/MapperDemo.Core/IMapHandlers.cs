namespace MapperDemo.Core
{
    /// <summary>
    /// Defines the contract for the main mapping handler.
    /// </summary>
    public interface IMapHandler
    {
        /// <summary>
        /// Maps data from source type to target type.
        /// </summary>
        /// <param name="data">The source data object to map from.</param>
        /// <param name="sourceType">The name/identifier of the source type.</param>
        /// <param name="targetType">The name/identifier of the target type.</param>
        /// <returns>The mapped object in the target type format.</returns>
        /// <exception cref="MappingException">Thrown when mapping cannot be performed.</exception>
        object Map(object data, string sourceType, string targetType);
        
        /// <summary>
        /// Maps data from source type to target type with strong typing.
        /// </summary>
        /// <typeparam name="TTarget">The expected target type.</typeparam>
        /// <param name="data">The source data object to map from.</param>
        /// <param name="sourceType">The name/identifier of the source type.</param>
        /// <param name="targetType">The name/identifier of the target type.</param>
        /// <returns>The mapped object cast to the target type.</returns>
        TTarget Map<TTarget>(object data, string sourceType, string targetType);
    }
}