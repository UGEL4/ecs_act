using UnityEngine;
using Entitas;

public class UnityViewService : IViewService
{
    // now returns void instead of IViewController
    public void LoadAsset(Contexts contexts, IEntity entity, string assetName)
    {
        //Similar to before, but now we don't return anything. 
        var viewGo = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + assetName));
        if (viewGo != null)
        {
            var viewController = viewGo.GetComponent<IViewController>();
            if (viewController != null)
            {
                viewController.InitializeView(contexts, entity);
            }

            // except we add some lines to find and initialize any event listeners
            var eventListeners = viewGo.GetComponents<IEventListener>();
            foreach (var listener in eventListeners)
            {
                listener.RegisterListeners(entity);
            }
        }
    }
}