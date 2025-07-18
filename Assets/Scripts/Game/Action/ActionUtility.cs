using System.Collections.Generic;

namespace ACTGame.Action
{
    public static class ActionUtility
    {
        public static ActionInfo CreateActionInfo(EditActionInfo actionConfig)
        {
            ActionInfo actionInfo = new ActionInfo();

            actionInfo.name               = actionConfig.ActionName;
            actionInfo.animation          = actionConfig.Animation;
            actionInfo.actionFrameInfos   = actionConfig.InitFrameList();
            actionInfo.commandList        = actionConfig.commandList;
            actionInfo.priority           = actionConfig.priority;
            actionInfo.autoNextActionName = actionConfig.autoNextActionName;
            actionInfo.autoTerminate      = actionConfig.autoTerminate;
            actionInfo.keepPlayingAnim    = actionConfig.keepPlayingAnim;
            actionInfo.cancelDatas        = actionConfig.cancelDatas;
            actionInfo.hitInfo            = actionConfig.hitInfo;
            actionInfo.rootMotionTween    = actionConfig.rootMotionTween;
            actionInfo.enterActionEvent   = actionConfig.enterActionEvent;
            actionInfo.ApplyGravity       = actionConfig.ApplyGravity;
            if (actionInfo.actionFrameInfos.Count > 0)
            {
                actionInfo.currentFrame = actionInfo.actionFrameInfos[0];
            }

            return actionInfo;
        }

        /*public static bool CanActionCancelCurrentAction(
            ActionInfo actionInfo, 
            bool checkCommand, 
            InputToCommandComponent input,
            long curFrame,
            long entityGlobalFrame,
            List<BeCanceledTag> beCanceledTagList,
            out CancelTag foundTag,
            out BeCanceledTag beCabceledTag
        )
        {
            foundTag      = new CancelTag();
            beCabceledTag = new BeCanceledTag();
            foreach (BeCanceledTag bcTagInfo in beCanceledTagList)
            {
                bool tagFit = false;
                foreach (string bcTagName in bcTagInfo.cancelTag)
                {
                    // if (!(_wasPercentage <= bcTagInfo.range.max && curPercent >= bcTagInfo.range.min)) continue;
                    // Log.SimpleLog.Info("CanActionCancelCurrentAction:", mLastFrameIndex, mCurrentFrameIndex);
                    if (!(curFrame <= bcTagInfo.validRange.max && curFrame >= bcTagInfo.validRange.min)) continue;
                    foreach (CancelTag cTag in actionInfo.cancelTags)
                    {
                        if (bcTagName == cTag.tag)
                        {
                            // if (actionInfo.SelfLoopCount > 0 && ActionLoopCount > actionInfo.SelfLoopCount)
                            // {
                            //     continue;
                            // }
                            tagFit        = true;
                            foundTag      = cTag;
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
                        if (input.ActionOccur(cmd, entityGlobalFrame)) return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }*/

        public static bool CanActionCancelCurrentAction(
            ActionInfo actionInfo, 
            CurrentActionComponent curActionComp,
            bool checkCommand, 
            InputToCommandComponent input,
            long entityFrame,
            out CancelData foundCancelData,
            out CancelTag cancelTag
        )
        {
            foundCancelData       = null;
            cancelTag             = new CancelTag();
            var currentActionInfo = curActionComp.value;
            
            curActionComp.allCancelTag.Clear();
            //当前动作本帧所有的cancelTag
            var currentFrameInfo = currentActionInfo.currentFrame;
            for (int j = 0; j < currentFrameInfo.cancelTags.Count; j++)
            {
                curActionComp.allCancelTag.Add(currentFrameInfo.cancelTags[j]);
            }
            //当前动作临时开启的cancelTag
            for (int j = 0; j < curActionComp.beCanceledTags.Count; j++)
            {
                BeCanceledTag bcTagInfo = curActionComp.beCanceledTags[j];
                foreach (string bcTagName in bcTagInfo.cancelTag)
                {
                    if (!(curActionComp.currentFrame <= bcTagInfo.validRange.max && curActionComp.currentFrame >= bcTagInfo.validRange.min)) continue;
                    curActionComp.allCancelTag.Add(new CancelTag(bcTagName, bcTagInfo.priority, true));
                }
            }

            for (int i = 0; i < actionInfo.cancelDatas.Count; i++)
            {
                bool tagFit           = false;
                CancelData cancelData = actionInfo.cancelDatas[i];
                foreach (var tagName in cancelData.tags)
                {
                    // 本帧的所有cancelTag和临时开启的cancelTag，能否被cancelData所cancel
                    for (int j = 0; j < curActionComp.allCancelTag.Count; j++)
                    {
                        if (tagName == curActionComp.allCancelTag[j].tag)
                        {
                            tagFit          = true;
                            foundCancelData = cancelData;
                            cancelTag       = curActionComp.allCancelTag[j];
                            break;
                        }
                    }
                    if (tagFit)
                    {
                        break;
                    }
                }
                if (!tagFit)
                {
                    continue;
                }

                if (checkCommand)
                {
                    for (int j = 0; j < actionInfo.commandList.Count; j++)
                    {
                        if (input.ActionOccur(actionInfo.commandList[j], entityFrame))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}