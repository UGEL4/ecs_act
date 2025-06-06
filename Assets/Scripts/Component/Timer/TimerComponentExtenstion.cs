using System;
using System.Collections.Generic;
using System.Linq;

public static class TimerComponentExtenstion
{
    public static long GetNow(this TimerComponent self)
    {
        return self.curFrame;
    }

    public static void UpdateFrame(this TimerComponent self, long accumulator)
    {
        if (self.hertz == 0)
        {
            return;
        }

        long dt = self.GetFrameLength();
        self.accumulator += accumulator;
        while (self.accumulator >= dt)
        {
            self.accumulator -= dt;
            self.Tick();
        }
    }

    public static long GetFrameLength(this TimerComponent self)
    {
        return self.hertz == 0 ? 0 : TimeSpan.TicksPerSecond / self.hertz;
    }

    public static void Tick(this TimerComponent self, long tickCount = 1)
    {
        self.curFrame += tickCount;
        if (self.timerId.Count == 0)
        {
            return;
        }

        if (self.curFrame < self.minFrame)
        {
            return;
        }

        foreach (long k in self.timerId.Keys)
        {
            if (k > self.curFrame)
            {
                self.minFrame = k;
                break;
            }

            self.timeOutTime.Enqueue(k);
        }

        while (self.timeOutTime.Count > 0)
        {
            long time = self.timeOutTime.Dequeue();
            var list = self.timerId[time];
            for (int i = 0; i < list.Count; i++)
            {
                long timerId = list[i];
                self.timeOutTimerIds.Enqueue(timerId);
            }
            self.timerId.Remove(time);
        }

        while (self.timeOutTimerIds.Count > 0)
        {
            long timerId = self.timeOutTimerIds.Dequeue();
            if (!self.timerActionMap.Remove(timerId, out TimerAction timerAction))
            {
                continue;
            }
            self.Run(timerAction);
        }
    }

    private static void Run(this TimerComponent self, TimerAction timerAction)
    {
        switch (timerAction.timerClass)
        {
            case TimerClass.RepeatTimer:
                {
                    timerAction.startFrame = self.curFrame;
                    self.AddTimer(timerAction);
                    EventSystem.Instance.Invoke(timerAction.invokeType, new TimerCallback() { Args = timerAction.Object, frame = self.curFrame });
                    //timerAction.Recycle();
                    break;
                }
        }
    }

    private static void AddTimer(this TimerComponent self, TimerAction timer)
    {
        long tillFrame = timer.startFrame + timer.frame;
        self.timerId.Add(tillFrame, timer.id);
        self.timerActionMap.Add(timer.id, timer);

        if (tillFrame < self.minFrame)
        {
            self.minFrame = tillFrame;
        }
    }

    private static long GetTd(this TimerComponent self)
    {
        return ++self.idGenerator;
    }

    public static long NewFrameTimer(this TimerComponent self, int invokeType, object args)
    {
        return self.NewRepeatedTimer(1, invokeType, args);
    }

    private static long NewRepeatedTimer(this TimerComponent self, long frame, int invokeType, object args)
    {
        TimerAction timerAction = TimerAction.Create(self.GetTd(), TimerClass.RepeatTimer, self.curFrame, frame, args, invokeType);
        self.AddTimer(timerAction);
        return timerAction.id;
    }
}