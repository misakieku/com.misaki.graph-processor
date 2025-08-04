using Misaki.GraphProcessor.Editor;
using System;
using UnityEngine;

namespace Misaki.GraphProcessor.Sample
{
    [Serializable]
    internal class OutputNode : ExecutableNode
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

            Debug.Log($"OutputNode executed with input value: {inputValue}");
        }
    }
}