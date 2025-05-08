using Entitas;

[Game]
public sealed class LevelAddEntityCmdComponent : IComponent
{
    public long levelId;
    public long entityId;
    public string assetName;
}