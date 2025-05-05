using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class TimerComponentSystem : IExecuteSystem
{
    private readonly GameContext _gameContext;
    private readonly long tickPerSecond = TimeSpan.TicksPerSecond;
    public TimerComponentSystem(Contexts contexts)
    {
        _gameContext = contexts.game;
    }

    public void Execute()
    {
        var timerManager     = _gameContext.timerManager;
        var sceneTimerEntity = _gameContext.GetEntityWithId(timerManager.sceneTimerId);
        var sceneTimer       = sceneTimerEntity.timer;
        if (sceneTimer == null)
        {
            return;
        }
        long prefame = sceneTimer.GetNow();
        sceneTimer.UpdateFrame(timerManager.accumulator);
        long nowFrame = sceneTimer.GetNow();
        long _DT = nowFrame - prefame;
        while (_DT-- > 0)
        {
            Debug.Log("TimerComponentSystem Execute:" + (nowFrame - _DT));
        }
        //GameEntity sceneTimerEntity = Contexts.sharedInstance.game.GetEn
    }
}