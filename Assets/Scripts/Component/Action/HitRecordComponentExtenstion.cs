public static class HitRecordComponentExtenstion
{
    public static HitRecordInfo GetHitRecordInfo(this HitRecordComponent self, long entityId)
    {
        var values = self.values;
        for (int i = 0; i < values.Count; i++)
        {
            if (entityId == values[i].targetId)
            {
                return values[i];
            }
        }
        return null;
    }

    public static void AddHitRecord(this HitRecordComponent self, GameEntity attacker, GameEntity target)
    {
        HitRecordInfo rc = self.GetHitRecordInfo(target.id.value);
        if (rc != null)
        {
            rc.canHitTimes -= 1;
            rc.cooldown = attacker.currentAction.value.hitInfo.frameToHitAgain;
        }
        else
        {
            rc = new HitRecordInfo
            {
                targetId = target.id.value,
                canHitTimes = (short)(attacker.currentAction.value.hitInfo.canHitCount - 1),
                cooldown = attacker.currentAction.value.hitInfo.frameToHitAgain
            };
            self.values.Add(rc);
        }
    }
}