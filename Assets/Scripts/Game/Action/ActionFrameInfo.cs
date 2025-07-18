using System;
using System.Collections.Generic;

[Serializable]
public class ActionFrameInfo
{
    public int frameId;
    public int loopFrame;
    public List<CancelTag> cancelTags = new();
    public AttackHitBox[] attackHitBoxes;
    public ColliderBox[] colliderBoxes;
    public HitBox[] hitBoxes;
    public FrameCapsuleDimensions capsuleDimensions;
    public float moveInputAcceptance;
}