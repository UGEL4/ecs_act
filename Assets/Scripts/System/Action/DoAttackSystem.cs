using System.Collections.Generic;
using Entitas;

public class DoAttackSystem : ReactiveSystem<GameEntity>
{
    private readonly GameContext gameContext;
    public DoAttackSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
    }

    // protected override void Execute(List<GameEntity> entities)
    // {
    //     foreach (var attacker in entities)
    //     {
    //         var hitResult = attacker.attackHitResult.value;
    //         var curAction = attacker.currentAction;
    //         //处理每个攻击框的命中结果
    //         for (int i = 0; i < hitResult.Count; i++)
    //         {
    //             var targetId = hitResult[i].targetEntityId;
    //             var target = gameContext.GetEntityWithId(targetId);
    //             if (target == null)
    //             {
    //                 continue;
    //             }

    //             var hitResultData = hitResult[i].result;
    //             List<int> bestAttackBoxIndexArray = new List<int>(hitResultData.hitMap.Count);
    //             Dictionary<int, int> bestHitBoxIndexMap = new();
    //             foreach (var kvp in hitResultData.hitMap)
    //             {
    //                 int attackBoxIndex = kvp.Key;
    //                 var hitBoxes = kvp.Value;

    //                 //命中的最有价值的受击框才行
    //                 int bestIndex = -1;
    //                 int bestPriority = -1;
    //                 bool foundBestHit = false;
    //                 for (int j = 0; j < hitBoxes.Count; j++)
    //                 {
    //                     int thisBestPriority = hitBoxes[j].priority;
    //                     if (bestIndex == -1 || thisBestPriority > bestPriority)
    //                     {
    //                         bestIndex = j;
    //                         bestPriority = thisBestPriority;
    //                         foundBestHit = true;
    //                     }
    //                 }
    //                 if (!foundBestHit) continue;

    //                 //
    //                 bestAttackBoxIndexArray.Add(attackBoxIndex);
    //                 bestHitBoxIndexMap.Add(attackBoxIndex, bestIndex);
    //             }

    //             //找最有价值的攻击框
    //             if (bestAttackBoxIndexArray.Count > 0)
    //             {
    //                 bestAttackBoxIndexArray.Sort((a, b) =>
    //                 {
    //                     var frameInfo = curAction.value.actionFrameInfos[curAction.currentFrame];
    //                     return (frameInfo.attackHitBoxes[a].priority > frameInfo.attackHitBoxes[b].priority) ? -1 : 1;
    //                 });

    //                 //最终最有价值的攻击框
    //                 int bestIndex = bestAttackBoxIndexArray[0];
    //                 var frameInfo = curAction.value.actionFrameInfos[curAction.currentFrame];
    //                 var finalAttackHitBox = frameInfo.attackHitBoxes[bestIndex];
    //                 //最有价值的攻击框碰撞到的最有价值的
    //                 var targetCurAction = target.currentAction;
    //                 var targetFrameInfo = targetCurAction.value.actionFrameInfos[targetCurAction.currentFrame];
    //                 var finalBeHitBox = targetFrameInfo.hitBoxes[bestHitBoxIndexMap[bestIndex]];
    //             }
    //         }

    //         hitResult.Clear();
    //         attacker.isAttackOrHitTag = false;
    //     }
    // }

    struct HitBoxCollection
    {
        public short id;
        public short hittargetResultDataIndex;
        public int totalPriority;
    };

    protected override void Execute(List<GameEntity> entities)
    {
        for (int i = 0; i < entities.Count; i++)
        {
            var attacker = entities[i];
            var hitResults = attacker.attackHitResult.value;
            var curAction = attacker.currentAction;
            var curFrameInfo = curAction.value.actionFrameInfos[curAction.currentFrame];
            var hitRecordComp = attacker.hitRecord;
            for (int j = 0; j < hitResults.Count; j++)
            {
                var hitTargetResult = hitResults[j];
                var targetId = hitTargetResult.targetEntityId;
                var target = gameContext.GetEntityWithId(targetId);
                if (target == null)
                {
                    continue;
                }
                var targetCurAction = target.currentAction;
                var targetCurFrameInfo = targetCurAction.value.actionFrameInfos[targetCurAction.currentFrame];

                //收集所有优先级最高的 攻击框-受击框 组合
                int maxPriority = int.MinValue;
                int bestIndex = -1;
                List<HitBoxCollection> collection = new();
                for (int k = 0; k < hitTargetResult.hitResults.Count; k++)
                {
                    var result = hitTargetResult.hitResults[k];
                    //int attackBoxIndex = result.attackBoxIndex;
                    // 获取攻击框优先级
                    int attackBoxPriority = curFrameInfo.attackHitBoxes[result.attackBoxIndex].priority;
                    //int hitBoxIndex = hitTargetResult.hitResults[k].hitBoxIndex;
                    // 获取受击框优先级
                    int priority = targetCurFrameInfo.hitBoxes[result.hitBoxIndex].priority;
                    // 计算总优先级
                    int totalPriority = attackBoxPriority + priority;
                    // 直接比较并记录最优结果
                    if (totalPriority > maxPriority)
                    {
                        maxPriority = totalPriority;
                        bestIndex = k;
                    }
                }

                if (bestIndex >= 0)
                {
                    // 找到最优的攻击框-受击框组合
                    var hitRecordInfo = hitRecordComp.GetHitRecordInfo(targetId);
                    if (hitRecordInfo == null || (hitRecordInfo.canHitTimes > 0 && hitRecordInfo.cooldown <= 0))
                    {
                        var bestResult = hitTargetResult.hitResults[bestIndex];
                        var attackBox = curFrameInfo.attackHitBoxes[bestResult.attackBoxIndex];
                        var hitBox = targetCurFrameInfo.hitBoxes[bestResult.hitBoxIndex];
                        //attacker
                        ActionChangeInfo attackerChangeInfo = attackBox.selfActionChangeInfo.priority > hitBox.attackerActionChangeInfo.priority ?
                            attackBox.selfActionChangeInfo : hitBox.attackerActionChangeInfo;
                        attacker.preorderAction.PreorderActionByActionChangeInfo(attackerChangeInfo, attacker.action.actions);

                        //target
                        ActionChangeInfo targetChangeInfo = attackBox.targetActionChangeInfo.priority > hitBox.selfActionChangeInfo.priority ?
                            attackBox.targetActionChangeInfo : hitBox.selfActionChangeInfo;
                        target.preorderAction.PreorderActionByActionChangeInfo(targetChangeInfo, target.action.actions);

                        foreach (var tag in attackerChangeInfo.tempBeCanceledTags)
                        {
                            attacker.tempBeCancelTag.AddTempBeCancelledTag(tag);
                        }
                        foreach (var tag in targetChangeInfo.tempBeCanceledTags)
                        {
                            target.tempBeCancelTag.AddTempBeCancelledTag(tag);
                        }

                        //产生伤害

                        //命中记录
                        hitRecordComp.AddHitRecord(attacker, target);
                    }
                }
            }

            attacker.isAttackOrHitTag = false;
            hitResults.Clear();
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isAttackOrHitTag && entity.hasAttackHitResult;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.AttackHitResult, GameMatcher.AttackOrHitTag));
    }


}