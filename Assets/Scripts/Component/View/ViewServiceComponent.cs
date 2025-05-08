using Entitas;
using Entitas.CodeGeneration.Attributes;

[Meta, Unique]
public sealed class ViewServiceComponent : IComponent
{
    public IViewService instance;
}