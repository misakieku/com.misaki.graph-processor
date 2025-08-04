using Misaki.GraphProcessor.Editor;
using System;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;

#nullable enable

namespace Misaki.GraphProcessor.Sample
{
    [Graph("math")]
    [Serializable]
    internal class MathGraph : Graph
    {
        private IGraphProcessor? _processor;

        [MenuItem("Assets/Create/Graph Processor Samples/Math Graph", false)]
        private static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<MathGraph>();
        }

        public override void OnEnable()
        {
            _processor ??= new ExecutableGraphProcessor(this);
        }

        public override void OnGraphChanged(GraphLogger graphLogger)
        {
            if (_processor == null)
            {
                graphLogger.LogError("Graph processor is not initialized.");
                return;
            }

            var options = new ExecutableGraphProcessor.BuildOption()
            {
                Flow = GraphFlow.Backward,
                MasterNodes = GetNodes().Where(n => n is OutputNode).ToArray()
            };
            _processor.BuildGraph(in options);
            _processor.ExecuteGraph();
        }
    }
}