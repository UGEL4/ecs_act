using Entitas;

namespace ACTGame
{
    public sealed class GameLogicSystems : Feature
    {
        public GameLogicSystems(Contexts contexts)
        {
            Add(new AddWorldSystem(contexts));
            Add(new AddLevelSystem(contexts));
            Add(new LevelAddEntitySystem(contexts));
            Add(new ACTSystems.TimerComponentSystem(contexts));
            Add(new InputToCommandSystem(contexts));
            Add(new ActionUpdateSystem(contexts));
            Add(new GravitySystem(contexts));
            Add(new KCCSystem(contexts));
            // Add(new MoveSystem(contexts));
            // actionsystem

            // Events (Genetrate)
            Add(new GameEventSystems(contexts));

            // CleanUp (Genetrate)
            Add(new GameCleanupSystems(contexts));
        }
    }

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
            // var inputService     = new UnityInputService();
            var service     = new Service(viewService, levelViewService, worldViewService);
            _serviceSystems = new ServiceRegisterSystem(contexts, service);

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
}