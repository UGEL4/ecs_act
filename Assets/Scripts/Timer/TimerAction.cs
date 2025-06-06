using UnityEngine;

public enum TimerClass
{
    None,
    RepeatTimer
}

public class TimerAction
{
    public long id;
    public TimerClass timerClass;
    public long startFrame;
    public long frame;
    public object Object;
    public int invokeType;

    public static TimerAction Create(long id, TimerClass timerClass, long startFrame, long frame, object Object, int invokeType)
    {
        TimerAction timerAction = ObjectPool.Instance.Fetch<TimerAction>();
        timerAction.id = id;
        timerAction.timerClass = timerClass;
        timerAction.startFrame = startFrame;
        timerAction.frame = frame;
        timerAction.Object = Object;
        timerAction.invokeType = invokeType;
        return timerAction;
    }

    public void Recycle()
    {
        id = 0;
        timerClass = TimerClass.None;
        startFrame = 0;
        frame = 0;
        Object = null;
        invokeType = 0;
        ObjectPool.Instance.Recycle(this);
    }
}