using Entitas;
using Entitas.CodeGeneration.Attributes;

[Meta, Unique]
public sealed class LevelViewServiceComponent : IComponent
{
    public ILevelViewService instance;
}