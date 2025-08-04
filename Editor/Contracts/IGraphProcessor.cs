namespace Misaki.GraphProcessor.Editor
{
    /// <summary>
    /// Interface for options used to build a graph.
    /// </summary>
    public interface IBuildOption
    {
    }

    /// <summary>
    /// Interface for processing a graph, allowing for building and executing it.
    /// </summary>
    public interface IGraphProcessor
    {
        /// <summary>
        /// Builds the graph based on the provided build options.
        /// </summary>
        /// <typeparam name="T">Type of the build option, which must implement <see cref="IBuildOption"/>.</typeparam>
        /// <param name="buildOption">The build option containing parameters for building the graph.</param>
        public void BuildGraph<T>(in T buildOption)
            where T : IBuildOption;

        /// <summary>
        /// Executes the graph after it has been built.
        /// </summary>
        public void ExecuteGraph();
    }
}