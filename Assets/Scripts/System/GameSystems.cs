public sealed class GameSystems : Feature
{
    public GameSystems(Contexts contexts)
    {
        Add(new TimerManagerSystem(contexts));
        //Add(new TimerComponentSystem(contexts));
        //Add(new AddWorldSystem(contexts));
        //Add(new AddLevelSystem(contexts));
        //Add(new LevelAddEntitySystem(contexts));
        //Add(new InputSystem(contexts));
        //Add(new InputCommondProcessSystem(contexts));
        //Add(new MoveSystem(contexts));
    }
}

public sealed class GameLogicSystems : Feature
{
    public GameLogicSystems(Contexts contexts)
    {
        Add(new AddWorldSystem(contexts));
        Add(new AddLevelSystem(contexts));
        Add(new LevelAddEntitySystem(contexts));
        Add(new TimerComponentSystem(contexts));
        Add(new InputToCommandSystem(contexts));
        Add(new ActionUpdateSystem(contexts));
        //Add(new MoveSystem(contexts));
        //actionsystem
    }
}