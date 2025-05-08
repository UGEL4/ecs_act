using Entitas;
public interface ILevelViewService
{
    // create a view from a premade asset (e.g. a prefab)
    public void LoadAsset(Contexts contexts, IEntity entity, string assetName, string configName);
}