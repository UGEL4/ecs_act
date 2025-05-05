using System.Diagnostics;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Unique]
public sealed class TimerManagerComponent : IComponent
{
    public long lastTime = 0;
    public long accumulator = 0;
    public long sceneTimerId = -1; //主时间轴
}