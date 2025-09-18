using System;
using System.Collections.Generic;
using ACTGame.Action;
using Entitas;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class ActionUpdateSystem : IExecuteSystem
{
    private readonly Contexts contexts;
    private readonly IGroup<GameEntity> actionEntities;

    public ActionUpdateSystem(Contexts contexts)
    {
        this.contexts  = contexts;
        actionEntities = contexts.game.GetGroup(GameMatcher.AllOf(
        // GameMatcher.Action,
        GameMatcher.CurrentAction,
        GameMatcher.ACTGameComponentTimer,
        GameMatcher.InputToCommand));
    }

    public void Execute()
    {
        foreach (var e in actionEntities.GetEntities())
        {
            //var timer  = e.timer;
            var timer  = e.aCTGameComponentTimer;
            var action = e.action;

            long curFrame = timer.curFrame;
            var curAction = e.currentAction;
            curAction.currentFrame += 1;
            if (curAction.currentFrame >= curAction.value.FrameCount)
            {
                curAction.currentFrame = 0;
            }
            if (e.hasHitRecord)
            {
                // if (!curaction.value.isFreezing)
                var hitRecords = e.hitRecord.values;
                for (int i = 0; i < hitRecords.Count; i++)
                {
                    hitRecords[i].Update();
                }
            }
            var input        = e.inputToCommand;
            var preorderList = e.preorderAction.value;
            foreach (var act in action.actions)
            {
                if (ActionUtility.CanActionCancelCurrentAction(act, curAction, true, input, curFrame, out CancelData foundCancelData, out CancelTag cancelTag))
                {
                    preorderList.Add(new PreorderActionInfo(act.name, act.priority + foundCancelData.Priority + cancelTag.Priority, foundCancelData.StartFrame,
                    foundCancelData.FadeInPercentage, foundCancelData.AnimStartFromPercentage));
                }
            }
            if (preorderList.Count <= 0 && (curAction.currentFrame == 0 || curAction.value.autoTerminate))
            {
                // Debug.Log($"KeepAction:{curFrame}");
                var NextAutoAction = curAction.value.nextAutoActionInfo;
                preorderList.Add(new PreorderActionInfo(NextAutoAction.ActionName, 0, NextAutoAction.InFromFrame, 
                NextAutoAction.FadeInPercentage, NextAutoAction.AnimStartFromPercentage));
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
                    var preorderAction = preorderList[0];
                    ChangeAction(e, preorderAction.ActionId, preorderAction.FromFrameIndex, preorderAction.TransitionNormalized, preorderAction.FromNormalized);
                }
            }
            preorderList.Clear();
            curAction.value.currentFrame = curAction.value.actionFrameInfos[curAction.currentFrame];

            // 更新当前帧的碰撞数据
            UpdateFrameHitBoxes(e, curAction.value.actionFrameInfos[curAction.currentFrame]);

            //移动输入接受
            CalculateInputAcceptance(curAction.currentFrame, curAction);

            // 动作产生的rootmotion
            // 算一下2帧之间的RootMotion变化
            ScriptMethodInfo rootMotion = curAction.value.rootMotionTween;
            var rootMotionComp          = e.hasRootMotion ? e.rootMotion : null;
            if (rootMotion && !string.IsNullOrEmpty(rootMotion.MethodName) && RootMotionMethod.Methods.ContainsKey(rootMotion.MethodName))
            {
                int lastFrame = curAction.currentFrame - 1;
                if (lastFrame < 0) lastFrame = 0;
                Vector3 rmThisTick = RootMotionMethod.Methods[rootMotion.MethodName](curAction.currentFrame, rootMotion.Params);
                Vector3 rmLastTick = RootMotionMethod.Methods[rootMotion.MethodName](lastFrame, rootMotion.Params);
                if (rootMotionComp != null)
                {
                    rootMotionComp.value = rmThisTick - rmLastTick;
                }
                //Debug.Log("RootMotion distance " + rootMotionComp.value + "=>" + curAction.currentFrame + " - " + lastFrame);
            }
            else
            {
                if (rootMotionComp != null)
                {
                    rootMotionComp.value = Vector3.zero;
                }
            }
        }
    }

    void CalculateInputAcceptance(int frame, CurrentActionComponent CurrentAction)
    {
        //float MoveInputAcceptance = 0;
        if (CurrentAction.value.currentFrame == null) return;
        /*foreach (MoveInputAcceptance acceptance in CurrentAction.value.MoveInputAcceptances)
        {
            if (acceptance.FrameRange.min <= frame && acceptance.FrameRange.max >= frame &&
                (MoveInputAcceptance <= 0 || acceptance.Rate < MoveInputAcceptance))
                MoveInputAcceptance = acceptance.Rate;
        }*/
        CurrentAction.MoveInputAcceptance = CurrentAction.value.currentFrame.MoveInputAcceptance;
    }

    void KeepAction(ActionInfo actionInfo)
    {
        // Debug.Log($"KeepAction:{actionInfo.name}");
    }

    void ChangeAction(GameEntity target, string actionName, int fromFrameIndex, float transitionNormalized, float fromNormalized)
    {
        ActionInfo foundAction      = null;
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
            
            var enterEvent = foundAction.enterActionEvent;
            if (enterEvent != null)
            {
                if (!string.IsNullOrEmpty(enterEvent.MethodName))
                {
                    if (ActionEventMethod.Methods.TryGetValue(enterEvent.MethodName, out var method))
                    {
                        method(target, fromFrameIndex, enterEvent.Params);
                    }
                }
            }

            if (target.hasAnimationController)
            {
                var animationController = target.animationController.value;
                if (animationController != null)
                {
                    animationController.PlayAnimation(foundAction.animation, transitionNormalized, 0, fromNormalized);
                }
            }
            curAction.value = foundAction;
            curAction.beCanceledTags.Clear();
            // 动作切换之后，开启的tempBeCancelTag，这是根据ActionChangeInfo的数据添加进来的
            if (target.hasTempBeCancelTag)
            {
                foreach (var tag in target.tempBeCancelTag.values)
                {
                    curAction.beCanceledTags.Add(BeCanceledTag.FromTemp(tag, 0));
                }
            }
            curAction.currentFrame = fromFrameIndex;
            if (target.hasHitRecord)
            {
                target.hitRecord.values.Clear();
            }
        }
    }

    void UpdateFrameHitBoxes(GameEntity e, ActionFrameInfo frameInfo)
    {
        var hitBoxComp = e.hitBox;
        hitBoxComp.values.Clear();
        if (frameInfo.HitBoxes != null)
        {
            hitBoxComp.values.AddRange(frameInfo.HitBoxes);
        }

        var attackHitBoxComp = e.attackHitBox;
        if (frameInfo.AttackHitBoxes != null)
        {
            attackHitBoxComp.values.AddRange(frameInfo.AttackHitBoxes);
        }
    }
}