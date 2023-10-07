namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Base interface for type resolution interfaces working in a sequence-like fashion with support for overriding the index value.
    /// </summary>
    public interface ISequenceTypeResolutionParameter : ITypeResolutionParameter
    {
        /// <summary>
        /// Optional value to override the index defined by the type resolution feature.
        /// </summary>
        int IndexOverride
        {
            get;
        }
    }
}