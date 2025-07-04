using System;
using System.Collections.Generic;

[Serializable]
public sealed class ActionInfo
{
    public string name = string.Empty;
    public string animation = string.Empty;
    public List<ActionFrameInfo> actionFrameInfos = new List<ActionFrameInfo>();
    public int FrameCount => actionFrameInfos.Count;

    public List<ActionCommand> commandList = new();

    public int priority = 0;
    //下一个自然动作
    public string autoNextActionName = string.Empty;
    public bool autoTerminate = false;
    //public List<ActionFrame> mActionFrames;
    public bool keepPlayingAnim = false;
    public CancelTag[] cancelTags = new CancelTag[0];
    public BeCanceledTag[] beCanceledTags = new BeCanceledTag[0];
    public TempBeCancelledTag[] tempBeCancelledTags = new TempBeCancelledTag[0];
    public HitInfo hitInfo;

    public ScriptMethodInfo rootMotionTween;
    public bool ApplyGravity;
}