using System.Collections.Generic;
using Entitas;
using UnityEngine;

public sealed class LevelAddEntitySystem : ReactiveSystem<GameEntity>, IInitializeSystem
{
    private IViewService viewService;
    private readonly Contexts contexts;
    public LevelAddEntitySystem(Contexts context) : base(context.game)
    {
        contexts = context;
    }

    public void Initialize()
    {
        viewService = contexts.meta.viewService.instance;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            var objEntity = contexts.game.CreateEntity();
            objEntity.AddEntity(entity.levelAddEntityCmd.levelId);
            objEntity.AddId(entity.levelAddEntityCmd.entityId);
            viewService.LoadAsset(contexts, objEntity, entity.levelAddEntityCmd.assetName);
            entity.Destroy();
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.LevelAddEntityCmd);
    }
}