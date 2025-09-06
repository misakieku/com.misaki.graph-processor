namespace Misaki.GraphProcessor.Editor
{
    /// <summary>
    /// Represents data that is unique to each branch in a graph processing system.
    /// </summary>
    /// <remarks>
    /// This only creates a unique copy of the data when necessary. No data is copied when the graph flow is linear at current step.
    /// </remarks>
    public interface IBranchUniqueData
    {
        /// <summary>
        /// Make a unique copy of the data for writing. This is used to ensure that each branch has its own copy of the data when it is modified.
        /// </summary>
        /// <returns>A unique copy of the data that can be modified without affecting other branches.</returns>
        public object MakeUniqueForWrite();
    }
}