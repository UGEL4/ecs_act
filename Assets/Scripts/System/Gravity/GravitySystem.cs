using Entitas;
using Unity.VisualScripting;
using UnityEngine;

public class GravitySystem : IExecuteSystem
{
    private readonly Contexts contexts;
    private readonly IGroup<GameEntity> entities;
    public GravitySystem(Contexts contexts)
    {
        this.contexts = contexts;
        entities      = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Gravity, GameMatcher.ACTGameComponentTimer));
    }

    public void Execute()
    {
        var entites = this.entities.GetEntities();
        for (int i = 0; i < entites.Length; i++)
        {
            var e       = entites[i];
            var gravity = e.gravity;
            var action  = e.hasCurrentAction ? e.currentAction : null;
            if (action != null)
            {
                gravity.ApplyGravity = action.value.ApplyGravity;
            }

            if (gravity.IsGrounded || !gravity.ApplyGravity)
            {
                continue;
            }

            var motor = e.aCTGameKCCMotor.value;
            motor.BaseVelocity.y += -gravity.Gravity * 1f / e.aCTGameComponentTimer.hertz;
        }
    }
}