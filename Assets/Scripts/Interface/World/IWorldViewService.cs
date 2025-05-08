using Entitas;
public interface IWorldViewService
{
    // create a view from a premade asset (e.g. a prefab)
    public void LoadAsset(Contexts contexts, IEntity entity, string assetName);
}