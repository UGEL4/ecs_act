
public class GameLoopInvoke : Timer<GameLoop>
{
    protected override void Run(GameLoop t, long frame)
    {
        t.OnUpdate(frame);
    }
}