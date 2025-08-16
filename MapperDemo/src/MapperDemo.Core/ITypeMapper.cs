namespace MapperDemo.Core
{
    /// <summary>
    /// Defines the contract for a strongly-typed mapper between specific source and target types.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TTarget">The target type.</typeparam>
    public interface ITypeMapper<TSource, TTarget>
    {
        /// <summary>
        /// Maps an object from source type to target type.
        /// </summary>
        /// <param name="source">The source object to map from.</param>
        /// <returns>The mapped target object.</returns>
        TTarget Map(TSource source);
    }
}