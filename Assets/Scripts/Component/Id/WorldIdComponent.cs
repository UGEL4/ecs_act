using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public class WorldIdComponent : IComponent
{
    [PrimaryEntityIndex]
    public long value;
}