using Entitas;
using Unity.Mathematics;
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
            IViewController viewController = viewGo.GetComponent<MonoLevel>();
            if (viewController == null)
            {
                viewController = viewGo.AddComponent<MonoLevel>();
            }
            if (viewController != null)
            {
                if (config != null)
                {
                    viewController.Position = config.position;
                }
                var e = (GameEntity)entity;
                //e.AddPosition(viewController.Position);
                float3 pos = viewController.Position;
                float3 scale = viewController.Scale;
                quaternion rotation = quaternion.Euler(viewController.Rotation);
                e.AddPosition(pos);
                e.AddRotation(rotation);
                e.AddScale(scale);
                e.AddWorldTransform(TransformComponentExtenstion.TRS(pos, rotation, scale, float4x4.identity));

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