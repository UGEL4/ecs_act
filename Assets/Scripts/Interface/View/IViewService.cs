using Entitas;
public interface IViewService
{
    // create a view from a premade asset (e.g. a prefab)
    void LoadAsset(Contexts contexts, IEntity entity, string assetName);
}