using System.Collections.Generic;
using Entitas;

public class ActionControllerSystem : ReactiveSystem<GameEntity>, IInitializeSystem
{
    readonly Contexts contexts;
    
    public ActionControllerSystem(Contexts contexts) : base(contexts.game)
    {
        this.contexts = contexts;
    }

    public void Initialize()
    {
        
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Action, GameMatcher.Timer));
    }
}