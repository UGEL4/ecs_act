using System.Collections.Generic;
using Entitas;

[Game]
public class TimerComponent : IComponent
{
    public long curFrame = 0;
    public int hertz = 60;
    public long accumulator = 0;
    public bool isUnitTimer = false;

    public MultiMap<long, long> timerId = new();
    public Queue<long> timeOutTime = new();
    public Queue<long> timeOutTimerIds = new();
    public Dictionary<long, TimerAction> timerActionMap = new();
    public long minFrame = long.MaxValue;
    public long idGenerator = 0;
}