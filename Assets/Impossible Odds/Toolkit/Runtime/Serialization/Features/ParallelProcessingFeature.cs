namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Feature object to enable/disable parallel processing of data during (de)serialization.
    /// </summary>
    public class ParallelProcessingFeature : IParallelProcessingFeature
    {
        /// <inheritdoc />
        public bool Enabled { get; set; } = false;
    }
}