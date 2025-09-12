using System;
using System.Collections.Generic;

namespace ACTGame
{

    [Serializable]
    public struct NextAutoActionInfo
    {
        public string ActionName;
        public float FadeOutPercentage;
        public float FadeInPercentage;
        public float AnimStartFromPercentage;
        public int InFromFrame;
    }
}

[Serializable]
public sealed class ActionInfo
{
    public string name      = string.Empty;
    public string animation = string.Empty;

    public List<ActionFrameInfo> actionFrameInfos  = new List<ActionFrameInfo>();
    public int FrameCount                         => actionFrameInfos.Count;
    public ActionFrameInfo currentFrame;

    public List<ActionCommand> commandList = new();

    public int priority = 0;
    // 下一个自然动作
    public ACTGame.NextAutoActionInfo nextAutoActionInfo;
    public bool autoTerminate        = false;
    // public List<ActionFrame> mActionFrames;
    public bool keepPlayingAnim         = false;
    public CancelData[] cancelDatas;
    public HitInfo hitInfo;

    public ScriptMethodInfo rootMotionTween;
    public ScriptMethodInfo enterActionEvent;
    public bool ApplyGravity = true;

    public static ActionInfo Allocate()
    {
        return new ActionInfo();
    }
}