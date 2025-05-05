using System.Threading;

public static class IdGenerator
{
    private static long _sceneTimerIdCounter;
    private static long _entityIdCounter;

    public static long NextSceneTimerId() => Interlocked.Increment(ref _sceneTimerIdCounter);
    public static long NextEntityId() => Interlocked.Increment(ref _entityIdCounter);

    public static void ResetSceneTimerIds() => Interlocked.Exchange(ref _sceneTimerIdCounter, 0);
    public static void ResetEntityIds() => Interlocked.Exchange(ref _entityIdCounter, 0);
}