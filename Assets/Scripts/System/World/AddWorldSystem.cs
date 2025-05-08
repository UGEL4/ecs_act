using System.Collections.Generic;
using Entitas;

public sealed class AddWorldSystem : ReactiveSystem<GameEntity>, IInitializeSystem
{
    Contexts contexts;
    private IWorldViewService service;

    public AddWorldSystem(Contexts contexts) : base(contexts.game)
    {
        this.contexts = contexts;
    }

    public void Initialize()
    {
        service = contexts.meta.worldViewService.instance;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.CreateWorldCmd);
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            long worldId     = entity.createWorldCmd.worldId;
            long levelId     = entity.createWorldCmd.defaultLevelId;
            string levelName = entity.createWorldCmd.defaultLevelAssetName;
            var world        = contexts.game.CreateEntity();
            world.isWorld    = true;
            world.AddWorldId(entity.createWorldCmd.worldId);
            service.LoadAsset(contexts, world, entity.createWorldCmd.assetName);
            entity.Destroy();

            var defaultLevelCmd = contexts.game.CreateEntity();
            defaultLevelCmd.AddLevelCreateCommand(worldId, levelId, levelName, "level_01");
        }
    }
}