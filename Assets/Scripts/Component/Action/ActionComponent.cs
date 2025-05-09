using System.Collections.Generic;
using Entitas;

[Game]
public sealed class ActionComponent : IComponent
{
    public long entityId;
    public List<ActionInfo> actions;
    public ActionInfo currentAction;
    public int currentActionFrame;
}