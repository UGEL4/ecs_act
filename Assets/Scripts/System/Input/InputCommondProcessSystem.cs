using Entitas;

public sealed class InputCommondProcessSystem : IExecuteSystem
{
    private IGroup<GameEntity> inputEntities;

    private const int mRecordKeyKeepFrame = 30; //todo : globalconfig

    public InputCommondProcessSystem(Contexts contexts)
    {
        inputEntities = contexts.game.GetGroup(GameMatcher.InputToCommand);
    }

    public void Execute()
    {
        var entities = inputEntities.GetEntities();
        foreach (var e in entities)
        {
            var timer    = e.timer;
            var commands = e.inputToCommand.commands;
            if (timer == null)
            {
                commands.Clear();
                continue;
            }
            float now = timer.GetNow();
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
            //mNewInputList.Clear();
        }
    }
}