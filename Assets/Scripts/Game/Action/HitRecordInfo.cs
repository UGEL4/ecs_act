public class HitRecordInfo
{
    public long targetId;
    public short canHitTimes;
    public short cooldown;

    public void Update()
    {
        if (cooldown > 0)
        {
            cooldown -= 1;
        }
    }
}