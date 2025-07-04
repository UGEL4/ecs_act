using Entitas;

public sealed class GameMainLogic
{
    GameLogicSystems _systems;
    Systems _serviceSystems;

    public void Start()
    {
        var contexts         = Contexts.sharedInstance;
        var viewService      = new UnityViewService();
        var levelViewService = new UnityLevelViewService();
        var worldViewService = new UnityWorldViewService();
        //var inputService     = new UnityInputService();
        var service          = new Service(viewService, levelViewService, worldViewService);
        _serviceSystems      = new ServiceRegisterSystem(contexts, service);
        
        _systems = new GameLogicSystems(contexts);

        _serviceSystems.Initialize();
        _systems.Initialize();

        var world = contexts.game.CreateEntity();
        world.AddCreateWorldCmd(IdGenerator.NextEntityId(), IdGenerator.NextEntityId(), "DefaultWorld", "Levels/Level_01");
    }

    public void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}