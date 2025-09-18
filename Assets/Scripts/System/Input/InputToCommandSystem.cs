using System.Collections.Generic;
using Entitas;

public class InputToCommandSystem : IExecuteSystem
{
    private readonly Contexts contexts;

    private readonly IGroup<GameEntity> entities;

    public InputToCommandSystem(Contexts contexts)
    {
        this.contexts = contexts;
        entities = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.InputToCommand, GameMatcher.InputController));
    }

    public void Execute()
    {
        foreach (var e in entities.GetEntities())
        {
            if (!e.hasACTGameComponentTimer)
            {
                continue;
            }
            var controller = e.inputController.instance;
            //var timer = e.timer;
            var timer = e.aCTGameComponentTimer;
            var commandComp = e.inputToCommand;

            long now = timer.curFrame;
            int index = 0;
            long mRecordKeyKeepFrame = 30; //todo : globalconfig
            while (index < commandComp.commands.Count) //移除过期的指令
            {
                if (now - commandComp.commands[index].frame > mRecordKeyKeepFrame)
                {
                    commandComp.commands.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }

            List<KeyRecord> newCmds = controller.GetCommands(now);
            if (newCmds.Count > 0)
            {
                commandComp.commands.AddRange(newCmds);
            }
        }
    }
}