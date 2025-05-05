using System.Diagnostics;
using Entitas;

public class TimerManagerSystem : IInitializeSystem, IExecuteSystem
{
    private static readonly Stopwatch _gameTimer = new Stopwatch();
    private GameEntity _timerEntity;

    public TimerManagerSystem(Contexts contexts)
    {
        //_timerEntity = contexts.game.timerManagerEntity;
    }

    public void Initialize()
    {
        // Contexts contexts = Contexts.sharedInstance;
        // GameEntity e      = contexts.game.CreateEntity();
        // e.AddTimerManager(new Stopwatch(), 0);
        // TimerManagerComponent timerManager = e.timerManager;
        // timerManager.gameTimer.Restart();
        // timerManager.lastTime = timerManager.gameTimer.ElapsedTicks;
        var gameContext             = Contexts.sharedInstance.game;
        long sceneTimerEntityId     = IdGenerator.NextSceneTimerId();
        GameEntity sceneTimerEntity = gameContext.CreateEntity();
        sceneTimerEntity.AddTimer(0, 60, 0, false);
        sceneTimerEntity.AddId(sceneTimerEntityId);

        _gameTimer.Restart();
        gameContext.SetTimerManager(_gameTimer.ElapsedTicks, 0, sceneTimerEntityId);
        _timerEntity = gameContext.timerManagerEntity;
    }

    public void Execute()
    {
        var _timer         = _timerEntity.timerManager;
        long now           = _gameTimer.ElapsedTicks;
        _timer.accumulator = now - _timer.lastTime;
        _timer.lastTime    = now;
    }
}
