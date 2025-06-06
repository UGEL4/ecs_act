using Entitas;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    Systems _systems;

    void Start()
    {
        var contexts = Contexts.sharedInstance;
        _systems = new GameSystems(contexts);
        _systems.Initialize();
        EventSystem.Instance.AddInvoke(InvokeType.GameLoop, typeof(GameLoopInvoke));
        //EventSystem.Instance.AddInvoke(InvokeType.EntityUpdate, typeof(EntityUpdateTimer));
    }

    void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}