using System;
using System.Collections.Generic;

public class AttackHitResultData
{
    //public int attackBoxIndex; //攻击框索引
    //<entityId, hitboxlist>
    //public Dictionary<long, List<HitBox>> hitMap; //碰到的所有实体的受击框列表

    public Dictionary<int, List<HitBox>> hitMap = new();
    public Dictionary<long, HitBox> bestHitBoxPerTarget = new();

    // 所有碰撞到的受击框列表（用于调试或特殊逻辑）
    public List<HitBox> allHitBoxes = new();
}

public class AttackHitTargetResultData
{
    public long targetEntityId = -1;
    public struct HitResult
    {
        public short attackBoxIndex;
        public short hitBoxIndex; 
    }
    public List<HitResult> hitResults = new List<HitResult>();
}
