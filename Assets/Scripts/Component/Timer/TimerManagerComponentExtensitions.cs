public static class TimerManagerComponentExtensitions
{
    public static void OnAddUnitTimer(this TimerManagerComponent self, long entityId)
    {
        self.isntanceIds.Enqueue(entityId);
    }

    public static void OnRemoveUnitTimer(this TimerManagerComponent self, long entityId)
    {
        long instanceCount = self.isntanceIds.Count;
        while (instanceCount-- > 0)
        {
            long instanceId = self.isntanceIds.Dequeue();
            if (instanceId == entityId)
            {
                continue;
            }
            self.isntanceIds.Enqueue(instanceId);
        }
    }

    public static TimerComponent GetSceneTimer(this TimerManagerComponent self, GameContext context)
    {
        long sceneTimerId = self.sceneTimerId;
        var entity        = context.GetEntityWithId(sceneTimerId);
        if (entity == null)
        {
            return null;
        }
        if (!entity.hasTimer)
        {
            return null;
        }
        return entity.timer;
    }
}