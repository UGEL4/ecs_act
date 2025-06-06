using Entitas;
using Unity.Mathematics;

[Game]
public class WorldTransformComponent : IComponent
{
    public float4x4 value;
}