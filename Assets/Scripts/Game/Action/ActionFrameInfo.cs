using System;
using System.Collections.Generic;

[Serializable]
public class ActionFrameInfo
{
    public int FrameId;
    public int LoopFrame;
    public CancelTag[] CancelTags;
    public AttackHitBox[] AttackHitBoxes;
    public ColliderBox[] ColliderBoxes;
    public HitBox[] HitBoxes;
    public FrameCapsuleDimensions CapsuleDimensions;
    public float MoveInputAcceptance;

    public void Reset()
    {
        FrameId             = 0;
        LoopFrame           = 0;
        CancelTags          = null;
        AttackHitBoxes      = null;
        ColliderBoxes       = null;
        HitBoxes            = null;
        CapsuleDimensions   = new();
        MoveInputAcceptance = 0f;
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