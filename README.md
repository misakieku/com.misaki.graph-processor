# Graph Processor

A powerful and extensible graph processing system built on Unity's Graph Toolkit, designed for creating data flow graphs with executable nodes and automatic dependency resolution.

## Features

- **Executable Node System**: Create custom nodes that can be executed in a controlled manner
- **Automatic Dependency Resolution**: Topological sorting ensures nodes execute in the correct order
- **Port Value Management**: Seamless data flow between connected nodes with type safety
- **Bidirectional Graph Processing**: Support for both forward and backward graph traversal
- **Built on Graph Toolkit**: Leverages Unity's official Graph Toolkit for robust graph infrastructure
- **Sample Implementation**: Includes a complete math graph example with various node types

## Requirements

- Unity 6000.2 or later
- Unity Graph Toolkit package

## Installation

1. Ensure Unity Graph Toolkit is installed in your project
2. Add this package to your project via Package Manager or by placing it in your `Packages` folder
3. The package will automatically reference the required Graph Toolkit assemblies

## Quick Start

### Creating a Custom Graph

```csharp
using Misaki.GraphProcessor.Editor;
using Unity.GraphToolkit.Editor;

[Graph("my-custom-graph")]
[Serializable]
public class MyCustomGraph : Graph
{
    private IGraphProcessor _processor;

    public override void OnEnable()
    {
        _processor ??= new ExecutableGraphProcessor(this);
    }

    public override void OnGraphChanged(GraphLogger graphLogger)
    {
        if (_processor == null) return;

        var options = new ExecutableGraphProcessor.BuildOption()
        {
            Flow = GraphFlow.Backward, // or GraphFlow.Forward
            MasterNodes = GetNodes().Where(n => n is MyOutputNode).ToArray()
        };
        
        _processor.BuildGraph(in options);
        _processor.ExecuteGraph();
    }
}
```

### Creating Executable Nodes

#### Basic Executable Node

```csharp
using Misaki.GraphProcessor.Editor;

[Serializable]
public class AddNode : ExecutableNode
{
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<float>("A")
            .WithDefaultValue(0)
            .Build();

        context.AddInputPort<float>("B")
            .WithDefaultValue(0)
            .Build();

        context.AddOutputPort<float>("Result")
            .Build();
    }

    public override void Execute()
    {
        var a = GetInputPortValue<float>("A");
        var b = GetInputPortValue<float>("B");
        var result = a + b;
        
        SetPortValue("Result", result);
    }
}
```

#### Output Node Example

```csharp
[Serializable]
public class OutputNode : ExecutableNode
{
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<float>("Input")
            .WithDefaultValue(0)
            .Build();
    }

    public override void Execute()
    {
        var inputValue = GetInputPortValue<float>("Input");
        Debug.Log($"Output: {inputValue}");
    }
}
```

## Core Components

### ExecutableGraphProcessor

The main processor responsible for building and executing graphs:

- **BuildGraph()**: Analyzes the graph structure and creates an execution order using topological sorting
- **ExecuteGraph()**: Executes nodes in the determined order, handling data flow between ports
- **Dependency Resolution**: Automatically detects circular dependencies and prevents infinite loops
- **Data Propagation**: Manages value transfer between connected ports

### DataNode & ExecutableNode

Base classes for creating custom nodes:

- **DataNode**: Implements `IPortValueContainer` for port value management
- **ExecutableNode**: Extends DataNode with `IExecutable` for execution logic
- **Port Definition**: Use `OnDefinePorts()` to define input/output ports with types and default values

### Graph Flow Control

Two processing modes are supported:

- **GraphFlow.Forward**: Process from input nodes to output nodes
- **GraphFlow.Backward**: Process from output nodes back to input nodes (useful for pull-based evaluation)

## API Reference

### IGraphProcessor Interface

```csharp
public interface IGraphProcessor
{
    void BuildGraph<T>(in T buildOption) where T : IBuildOption;
    void ExecuteGraph();
}
```

### IPortValueContainer Interface

```csharp
public interface IPortValueContainer
{
    T GetInputPortValue<T>(string portName);
    object GetPortValue(string portName);
    void SetPortValue(string portName, object value);
}
```

### IExecutable Interface

```csharp
public interface IExecutable
{
    void Execute();
}
```

### BuildOption Structure

```csharp
public struct BuildOption : IBuildOption
{
    public GraphFlow Flow { get; set; }
    public INode[] MasterNodes { get; set; }
}
```

### Performance Considerations

- Uses iterative depth-first search to avoid stack overflow on large graphs
- Efficient port pooling for connection traversal
- Minimal memory allocations during execution

## Support

For issues, questions, or feature requests, please refer to the project's issue tracker or documentation.