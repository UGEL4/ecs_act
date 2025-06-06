using System.Collections.Generic;

public class InputToCommandProcress
{
    private const int mRecordKeyKeepFrame = 30; //todo : globalconfig
    private GameEntity entity;

    public InputToCommandProcress(Contexts contexts, GameEntity entity)
    {
        this.entity = entity;
    }

    public void Execute(long frame)
    {
        if (!entity.hasInputToCommand || !entity.hasInputController)
        {
            return;
        }

        var commands = entity.inputToCommand.commands;

        long now  = frame;
        int index = 0;
        while (index < commands.Count) //移除过期的指令
        {
            if (now - commands[index].frame > mRecordKeyKeepFrame)
            {
                commands.RemoveAt(index);
            }
            else
            {
                index++;
            }
        }

        var inputController     = entity.inputController;
        List<KeyRecord> newCMDs = inputController.instance.GetCommands(now);
        commands.AddRange(newCMDs);
        //mNewInputList.Clear();
    }
}