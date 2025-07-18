public static class ActionComponentExtenstion
{
    /// <summary>
    /// 开启临时的CancelTag
    /// </summary>
    /// <param name="tag"></param>
    public static void AddTempBeCancelledTag(this TempBeCancelTagComponent self, TempBeCancelledTag tag)
    {
        self.values.Add(tag);
    }

    /// <summary>
    /// 根据TempBeCancelledTag的id来开启
    /// </summary>
    /// <param name="target"></param>
    /// <param name="tempTagId"></param>
    // public static void AddTempBeCancelledTag(this TempBeCancelTagComponent self, GameEntity target, string tempTagId)
    // {
    //     foreach (var haveTag in target.currentAction.value.tempBeCancelledTags)
    //     {
    //         if (tempTagId == haveTag.id)
    //         {
    //             self.values.Add(haveTag);
    //         }
    //     }
    // }

    public static CurrentActionComponent CreateCurrentAction(this GameEntity self)
    {
        var index     = GameComponentsLookup.CurrentAction;
        var component = (CurrentActionComponent)self.CreateComponent(index, typeof(CurrentActionComponent));
        return component;
    }

    public static void ReplaceCurrentAction(this GameEntity self, CurrentActionComponent comp)
    {
        var index = GameComponentsLookup.CurrentAction;
        self.ReplaceComponent(index, comp);
    }
}