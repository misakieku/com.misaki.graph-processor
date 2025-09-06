using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;

#nullable enable

namespace Misaki.GraphProcessor.Editor
{
    /// <summary>
    /// Processes a graph by executing nodes in a topological order based on their dependencies.
    /// </summary>
    public class ExecutableGraphProcessor : IGraphProcessor
    {
        /// <summary>
        /// Represents options for building the graph.
        /// </summary>
        public struct BuildOption : IBuildOption
        {
            /// <summary>
            /// The flow direction for processing the graph.
            /// </summary>
            public GraphFlow Flow
            {
                get; set;
            }

            /// <summary>
            /// The master nodes to start processing from.
            /// </summary>
            public INode[] MasterNodes
            {
                get; set;
            }
        }

        private readonly Graph _graph;
        private readonly List<INode> _processed;
        private readonly List<IPort> _portsPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableGraphProcessor"/> class.
        /// </summary>
        /// <param name="graph">The graph to process.</param>
        public ExecutableGraphProcessor(Graph graph)
        {
            _graph = graph;
            _processed = new();
            _portsPool = new();
        }

        private IEnumerable<INode> GetDependentNodes(INode node, GraphFlow flow)
        {
            var ports = flow switch
            {
                GraphFlow.Forward => node.GetOutputPorts(),
                GraphFlow.Backward => node.GetInputPorts(),
                _ => null
            };

            if (ports == null)
            {
                yield break;
            }

            foreach (var port in ports)
            {
                if (!port.isConnected)
                {
                    continue;
                }

                port.GetConnectedPorts(_portsPool);
                foreach (var connectedPort in _portsPool)
                {
                    yield return connectedPort.GetNode();
                }
            }
        }

        // A depth-first search approach to process nodes in topological order
        // Uses stack instead of recursion to avoid stack overflow on large graphs
        private void ProcessTopologicalOrder(IEnumerable<INode> masterNodes, GraphFlow flow)
        {
            var visited = new HashSet<INode>();
            var visiting = new HashSet<INode>();

            // Need manual traversal state tracking in a iterative dfs
            var stack = new Stack<(INode node, bool isPostProcessing)>();

            // Initialize with master nodes
            foreach (var masterNode in masterNodes)
            {
                stack.Push((masterNode, false));
            }

            while (stack.Count > 0)
            {
                var (currentNode, isPostProcessing) = stack.Pop();

                if (isPostProcessing)
                {
                    // Post-processing: add to processed list and mark as visited
                    visiting.Remove(currentNode);
                    visited.Add(currentNode);

                    _processed.Add(currentNode);
                }
                else
                {
                    // Pre-processing: check for cycles and add dependencies
                    if (visiting.Contains(currentNode))
                    {
                        throw new System.InvalidOperationException($"Circular dependency detected in graph involving node: {currentNode}");
                    }

                    if (visited.Contains(currentNode))
                    {
                        continue;
                    }

                    visiting.Add(currentNode);

                    // Push post-processing entry for this node
                    stack.Push((currentNode, true));

                    INode[] dependencies;
                    if (currentNode is ICustomDependency dependent)
                    {
                        dependencies = dependent.GetDependentNodes(flow);
                    }
                    else
                    {
                        dependencies = GetDependentNodes(currentNode, flow).ToArray();
                    }

                    // Push all dependencies for pre-processing (in reverse order to maintain proper traversal order)
                    for (var i = dependencies.Length - 1; i >= 0; i--)
                    {
                        var dependency = dependencies[i];
                        if (!visited.Contains(dependency))
                        {
                            stack.Push((dependency, false));
                        }
                    }
                }
            }
        }

        public void BuildGraph<T>(in T buildOption)
            where T : IBuildOption
        {
            if (buildOption is not BuildOption option)
            {
                throw new ArgumentException("Invalid build option type", nameof(buildOption));
            }

            var nodes = _graph.GetNodes().ToArray();
            foreach (var node in option.MasterNodes)
            {
                if (!nodes.Contains(node))
                {
                    throw new ArgumentException($"Master node {node} is not part of the graph", nameof(option.MasterNodes));
                }
            }

            _processed.Clear();
            _processed.Capacity = nodes.Length;

            ProcessTopologicalOrder(option.MasterNodes, option.Flow);

            if (option.Flow == GraphFlow.Forward)
            {
                _processed.Reverse();
            }
        }

        private void PushPortData(INode sourceNode)
        {
            object? value = null;

            switch (sourceNode)
            {
                case IConstantNode constantNode:
                    constantNode.TryGetValue<object>(out value);
                    break;
                case IVariableNode variableNode:
                    variableNode.variable.TryGetDefaultValue<object>(out value);
                    break;
                default:
                    // value will be set per port below
                    break;
            }

            foreach (var outputPort in sourceNode.GetOutputPorts())
            {
                if (!outputPort.isConnected)
                    continue;

                // For IPortContainer, get value per port
                var portValue = (sourceNode is IPortValueContainer container)
                    ? container.GetPortValue(outputPort.name)
                    : value;

                var shouldDispose = false;
                outputPort.GetConnectedPorts(_portsPool);
                if (portValue is IBranchUniqueData immutableData)
                {
                    foreach (var connectedPort in _portsPool)
                    {
                        if (connectedPort.GetNode() is IPortValueContainer connectedContainer)
                        {
                            var toSend = _portsPool.Count == 1 ? immutableData : immutableData.MakeUniqueForWrite();
                            connectedContainer.SetPortValue(connectedPort.name, toSend);
                        }
                    }

                    if (_portsPool.Count > 1)
                    {
                        shouldDispose = true;
                    }
                }
                else
                {
                    foreach (var connectedPort in _portsPool)
                    {
                        if (connectedPort.GetNode() is IPortValueContainer connectedContainer)
                        {
                            connectedContainer.SetPortValue(connectedPort.name, portValue);
                        }
                    }
                }

                if ((_portsPool.Count == 0 || shouldDispose) && portValue is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        public void ExecuteGraph()
        {
            if (_processed.Count == 0)
            {
                throw new System.InvalidOperationException("Graph has not been built. Call BuildGraph first.");
            }

            foreach (var node in _processed)
            {
                if (node is IExecutable executableNode)
                {
                    executableNode.Execute();
                }

                PushPortData(node);
            }
        }
    }
}