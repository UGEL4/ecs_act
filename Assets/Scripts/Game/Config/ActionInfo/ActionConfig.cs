using System.Collections.Generic;
using UnityEngine;

namespace ACTGame.Action
{

    public class ActionConfig : ScriptableObject
    {
        public string ActionName;
        public int FrameCount;
        public string Animation                     = string.Empty;
        public List<CancelData> CancelDatas         = new();
        public List<ConfigCancelTag> BeCanceledTags = new();
        public List<ActionCommand> CommandList      = new();
        public int Priority                         = 0;
        // 下一个自然动作
        public string AutoNextActionName = string.Empty;
        public bool AutoTerminate        = false;
        public bool KeepPlayingAnim      = false;
        public HitInfo HitInfo;
        public ScriptMethodInfo RootMotionTween;
        public ScriptMethodInfo EnterActionEvent;
        public bool ApplyGravity                              = true;
        public List<MoveInputAcceptance> MoveInputAcceptances = new();

        public List<ActionFrameInfo> InitFrameList()
        {
            List<ActionFrameInfo> frames = new(FrameCount);
            for (int i = 0; i < FrameCount; i++)
            {
                ActionFrameInfo frame      = ActionFrameInfo.Allocate();
                frame.frameId              = i;
                frame.loopFrame            = 1;
                List<CancelTag> cancelTags = new();
                foreach (var beTag in BeCanceledTags)
                {
                    if (beTag.validRange.min <= i && i <= beTag.validRange.max)
                    {
                        foreach (var tag in beTag.cancelTags)
                        {
                            cancelTags.Add(tag);
                        }
                    }
                }
                frame.cancelTags = cancelTags.ToArray();

                // frame.capsuleDimensions = new FrameCapsuleDimensions {
                //     frameRange = new FrameIndexRange(-1, -1),
                //     dimensions = new ACTGame.Math.CapsuleDimensions()
                // };
                // foreach (var capsule in frameCapsuleDimensions)
                // {
                //     if (capsule.frameRange.min <= i && i <= capsule.frameRange.max)
                //     {
                //         frame.capsuleDimensions = capsule;
                //         break;
                //     }
                // }

                frame.moveInputAcceptance = 0f;
                foreach (var inputAcceptance in MoveInputAcceptances)
                {
                    if (inputAcceptance.FrameRange.min <= i && i <= inputAcceptance.FrameRange.max &&
                        (frame.moveInputAcceptance <= 0 || inputAcceptance.Rate < frame.moveInputAcceptance))
                    {
                        frame.moveInputAcceptance = inputAcceptance.Rate;
                    }
                }

                frames.Add(frame);
            }
            return frames;
        }
    
        public ActionInfo GetActionInfo()
        {
            ActionInfo actionInfo = ActionInfo.Allocate();

            actionInfo.name               = ActionName;
            actionInfo.animation          = Animation;
            actionInfo.actionFrameInfos   = InitFrameList();
            actionInfo.currentFrame       = actionInfo.actionFrameInfos.Count > 0 ? actionInfo.actionFrameInfos[0] : null;
            actionInfo.commandList        = CommandList;
            actionInfo.priority           = Priority;
            actionInfo.autoNextActionName = AutoNextActionName;
            actionInfo.autoTerminate      = AutoTerminate;
            actionInfo.keepPlayingAnim    = KeepPlayingAnim;
            actionInfo.cancelDatas        = CancelDatas.ToArray();
            actionInfo.hitInfo            = HitInfo;
            actionInfo.rootMotionTween    = RootMotionTween;
            actionInfo.enterActionEvent   = EnterActionEvent;
            actionInfo.ApplyGravity       = ApplyGravity;

            return actionInfo;
        }
    
    }
}