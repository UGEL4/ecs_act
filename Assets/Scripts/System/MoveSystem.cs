using System.Collections.Generic;
using Entitas;
using UnityEngine;

public sealed class MoveSystem : IExecuteSystem
{
    IGroup<GameEntity> _group;
    public MoveSystem(Contexts contexts)// : base(contexts.game)
    {
        _group = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Position, GameMatcher.InputToCommand));
    }

    public void Execute()
    {
        var entities = _group.GetEntities();
        foreach (var entity in entities)
        {
            var commands = entity.inputToCommand.commands;
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].key == KeyMap.DirInput)
                {
                    //entity.ReplacePosition(entity.position.value + new Vector3(0.01f, 0, 0));
                }
            }
        }
    }

    // protected override void Execute(List<GameEntity> entities)
    // {
    //     foreach (var entity in entities)
    //     {
    //         var commands = entity.inputToCommand.commands;
    //         for (int i = 0; i < commands.Count; i++)
    //         {
    //             if (commands[i].key == KeyMap.DirInput)
    //             {
    //                 entity.ReplacePosition(entity.position.value + new Vector3(1, 0, 0));
    //             }
    //         }
    //     }
    // }

    // protected override bool Filter(GameEntity entity)
    // {
    //     return true;
    // }

    // protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    // {
    //     return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Position, GameMatcher.InputToCommand));
    // }
}