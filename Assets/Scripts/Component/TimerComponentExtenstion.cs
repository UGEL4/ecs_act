using System;

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
    }
}