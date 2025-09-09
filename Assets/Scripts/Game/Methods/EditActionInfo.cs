using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FrameCapsuleDimensions
{
    public FrameIndexRange frameRange;
    public ACTGame.Math.CapsuleDimensions dimensions;
}

[CreateAssetMenu(fileName = "ActionInfo", menuName = "动作编辑/ActionInfo")]
public class EditActionInfo : ScriptableObject
{
    public string ActionName;
    public int FrameCount;
    public string Animation                   = string.Empty;
    public List<CancelData> cancelDatas       = new();
    public List<ConfigCancelTag> beCanceledTags = new();
    public List<ActionCommand> commandList    = new();
    public int priority                       = 0;
    // 下一个自然动作
    public string autoNextActionName = string.Empty;
    public bool autoTerminate        = false;
    public bool keepPlayingAnim      = false;
    public HitInfo hitInfo;
    public ScriptMethodInfo rootMotionTween;
    public ScriptMethodInfo enterActionEvent;
    public bool ApplyGravity                              = true;
    public List<MoveInputAcceptance> MoveInputAcceptances = new();
    
    [Header("胶囊体在一些帧中的变化")]
    public List<FrameCapsuleDimensions> frameCapsuleDimensions = new();


    /// <summary>
    /// 初始化帧列表
    /// </summary>
    /// <returns>帧列表</returns>
    public List<ActionFrameInfo> InitFrameList()
    {
        List<ActionFrameInfo> frames = new(FrameCount);
        for (int i = 0; i < FrameCount; i++)
        {
            ActionFrameInfo frame = new ActionFrameInfo();
            frame.frameId         = i;
            frame.loopFrame       = 1;
            int count = 0;
            foreach (var beTag in beCanceledTags)
            {
                if (beTag.validRange.min <= i && i <= beTag.validRange.max)
                {
                    count += beTag.cancelTags.Count;
                }
                // {
                //     var cancelTag = new CancelTag(beTag.cancelTag.tag, beTag.cancelTag.priority, beTag.cancelTag.isNowActive);
                //     frame.cancelTags.Add(cancelTag);
                // }
            }
            frame.cancelTags = new CancelTag[count];
            count = 0;
            for (int j = 0; j < beCanceledTags.Count; j++)
            {
                if (beCanceledTags[j].validRange.min <= i && i <= beCanceledTags[j].validRange.max)
                {
                    for (int k = 0; k < beCanceledTags[j].cancelTags.Count; k++)
                    {
                        frame.cancelTags[count] = beCanceledTags[j].cancelTags[k];
                        count++;
                    }
                    Array.Copy(
                        beCanceledTags[j].cancelTags.ToArray(),
                        frame.cancelTags,
                        beCanceledTags[j].cancelTags.Count
                    );
                }
            }


            frame.capsuleDimensions = new FrameCapsuleDimensions {
                frameRange = new FrameIndexRange(-1, -1),
                dimensions = new ACTGame.Math.CapsuleDimensions()
            };
            foreach (var capsule in frameCapsuleDimensions)
            {
                if (capsule.frameRange.min <= i && i <= capsule.frameRange.max)
                {
                    frame.capsuleDimensions = capsule;
                    break;
                }
            }

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
}