using Entitas;
using Unity.VisualScripting;
using UnityEngine;

public class UnityWorldViewService : IWorldViewService
{
    public void LoadAsset(Contexts contexts, IEntity entity, string assetName)
    {
        var viewGo = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + assetName));
        if (viewGo != null)
        {
            IViewController viewController = viewGo.GetOrAddComponent<MonoWorld>();
            if (viewController != null)
            {
                viewController.InitializeView(contexts, entity);
            }
        }
    }
}