//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public HitBoxComponent hitBox { get { return (HitBoxComponent)GetComponent(GameComponentsLookup.HitBox); } }
    public bool hasHitBox { get { return HasComponent(GameComponentsLookup.HitBox); } }

    public void AddHitBox(System.Collections.Generic.List<HitBox> newValues) {
        var index = GameComponentsLookup.HitBox;
        var component = (HitBoxComponent)CreateComponent(index, typeof(HitBoxComponent));
        component.values = newValues;
        AddComponent(index, component);
    }

    public void ReplaceHitBox(System.Collections.Generic.List<HitBox> newValues) {
        var index = GameComponentsLookup.HitBox;
        var component = (HitBoxComponent)CreateComponent(index, typeof(HitBoxComponent));
        component.values = newValues;
        ReplaceComponent(index, component);
    }

    public void RemoveHitBox() {
        RemoveComponent(GameComponentsLookup.HitBox);
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

    static Entitas.IMatcher<GameEntity> _matcherHitBox;

    public static Entitas.IMatcher<GameEntity> HitBox {
        get {
            if (_matcherHitBox == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.HitBox);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherHitBox = matcher;
            }

            return _matcherHitBox;
        }
    }
}
