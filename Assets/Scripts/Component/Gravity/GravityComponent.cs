using Entitas;

[Game]
public class GravityComponent : IComponent
{
    public float DefaultGravity;
    public float Gravity;
    public bool ApplyGravity;
    public bool IsGrounded;
}