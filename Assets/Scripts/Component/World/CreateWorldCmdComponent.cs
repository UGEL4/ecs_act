using Entitas;

[Game]
public sealed class CreateWorldCmdComponent : IComponent
{
    public long worldId;
    public long defaultLevelId;
    public string assetName;
    public string defaultLevelAssetName;
}