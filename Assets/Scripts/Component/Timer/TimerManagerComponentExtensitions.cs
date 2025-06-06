public static class TimerManagerComponentExtensitions
{
    public static void OnAddUnitTimer(this TimerManagerComponent self, long entityId)
    {
        self.isntanceIds.Enqueue(entityId);
    }

    public static TimerComponent GetSceneTimer(this TimerManagerComponent self, Contexts contexts)
    {
        long sceneTimerId = self.sceneTimerId;
        var entity        = contexts.game.GetEntityWithId(sceneTimerId);
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