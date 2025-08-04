namespace Misaki.GraphProcessor.Editor
{
    /// <summary>
    /// Interface for nodes that can be executed within a graph.
    /// </summary>
    public interface IExecutable
    {
        /// <summary>
        /// Executes the logic of the node.
        /// </summary>
        public void Execute();
    }
}
