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
        if (!sceneTimerEntity.hasTimer)
        {
            return;
        }
        var sceneTimer = sceneTimerEntity.timer;
        if (sceneTimer.hertz == 0)
        {
            return;
        }

            //更新子时间轴
            long instanceCount = timerManager.isntanceIds.Count;
            while (instanceCount-- > 0)
            {
                long instanceId = timerManager.isntanceIds.Dequeue();
                var e = _gameContext.GetEntityWithId(instanceId);
                if (e == null)
                {
                    continue;
                }
                if (!e.hasTimer)
                {
                    continue;
                }
                var timerComp = e.timer;
                timerManager.isntanceIds.Enqueue(instanceId);
                //SceneTimer逻辑帧帧长是固定的， 永远是 1 / 60 s
                timerComp.UpdateFrame(166666);
            }
    }
}