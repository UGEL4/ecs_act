using System.Collections.Generic;
using ACTGame;
using Entitas;
using Unity.Mathematics;

public class HitResultCollectSystem : IExecuteSystem
{
    readonly Contexts contexts;
    readonly IGroup<GameEntity> entities;

    public HitResultCollectSystem(Contexts contexts)
    {
        this.contexts = contexts;
        entities = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.HitBox, GameMatcher.AttackHitBox));
    }

    public void Execute()
    {
        GameEntity[] _entities = entities.GetEntities();
        for (int i = 0; i < _entities.Length; i++)
        {
            var entityA = _entities[i];
            for (int j = i + 1; j < _entities.Length; j++)
            {
                var entityB = _entities[j];
                CheckCollision(entityA, entityB);
                CheckCollision(entityB, entityA);
            }
        }
    }

    void CheckCollision(GameEntity entityA, GameEntity entityB)
    {
        var worldTransformA = entityA.worldTransform;
        var attackBoxA = entityA.attackHitBox.values;
        var resultComp = entityA.attackHitResult;

        var worldTransformB = entityB.worldTransform;
        var hitBoxB = entityB.hitBox.values;
        long targetIndex = entityB.id.value;

        AttackHitTargetResultData hitTargetResultData = null;
        bool hasTargetData = false;
        for (int i = 0; i < resultComp.value.Count; i++)
        {
            if (resultComp.value[i].targetEntityId == targetIndex)
            {
                hitTargetResultData = resultComp.value[i];
                hasTargetData = true;
                break;
            }
        }
        if (hitTargetResultData == null)
        {
            hitTargetResultData = new AttackHitTargetResultData();
            hitTargetResultData.targetEntityId = targetIndex;
        }

        for (int i = 0; i < attackBoxA.Count; i++)
        {
            var attackHitResultData = hitTargetResultData.hitResults;

            OrientedBox worldAttackBox = OrientedBox.GetWorldSpaceOBB(attackBoxA[i].box, worldTransformA.value);
            for (int j = 0; j < hitBoxB.Count; j++)
            {
                OrientedBox worldHitBox = OrientedBox.GetWorldSpaceOBB(hitBoxB[j].box, worldTransformB.value);
                if (OrientedBox.CheckOverlap(worldAttackBox, worldHitBox))
                {
                    attackHitResultData.Add(new AttackHitTargetResultData.HitResult()
                    {
                        attackBoxIndex = (short)i,
                        hitBoxIndex = (short)j
                    });
                    if (!hasTargetData)
                    {
                        hasTargetData = true;
                        resultComp.value.Add(hitTargetResultData);
                    }

                    if (!entityA.isAttackOrHitTag)
                    {
                        entityA.isAttackOrHitTag = true;
                    }
                    if (!entityB.isAttackOrHitTag)
                    {
                        entityB.isAttackOrHitTag = true;
                    }
                }
            }
        }
    }

    // void CheckCollision(GameEntity entityA, GameEntity entityB)
    // {
    //     var worldTransformA = entityA.worldTransform;
    //     var attackBoxA = entityA.attackHitBox.values;
    //     var resultComp = entityA.attackHitResult;

    //     var worldTransformB = entityB.worldTransform;
    //     var hitBoxB = entityB.hitBox.values;
    //     long targetIndex = entityB.id.value;

    //     for (int i = 0; i < attackBoxA.Count; i++)
    //     {
    //         OrientedBox worldAttackBox = OrientedBox.GetWorldSpaceOBB(attackBoxA[i].box, worldTransformA.value);
    //         for (int j = 0; j < hitBoxB.Count; j++)
    //         {
    //             //float3[] hitBoxCorners = GetBoxCorners(hitBox[i].box.position, hitBox[i].box.center, hitBox[i].box.size, hitBox[i].box.rotation);
    //             OrientedBox worldHitBox = OrientedBox.GetWorldSpaceOBB(hitBoxB[j].box, worldTransformB.value);
    //             if (OrientedBox.CheckOverlap(worldAttackBox, worldHitBox))
    //             {
    //                 if (!resultComp.value.TryGetValue(i, out AttackHitResultData resultData))
    //                 {
    //                     resultData = new AttackHitResultData
    //                     {
    //                         bestHitBoxPerTarget = new Dictionary<long, HitBox>(),
    //                         allHitBoxes = new List<HitBox>()
    //                     };
    //                     resultComp.value[i] = resultData;
    //                 }

    //                 // 添加到所有碰撞列表
    //                 resultData.allHitBoxes.Add(hitBoxB[j]);

    //                 // 更新最佳受击框
    //                 long targetId = entityB.id.value;
    //                 if (!resultData.bestHitBoxPerTarget.TryGetValue(targetId, out HitBox currentBest) ||
    //                     hitBoxB[j].priority > currentBest.priority)
    //                 {
    //                     resultData.bestHitBoxPerTarget[targetId] = hitBoxB[j];
    //                 }

    //                 if (!entityA.isAttackOrHitTag)
    //                 {
    //                     entityA.isAttackOrHitTag = true;
    //                 }
    //                 if (!entityB.isAttackOrHitTag)
    //                 {
    //                     entityB.isAttackOrHitTag = false;
    //                 }
    //             }
    //         }
    //     }
    // }

    float3[] GetBoxCorners(float3 pos, float3 center, float3 size, float3 rotation)
    {
        float3 halfSizeAttackBox = size * 0.5f;
        quaternion rot = quaternion.Euler(rotation);
        float3[] corners = new float3[8];
        corners[0] = pos + math.mul(rot, center + new float3(halfSizeAttackBox.x, halfSizeAttackBox.y, halfSizeAttackBox.z));
        corners[1] = pos + math.mul(rot, center + new float3(halfSizeAttackBox.x, halfSizeAttackBox.y, -halfSizeAttackBox.z));
        corners[2] = pos + math.mul(rot, center + new float3(halfSizeAttackBox.x, -halfSizeAttackBox.y, halfSizeAttackBox.z));
        corners[3] = pos + math.mul(rot, center + new float3(halfSizeAttackBox.x, -halfSizeAttackBox.y, -halfSizeAttackBox.z));
        corners[4] = pos + math.mul(rot, center + new float3(-halfSizeAttackBox.x, halfSizeAttackBox.y, halfSizeAttackBox.z));
        corners[5] = pos + math.mul(rot, center + new float3(-halfSizeAttackBox.x, halfSizeAttackBox.y, -halfSizeAttackBox.z));
        corners[6] = pos + math.mul(rot, center + new float3(-halfSizeAttackBox.x, -halfSizeAttackBox.y, halfSizeAttackBox.z));
        corners[7] = pos + math.mul(rot, center + new float3(-halfSizeAttackBox.x, -halfSizeAttackBox.y, -halfSizeAttackBox.z));
        return corners;
    }
}