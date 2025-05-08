using Entitas;

[Game]
public sealed class LevelCreateCommandComponent : IComponent
{
    public long worldId;
    public long levelId;
    public string assetName;
    public string levelConfigName;
}