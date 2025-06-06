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

    public PreorderActionInfo(string actionId, int priority = 0, int fromFrameIndex = 0)
    {
        ActionId = actionId;
        Priority = priority;
        FromFrameIndex = fromFrameIndex;
    }

}