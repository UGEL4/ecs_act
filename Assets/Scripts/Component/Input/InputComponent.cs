using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

[Input, Unique, Cleanup(CleanupMode.DestroyEntity)]
public sealed class InputComponent : IComponent
{
    public Vector2 axis;
    public Vector3 rotate;
}