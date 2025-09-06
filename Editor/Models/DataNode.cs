using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

#nullable enable

namespace Misaki.GraphProcessor.Editor
{
    /// <summary>
    /// Represents an abstract base class for nodes that manage port values in a data flow graph.
    /// </summary>
    public abstract class DataNode : Node, IPortValueContainer
    {
        private readonly Dictionary<string, object?> _portValues = new();

        public T? GetInputPortValue<T>(string portName)
        {
            var port = GetInputPortByName(portName);
            if (port.isConnected)
            {
                if (_portValues.TryGetValue(portName, out var obj) && obj is T value)
                {
                    return value;
                }
            }
            else
            {
                if (GetInputPortByName(portName).TryGetValue<T>(out var value))
                {
                    return value;
                }
            }

            throw new System.ArgumentException($"Port '{portName}' not found or value is not of type {typeof(T).Name}.");
        }

        public object? GetPortValue(string portName)
        {
            return _portValues.GetValueOrDefault(portName);
        }

        public void SetPortValue(string portName, object? value)
        {
            if (string.IsNullOrWhiteSpace(portName))
            {
                throw new System.ArgumentException("Port name cannot be null or empty.", nameof(portName));
            }

            _portValues[portName] = value;
        }

        public T GetOptionValue<T>(string optionName)
        {
            if (GetNodeOptionByName(optionName).TryGetValue<T>(out var value))
            {
                return value;
            }

            throw new System.ArgumentException($"Option '{optionName}' not found or value is not of type {typeof(T).Name}.");
        }
    }

    /// <summary>
    /// Represents an abstract base class for nodes that can be executed and hold port values in a data flow graph.
    /// </summary>
    public abstract class ExecutableNode : DataNode, IExecutable
    {
        public abstract void Execute();
    }
}