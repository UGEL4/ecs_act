public sealed class GameSystems : Feature
{
    public GameSystems(Contexts contexts)
    {
        Add(new TimerManagerSystem(contexts));
        Add(new TimerComponentSystem(contexts));
    }
}