using System;

[Serializable]
public struct CancelTag
{
    public string tag;
    public int priority;
    public int startFromFrameIndex;
}

[Serializable]
public struct BeCanceledTag
{
    public string[] cancelTag;
    public int priority;
    public FrameIndexRange validRange;


    public static BeCanceledTag FromTemp(TempBeCancelledTag tag, int fromFrameIndex) => new BeCanceledTag
    {
        cancelTag = tag.cancelTag,
        priority  = tag.increasePriority,
        validRange = new FrameIndexRange(fromFrameIndex, fromFrameIndex + tag.frameCount)
    };
}

[Serializable]
public struct TempBeCancelledTag
{
    /// <summary>
    /// 因为需要被索引，所以需要一个id
    /// </summary>
    public string id;

    /// <summary>
    /// 在当前动作中，有多少帧是开启的
    /// 从开启的帧往后算
    /// </summary>
    public int frameCount;
    
    /// <summary>
    /// 可以Cancel的CancelTag
    /// </summary>
    public string[] cancelTag;
    
    /// <summary>
    /// 当从这里被Cancel，动作会增加多少优先级
    /// </summary>
    public int increasePriority;
}