//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public LevelAddEntityCmdComponent levelAddEntityCmd { get { return (LevelAddEntityCmdComponent)GetComponent(GameComponentsLookup.LevelAddEntityCmd); } }
    public bool hasLevelAddEntityCmd { get { return HasComponent(GameComponentsLookup.LevelAddEntityCmd); } }

    public void AddLevelAddEntityCmd(long newLevelId, long newEntityId, string newAssetName) {
        var index = GameComponentsLookup.LevelAddEntityCmd;
        var component = (LevelAddEntityCmdComponent)CreateComponent(index, typeof(LevelAddEntityCmdComponent));
        component.levelId = newLevelId;
        component.entityId = newEntityId;
        component.assetName = newAssetName;
        AddComponent(index, component);
    }

    public void ReplaceLevelAddEntityCmd(long newLevelId, long newEntityId, string newAssetName) {
        var index = GameComponentsLookup.LevelAddEntityCmd;
        var component = (LevelAddEntityCmdComponent)CreateComponent(index, typeof(LevelAddEntityCmdComponent));
        component.levelId = newLevelId;
        component.entityId = newEntityId;
        component.assetName = newAssetName;
        ReplaceComponent(index, component);
    }

    public void RemoveLevelAddEntityCmd() {
        RemoveComponent(GameComponentsLookup.LevelAddEntityCmd);
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

    static Entitas.IMatcher<GameEntity> _matcherLevelAddEntityCmd;

    public static Entitas.IMatcher<GameEntity> LevelAddEntityCmd {
        get {
            if (_matcherLevelAddEntityCmd == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.LevelAddEntityCmd);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherLevelAddEntityCmd = matcher;
            }

            return _matcherLevelAddEntityCmd;
        }
    }
}
