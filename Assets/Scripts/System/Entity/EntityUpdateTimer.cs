public class EntityUpdateTimer : Timer<EntityUpdateLogic>
{
    protected override void Run(EntityUpdateLogic t, long frame)
    {
        t.Update(frame);
    }
}