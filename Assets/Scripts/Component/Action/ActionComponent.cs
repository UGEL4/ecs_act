using System.Collections.Generic;
using Entitas;

[Game]
public sealed class ActionComponent : IComponent
{
    public long entityId;
    public List<ActionInfo> actions;
}

[Game]
public sealed class CurrentActionComponent : IComponent
{
    public ActionInfo value;
    public List<BeCanceledTag> beCanceledTags;
    /// <summary>
    /// 当前更新到了第几帧
    /// </summary>
    public int currentFrame = 0;
}
