using System.Collections.Generic;
using Entitas;
using UnityEngine;

public sealed class AddLevelSystem : ReactiveSystem<GameEntity>, IInitializeSystem
{
    private ILevelViewService viewService;
    private readonly Contexts contexts;
    public AddLevelSystem(Contexts context) : base(context.game)
    {
        contexts = context;
    }

    public void Initialize()
    {
        viewService = contexts.meta.levelViewService.instance;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            var sceneEntity = contexts.game.CreateEntity();
            sceneEntity.AddLevel(entity.levelCreateCommand.worldId);//世界的id，用于查找属于哪个世界
            sceneEntity.AddId(entity.levelCreateCommand.levelId); //关卡的id，用于查找
            viewService.LoadAsset(contexts, sceneEntity, entity.levelCreateCommand.assetName, entity.levelCreateCommand.levelConfigName);
            //var levelService = _levelCreateService.CreateLevel(entity, levelRoot);
            entity.Destroy();
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.LevelCreateCommand);
    }
}