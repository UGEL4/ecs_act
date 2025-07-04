using Entitas;
using Unity.Mathematics;
[Game]
public class ForceMoveComponent : IComponent
{
    public float3 value;
    public bool force_move;
}