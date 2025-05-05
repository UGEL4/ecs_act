using System.Collections.Generic;
using Entitas;

public class InputProcessSystem : ReactiveSystem<InputEntity>
{
    Contexts _contexts;
    public InputProcessSystem(Contexts contexts) : base(contexts.input)
    {
        _contexts = contexts;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        foreach (var e in entities)
        {
            //e.isDestroyed = true;
            // var input = e.input;
            // var player = _contexts.player.GetEntityWithId(input.playerId);
            // if (player != null)
            // {
            //     var playerEntity = _contexts.player.GetEntityWithId(input.playerId);
            //     playerEntity.ReplaceMove(input.move);
            //     playerEntity.ReplaceRotate(input.rotate);
            // }
        }
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasInput;
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.Input);
    }
}