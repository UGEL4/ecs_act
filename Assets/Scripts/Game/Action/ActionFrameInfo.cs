using System;

[Serializable]
public struct ActionFrameInfo
{
    public int frameId;
    public int loopFrame;
    public CancelData[] cancelDatas;
    public CancelTag[] tempCancelTags;
    public AttackHitBox[] attackHitBoxes;
    public ColliderBox[] colliderBoxes;
    public HitBox[] hitBoxes;
}