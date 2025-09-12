using System;
using System.Collections.Generic;

[Serializable]
public struct CancelTag
{
    public string Tag;
    public int Priority;
    public bool IsNowActive;
}


/// <summary>
/// CancelTag的配置数据
/// 在一段有效帧下，可以有多个CancelTag生效
/// </summary>
[Serializable]
public struct ConfigCancelTag
{
    public List<CancelTag> CancelTags;
    public FrameIndexRange ValidRange;
}

public struct BeCanceledTag
{
    public string[] CancelTag;
    public int Priority;
    public float FadeOutPercentage;
    public FrameIndexRange ValidRange;

    public static BeCanceledTag FromTemp(TempBeCancelledTag tag, int fromFrameIndex) => new BeCanceledTag {
        CancelTag         = tag.CancelTag,
        Priority          = tag.IncreasePriority,
        ValidRange        = new FrameIndexRange(fromFrameIndex, fromFrameIndex + tag.FrameCount),
        FadeOutPercentage = tag.FadeOutPercentage
    };
}

[Serializable]
public struct TempBeCancelledTag
{
    /// <summary>
    /// 因为需要被索引，所以需要一个id
    /// </summary>
    public string Id;

    /// <summary>
    /// 在当前动作中，有多少帧是开启的
    /// 从开启的帧往后算
    /// </summary>
    public int FrameCount;
    
    /// <summary>
    /// 可以Cancel的CancelTag
    /// </summary>
    public string[] CancelTag;
    
    /// <summary>
    /// 当从这里被Cancel，动作会增加多少优先级
    /// </summary>
    public int IncreasePriority;

    /// <summary>
    /// 动画融合出去的时间
    /// </summary>
    public float FadeOutPercentage;
}