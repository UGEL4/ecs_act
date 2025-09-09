using System;
using System.Collections.Generic;

[Serializable]
public class ActionFrameInfo
{
    public int frameId;
    public int loopFrame;
    public CancelTag[] cancelTags;
    public AttackHitBox[] attackHitBoxes;
    public ColliderBox[] colliderBoxes;
    public HitBox[] hitBoxes;
    public FrameCapsuleDimensions capsuleDimensions;
    public float moveInputAcceptance;

    public void Reset()
    {
        frameId             = 0;
        loopFrame           = 0;
        cancelTags          = null;
        attackHitBoxes      = null;
        colliderBoxes       = null;
        hitBoxes            = null;
        capsuleDimensions   = new();
        moveInputAcceptance = 0f;
    }

    public void Recycle()
    {
        //Reset();
    }

    public static ActionFrameInfo Allocate()
    {
        ActionFrameInfo result = new ActionFrameInfo();
        return result;
    }
}