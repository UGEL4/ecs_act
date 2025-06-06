using System;

[Serializable]
public struct HitInfo
{
    //多少帧后能再次命中
    public short frameToHitAgain;
    // 能击中几次
    public short canHitCount;
}