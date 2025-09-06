using Unity.GraphToolkit.Editor;

namespace Misaki.GraphProcessor.Editor
{
    public interface ICustomDependency
    {
        public INode[] GetDependentNodes(GraphFlow flow);
    }
}