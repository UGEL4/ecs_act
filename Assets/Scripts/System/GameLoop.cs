using Entitas;

public sealed class GameLoop
{
    GameEntity gameEntity;
    GameMainLogic gameMainLogic;

    Contexts contexts;
    public GameLoop(Contexts contexts)
    {
        this.contexts = contexts;
        gameMainLogic = new GameMainLogic();
    }

    public void Initialize()
    {
        gameMainLogic.Start();
    }

    public void OnUpdate(long frame)
    {
        gameMainLogic.Update();
    }
}