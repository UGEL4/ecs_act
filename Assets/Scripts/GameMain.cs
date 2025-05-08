using Entitas;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    Systems _systems;
    Systems _serviceSystems;

    void Start()
    {
        var contexts         = Contexts.sharedInstance;
        var viewService      = new UnityViewService();
        var levelViewService = new UnityLevelViewService();
        var worldViewService = new UnityWorldViewService();
        var service          = new Service(viewService, levelViewService, worldViewService);
        _serviceSystems      = new ServiceRegisterSystem(contexts, service);
        
        _systems = new GameSystems(contexts);

        _serviceSystems.Initialize();
        _systems.Initialize();

        var world = contexts.game.CreateEntity();
        world.AddCreateWorldCmd(IdGenerator.NextEntityId(), IdGenerator.NextEntityId(), "DefaultWorld", "Levels/Level_01");
    }

    void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}