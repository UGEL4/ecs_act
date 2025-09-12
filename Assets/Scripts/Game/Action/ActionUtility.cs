using System.Collections.Generic;

namespace ACTGame.Action
{
    public static class ActionUtility
    {
        public static ActionInfo CreateActionInfo(ActionConfig actionConfig)
        {
            return actionConfig.GetActionInfo();
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
            foundCancelData       = new CancelData();
            cancelTag             = new CancelTag();
            var currentActionInfo = curActionComp.value;
            
            curActionComp.allCancelTag.Clear();
            //当前动作本帧所有的cancelTag
            var currentFrameInfo = currentActionInfo.currentFrame;
            for (int j = 0; j < currentFrameInfo.CancelTags.Length; j++)
            {
                curActionComp.allCancelTag.Add(currentFrameInfo.CancelTags[j]);
            }
            //当前动作临时开启的cancelTag
            for (int j = 0; j < curActionComp.beCanceledTags.Count; j++)
            {
                BeCanceledTag bcTagInfo = curActionComp.beCanceledTags[j];
                foreach (string bcTagName in bcTagInfo.CancelTag)
                {
                    if (!(curActionComp.currentFrame <= bcTagInfo.ValidRange.max && curActionComp.currentFrame >= bcTagInfo.ValidRange.min)) continue;
                    curActionComp.allCancelTag.Add(new CancelTag {
                        Tag               = bcTagName,
                        Priority          = bcTagInfo.Priority,
                        IsNowActive       = true
                    });
                }
            }

            if (actionInfo.cancelDatas == null)
            {
                return false;
            }

            for (int i = 0; i < actionInfo.cancelDatas.Length; i++)
            {
                bool tagFit           = false;
                CancelData cancelData = actionInfo.cancelDatas[i];
                foreach (var tagName in cancelData.Tags)
                {
                    // 本帧的所有cancelTag和临时开启的cancelTag，能否被cancelData所cancel
                    for (int j = 0; j < curActionComp.allCancelTag.Count; j++)
                    {
                        if (tagName == curActionComp.allCancelTag[j].Tag)
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