using Misaki.GraphProcessor.Editor;
using System;

namespace Misaki.GraphProcessor.Sample
{
    [Serializable]
    internal class AddNode : ExecutableNode
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
}