using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

[Input, Unique]
public sealed class InputServiceComponent : IComponent
{
    public IInputService instance;
}