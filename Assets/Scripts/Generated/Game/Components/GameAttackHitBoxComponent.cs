//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public AttackHitBoxComponent attackHitBox { get { return (AttackHitBoxComponent)GetComponent(GameComponentsLookup.AttackHitBox); } }
    public bool hasAttackHitBox { get { return HasComponent(GameComponentsLookup.AttackHitBox); } }

    public void AddAttackHitBox(System.Collections.Generic.List<AttackHitBox> newValues) {
        var index = GameComponentsLookup.AttackHitBox;
        var component = (AttackHitBoxComponent)CreateComponent(index, typeof(AttackHitBoxComponent));
        component.values = newValues;
        AddComponent(index, component);
    }

    public void ReplaceAttackHitBox(System.Collections.Generic.List<AttackHitBox> newValues) {
        var index = GameComponentsLookup.AttackHitBox;
        var component = (AttackHitBoxComponent)CreateComponent(index, typeof(AttackHitBoxComponent));
        component.values = newValues;
        ReplaceComponent(index, component);
    }

    public void RemoveAttackHitBox() {
        RemoveComponent(GameComponentsLookup.AttackHitBox);
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

    static Entitas.IMatcher<GameEntity> _matcherAttackHitBox;

    public static Entitas.IMatcher<GameEntity> AttackHitBox {
        get {
            if (_matcherAttackHitBox == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.AttackHitBox);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherAttackHitBox = matcher;
            }

            return _matcherAttackHitBox;
        }
    }
}
