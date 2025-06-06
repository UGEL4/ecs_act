using System;
using System.Collections.Generic;
using System.Diagnostics;
using Entitas;

public class TimerManagerSystem : IInitializeSystem, IExecuteSystem
{
    private static readonly Stopwatch _gameTimer = new Stopwatch();
    private GameEntity _timerEntity;
    private GameLoop gameLoop;
    private GameContext gameContext;

    public TimerManagerSystem(Contexts contexts)
    {
        //_timerEntity = contexts.game.timerManagerEntity;
        //mainLogic = new GameMainLogic();
        gameLoop = new GameLoop(contexts);
        gameContext = contexts.game;
    }

    public void Initialize()
    {
        // Contexts contexts = Contexts.sharedInstance;
        // GameEntity e      = contexts.game.CreateEntity();
        // e.AddTimerManager(new Stopwatch(), 0);
        // TimerManagerComponent timerManager = e.timerManager;
        // timerManager.gameTimer.Restart();
        // timerManager.lastTime = timerManager.gameTimer.ElapsedTicks;
        long sceneTimerEntityId     = IdGenerator.NextSceneTimerId();
        GameEntity sceneTimerEntity = gameContext.CreateEntity();
        var index = GameComponentsLookup.Timer;
        var component = (TimerComponent)sceneTimerEntity.CreateComponent(index, typeof(TimerComponent));
        component.curFrame = 0;
        component.hertz = 60;
        component.accumulator = 0;
        component.isUnitTimer = false;
        component.NewFrameTimer(InvokeType.GameLoop, gameLoop);
        sceneTimerEntity.AddComponent(index, component);
        sceneTimerEntity.AddId(sceneTimerEntityId);

        _gameTimer.Restart();
        gameContext.SetTimerManager(_gameTimer.ElapsedTicks, 0, sceneTimerEntityId, new Queue<long>());
        _timerEntity = gameContext.timerManagerEntity;
        gameLoop.Initialize();
        //gameContext.timerManager.OnAddUnitTimer(sceneTimerEntityId);
        
        //mainLogic.Start();
    }

    public void Execute()
    {
        var _timer = _timerEntity.timerManager;
        var sceneTimerEntity = gameContext.GetEntityWithId(_timer.sceneTimerId);
        var sceneTimer = sceneTimerEntity.timer;
        long now = _gameTimer.ElapsedTicks;
        _timer.accumulator = Math.Clamp(now - _timer.lastTime, 0, sceneTimer.GetFrameLength());
        _timer.lastTime = now;

        if (sceneTimer.hertz == 0)
        {
            return;
        }

        long DT = sceneTimer.GetFrameLength();
        sceneTimer.accumulator += _timer.accumulator;
        while (sceneTimer.accumulator >= DT)
        {
            sceneTimer.accumulator -= DT;
            //更新逻辑帧
            sceneTimer.Tick();
        }
    }
}
