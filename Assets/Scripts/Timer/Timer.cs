public struct TimerCallback
{
    public object Args;
    public long frame;
}

public abstract class Timer<T> : AInvokeHandler<TimerCallback> where T : class
{
    public override void Handle(TimerCallback a)
    {
        Run(a.Args as T, a.frame);
    }

    protected abstract void Run(T t, long frame);
}