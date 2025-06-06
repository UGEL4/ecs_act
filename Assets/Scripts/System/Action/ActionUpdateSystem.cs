using System.Collections.Generic;
using Entitas;
using UnityEngine;

public sealed class ActionUpdateSystem : IExecuteSystem
{
    private readonly Contexts contexts;
    private readonly IGroup<GameEntity> actionEntities;

    public ActionUpdateSystem(Contexts contexts)
    {
        this.contexts = contexts;
        actionEntities = contexts.game.GetGroup(GameMatcher.AllOf(
            //GameMatcher.Action,
            GameMatcher.CurrentAction,
            GameMatcher.Timer,
            GameMatcher.InputToCommand
        ));
    }

    public void Execute()
    {
        foreach (var e in actionEntities.GetEntities())
        {
            var timer = e.timer;
            var action = e.action;

            long curFrame = timer.curFrame;
            var curAction = e.currentAction;
            curAction.currentFrame += 1;
            if (curAction.currentFrame >= curAction.value.FrameCount)
            {
                curAction.currentFrame = 0;
            }
            var input = e.inputToCommand;
            var preorderList = e.preorderAction.value;
            foreach (var act in action.actions)
            {
                if (CanActionCancelCurrentAction(act, true, input, curAction.currentFrame, curAction.beCanceledTags, out CancelTag foundTag, out BeCanceledTag beCanceledTag))
                {
                    preorderList.Add(new PreorderActionInfo(act.name, act.priority + foundTag.priority + beCanceledTag.priority));
                }
            }
            if (preorderList.Count <= 0 && (curAction.currentFrame == 0 || curAction.value.autoTerminate))
            {
                //Debug.Log($"KeepAction:{curFrame}");
                preorderList.Add(new PreorderActionInfo(curAction.value.autoNextActionName));
            }

            if (preorderList.Count > 0)
            {
                preorderList.Sort((a, b) => a.Priority > b.Priority ? -1 : 1);
                if (preorderList[0].ActionId == curAction.value.name && curAction.value.keepPlayingAnim)
                {
                    KeepAction(curAction.value);
                }
                else
                {
                    ChangeAction(e, preorderList[0].ActionId, preorderList[0].FromFrameIndex);
                }
            }
            preorderList.Clear();

            //更新当前帧的碰撞数据
            UpdateFrameHitBoxes(e, curAction.value.actionFrameInfos[curAction.currentFrame]);
        }
    }

    bool CanActionCancelCurrentAction(
        ActionInfo actionInfo,
        bool checkCommand,
        InputToCommandComponent input,
        long curFrame,
        List<BeCanceledTag> beCanceledTagList,
        out CancelTag foundTag,
        out BeCanceledTag beCabceledTag
    )
    {
        foundTag = new CancelTag();
        beCabceledTag = new BeCanceledTag();
        foreach (BeCanceledTag bcTagInfo in beCanceledTagList)
        {
            bool tagFit = false;
            foreach (string bcTagName in bcTagInfo.cancelTag)
            {
                //if (!(_wasPercentage <= bcTagInfo.range.max && curPercent >= bcTagInfo.range.min)) continue;
                //Log.SimpleLog.Info("CanActionCancelCurrentAction:", mLastFrameIndex, mCurrentFrameIndex);
                if (!(curFrame <= bcTagInfo.validRange.max && curFrame >= bcTagInfo.validRange.min)) continue;
                foreach (CancelTag cTag in actionInfo.cancelTags)
                {
                    if (bcTagName == cTag.tag)
                    {
                        // if (actionInfo.SelfLoopCount > 0 && ActionLoopCount > actionInfo.SelfLoopCount)
                        // {
                        //     continue;
                        // }
                        tagFit = true;
                        foundTag = cTag;
                        beCabceledTag = bcTagInfo;
                        break;
                    }
                }
                if (tagFit) break;
            }
            if (!tagFit) continue;

            if (checkCommand)
            {
                foreach (ActionCommand cmd in actionInfo.commandList)
                {
                    if (input.ActionOccur(cmd, curFrame)) return true;
                }
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    void KeepAction(ActionInfo actionInfo)
    {
        //Debug.Log($"KeepAction:{actionInfo.name}");
    }

    void ChangeAction(GameEntity target, string actionName, int fromFrameIndex)
    {
        ActionInfo foundAction = null;
        List<ActionInfo> actionList = target.action.actions;
        foreach (var action in actionList)
        {
            if (actionName == action.name)
            {
                foundAction = action;
                break;
            }
        }
        if (foundAction != null)
        {
            CurrentActionComponent curAction = target.currentAction;
            Debug.Log($"ChangeAction: cur:{curAction.value.name}, new:{foundAction.name}");
            curAction.value = foundAction;
            curAction.beCanceledTags.Clear();
            foreach (var tag in foundAction.beCanceledTags)
            {
                curAction.beCanceledTags.Add(tag);
            }
            //动作切换之后，开启的tempBeCancelTag，这是根据ActionChangeInfo的数据添加进来的
            if (target.hasTempBeCancelTag)
            {
                foreach (var tag in target.tempBeCancelTag.values)
                {
                    curAction.beCanceledTags.Add(BeCanceledTag.FromTemp(tag, 0));
                }

            }
            curAction.currentFrame = fromFrameIndex;
        }
    }


    void UpdateFrameHitBoxes(GameEntity e, ActionFrameInfo frameInfo)
    {
        var hitBoxComp = e.hitBox;
        hitBoxComp.values.Clear();
        if (frameInfo.hitBoxes != null)
        {
            hitBoxComp.values.AddRange(frameInfo.hitBoxes);
        }

        var attackHitBoxComp = e.attackHitBox;
        if (frameInfo.attackHitBoxes != null)
        {
            attackHitBoxComp.values.AddRange(frameInfo.attackHitBoxes);
        }
    }

}