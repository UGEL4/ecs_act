//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameContext {

    public GameEntity timerManagerEntity { get { return GetGroup(GameMatcher.TimerManager).GetSingleEntity(); } }
    public TimerManagerComponent timerManager { get { return timerManagerEntity.timerManager; } }
    public bool hasTimerManager { get { return timerManagerEntity != null; } }

    public GameEntity SetTimerManager(long newLastTime, long newAccumulator, long newSceneTimerId, System.Collections.Generic.Queue<long> newIsntanceIds) {
        if (hasTimerManager) {
            throw new Entitas.EntitasException("Could not set TimerManager!\n" + this + " already has an entity with TimerManagerComponent!",
                "You should check if the context already has a timerManagerEntity before setting it or use context.ReplaceTimerManager().");
        }
        var entity = CreateEntity();
        entity.AddTimerManager(newLastTime, newAccumulator, newSceneTimerId, newIsntanceIds);
        return entity;
    }

    public void ReplaceTimerManager(long newLastTime, long newAccumulator, long newSceneTimerId, System.Collections.Generic.Queue<long> newIsntanceIds) {
        var entity = timerManagerEntity;
        if (entity == null) {
            entity = SetTimerManager(newLastTime, newAccumulator, newSceneTimerId, newIsntanceIds);
        } else {
            entity.ReplaceTimerManager(newLastTime, newAccumulator, newSceneTimerId, newIsntanceIds);
        }
    }

    public void RemoveTimerManager() {
        timerManagerEntity.Destroy();
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public TimerManagerComponent timerManager { get { return (TimerManagerComponent)GetComponent(GameComponentsLookup.TimerManager); } }
    public bool hasTimerManager { get { return HasComponent(GameComponentsLookup.TimerManager); } }

    public void AddTimerManager(long newLastTime, long newAccumulator, long newSceneTimerId, System.Collections.Generic.Queue<long> newIsntanceIds) {
        var index = GameComponentsLookup.TimerManager;
        var component = (TimerManagerComponent)CreateComponent(index, typeof(TimerManagerComponent));
        component.lastTime = newLastTime;
        component.accumulator = newAccumulator;
        component.sceneTimerId = newSceneTimerId;
        component.isntanceIds = newIsntanceIds;
        AddComponent(index, component);
    }

    public void ReplaceTimerManager(long newLastTime, long newAccumulator, long newSceneTimerId, System.Collections.Generic.Queue<long> newIsntanceIds) {
        var index = GameComponentsLookup.TimerManager;
        var component = (TimerManagerComponent)CreateComponent(index, typeof(TimerManagerComponent));
        component.lastTime = newLastTime;
        component.accumulator = newAccumulator;
        component.sceneTimerId = newSceneTimerId;
        component.isntanceIds = newIsntanceIds;
        ReplaceComponent(index, component);
    }

    public void RemoveTimerManager() {
        RemoveComponent(GameComponentsLookup.TimerManager);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherTimerManager;

    public static Entitas.IMatcher<GameEntity> TimerManager {
        get {
            if (_matcherTimerManager == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TimerManager);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherTimerManager = matcher;
            }

            return _matcherTimerManager;
        }
    }
}
