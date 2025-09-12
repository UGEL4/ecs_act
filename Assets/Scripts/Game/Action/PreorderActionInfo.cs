/// <summary>
/// 预约一个Action，作为下一个Action的候选人
/// </summary>
public struct PreorderActionInfo
{
    /// <summary>
    /// 这个action的id
    /// </summary>
    public string ActionId;

    /// <summary>
    /// 这条预约信息的优先级，最后冒泡出来最高的就是要换成的
    /// </summary>
    public int Priority;

    public int FromFrameIndex;
    /// <summary>
    /// 动作融合的百分比时间
    /// </summary>
    public float TransitionNormalized;

    /// <summary>
    /// 新的action对应的动画，从百分之多少的位置开始播放
    /// </summary>
    public float FromNormalized;

    public PreorderActionInfo(string actionId, int priority = 0, int fromFrameIndex = 0, float transitionNormalized = 0f, float fromNormalized = 0f)
    {
        ActionId = actionId;
        Priority = priority;
        FromFrameIndex = fromFrameIndex;
        TransitionNormalized = transitionNormalized;
        FromNormalized = fromNormalized;
    }

}