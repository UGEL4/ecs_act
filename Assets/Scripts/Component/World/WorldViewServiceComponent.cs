using Entitas;
using Entitas.CodeGeneration.Attributes;

[Meta, Unique]
public sealed class WorldViewServiceComponent : IComponent
{
    public IWorldViewService instance;
}