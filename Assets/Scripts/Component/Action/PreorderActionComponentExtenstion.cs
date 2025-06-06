using System.Collections.Generic;

public static class PreorderActionComponentExtenstion
{
    /// <summary>
    /// 预约一个动作
    /// </summary>
    /// <param name="acInfo">变换动作信息</param>
    /// <param name="forceDir">如有必要（其实就是byCatalog）得给个动作受力方向</param>
    /// <param name="freezing">如果切换到这个动作，硬直多少秒</param>
    public static void PreorderActionByActionChangeInfo(this PreorderActionComponent self,
        ActionChangeInfo acInfo, List<ActionInfo> AllActions/*, ForceDirection forceDir, float freezing = 0*/)
    {
        switch (acInfo.changeType)
        {
            case ActionChangeType.Keep:
                //既然保持，就啥也不做了
                break;
            /*case ActionChangeType.ChangeByCatalog:
                List<ActionInfo> actions = new List<ActionInfo>();
                foreach (ActionInfo info in AllActions)
                    if (info.catalog == acInfo.param)
                        actions.Add(info);
                if (actions.Count > 0)
                {
                    ActionInfo picked = actions[0];
                    //如果有策划设计的脚本，那就走脚本拿到数据
                    if (PickActionMethod.Methods.ContainsKey(acInfo.param))
                    {
                        picked = PickActionMethod.Methods[acInfo.param](actions, forceDir);
                    }
                    _preorderActions.Add(new PreorderActionInfo
                    {
                        ActionId = picked.id,
                        FromNormalized = acInfo.fromNormalized,
                        Priority = acInfo.priority + picked.priority,
                        TransitionNormalized = acInfo.transNormalized,
                        FreezingAfterChangeAction = freezing
                    });
                }
                break;*/
            case ActionChangeType.ChangeToActionId:
                //找到对应id的动作，如果有的话
                ActionInfo aInfo = GetActionById(AllActions, acInfo.param, out bool found);
                if (found)
                {
                    self.value.Add(new PreorderActionInfo(aInfo.name, aInfo.priority + acInfo.priority, acInfo.stratFrameIndex));
                }
                break;
        }
    }

    public static ActionInfo GetActionById(List<ActionInfo> allActions, string id, out bool found)
    {
        found = false;
        for (int i = 0; i < allActions.Count; i++)
        {
            if (id == allActions[i].name)
            {
                found = true;
                return allActions[i];
            }
        }
        return null;
    }
}