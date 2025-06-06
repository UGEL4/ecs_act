using System.Collections.Generic;
using Entitas;

[Game]
public class AttackHitResultComponent : IComponent
{
    //<attackHitBoxIndex, AttackHitResultData>
    //public Dictionary<int, AttackHitResultData> value;

    //<entityId, hitResult>
    public List<AttackHitTargetResultData> value;
}