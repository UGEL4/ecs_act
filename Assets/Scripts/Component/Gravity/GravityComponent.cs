using Entitas;

[Game]
public class GravityComponent : IComponent
{
    public float Weight;
    public int Ticked;
    public float CurrentWeight;
    public bool ApplyGravity;
    public bool IsGrounded;
}