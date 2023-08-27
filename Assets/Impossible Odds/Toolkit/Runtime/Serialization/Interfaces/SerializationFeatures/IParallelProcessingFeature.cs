namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Parallel processing serialization feature to define whether a process is allowed to process its data in
    /// parallel, potentially speeding up the (de)serialization process.
    /// </summary>
    public interface IParallelProcessingFeature : ISerializationFeature
    {
        /// <summary>
        /// Is the processor allowed to process data in parallel?
        /// </summary>
        bool Enabled
        {
            get;
        }
    }
}