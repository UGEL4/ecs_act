using Entitas;
using Entitas.CodeGeneration.Attributes;
using Unity.Mathematics;

[Game, Event(EventTarget.Self)]
public sealed class ScaleComponent : IComponent
{
    public float3 value;
}