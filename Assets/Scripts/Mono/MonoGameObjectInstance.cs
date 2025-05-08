using System.Collections.Generic;
using UnityEngine;

public static class MonoGameObjectInstance
{
    public static Dictionary<long, GameObject> worldMap = new();
    public static Dictionary<long, GameObject> levelMap = new();
    public static GameObject GetWorld(long worldId)
    {
        worldMap.TryGetValue(worldId, out GameObject world);
        return world;
    }

    public static GameObject GetLevel(long levelId)
    {
        levelMap.TryGetValue(levelId, out GameObject level);
        return level;
    }

    public static void AddWorld(long worldId, GameObject world)
    {
        if (worldMap.ContainsKey(worldId))
        {
            return;
        }
        worldMap.Add(worldId, world);
    }

    public static void AddLevel(long levelId, GameObject level)
    {
        if (levelMap.ContainsKey(levelId))
        {
            return;
        }
        levelMap.Add(levelId, level);
    }

    public static void ClearWorld()
    {
        worldMap.Clear();
    }

    public static void ClearLevel()
    {
        levelMap.Clear();
    }
}