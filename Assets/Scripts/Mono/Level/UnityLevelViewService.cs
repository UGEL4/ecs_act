using Entitas;
using Unity.VisualScripting;
using UnityEngine;

public class UnityLevelViewService : ILevelViewService
{
    public void LoadAsset(Contexts contexts, IEntity entity, string assetName, string configName)
    {
        TextAsset configTxt = Resources.Load<TextAsset>("ConfigJson/Levels/" + configName);
        LevelConfig config  = JsonUtility.FromJson<LevelConfig>(configTxt.text);
        var viewGo = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + assetName));
        if (viewGo != null)
        {
            IViewController viewController = viewGo.GetOrAddComponent<MonoLevel>();
            if (viewController != null)
            {
                if (config != null)
                {
                    viewController.Position = config.position;
                }
                var e = (GameEntity)entity;
                e.AddPosition(viewController.Position);
                viewController.InitializeView(contexts, entity);
            }

            // except we add some lines to find and initialize any event listeners
            // var eventListeners = viewGo.GetComponents<IEventListener>();
            // foreach (var listener in eventListeners)
            // {
            //     listener.RegisterListeners(entity);
            // }
            // {
            //     var c = viewGo.GetComponent<MonoLevelCreateListener>();
            //     if (c == null)
            //     {
            //         c = viewGo.AddComponent<MonoLevelCreateListener>();
            //         c.RegisterListeners(entity);
            //     }
            // }
        }
    }
}