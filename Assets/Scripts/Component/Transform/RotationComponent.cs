using Entitas;
using Entitas.CodeGeneration.Attributes;
using Unity.Mathematics;

[Game, Event(EventTarget.Self)]
public sealed class RotationComponent : IComponent
{
    public quaternion value;
}