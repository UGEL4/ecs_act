using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class ActionEventMethod
{
    public static Dictionary<string, Func<GameEntity, int, string[], bool>> Methods = new Dictionary<string, Func<GameEntity, int, string[], bool>> {
        { "EnterJump",
          (entity, frame, param) =>
          {
              var action       = entity.currentAction;
              var motor        = entity.aCTGameKCCMotor.value;

              // Calculate jump direction before ungrounding
              Vector3 jumpDirection = motor.CharacterUp;
              if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
              {
                  jumpDirection = motor.GroundingStatus.GroundNormal;
              }

              // Makes the character skip ground probing/snapping on its next update.
              // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
              motor.ForceUnground(0.1f);

              float timeToJumpApex = param.Length > 0 ? float.Parse(param[0]) : 1f;
              float jumpHeight     = param.Length > 1 ? float.Parse(param[1]) : 1f;
              float time           = timeToJumpApex * 0.5f;
              float jumpSpeed      = 2 * jumpHeight / time;
              var gravityComp      = entity.gravity;
              gravityComp.Gravity  = 2 * jumpHeight / Mathf.Pow(time, 2);

              // Add to the return velocity and reset jump state
              motor.BaseVelocity += (jumpDirection * jumpSpeed) - Vector3.Project(motor.BaseVelocity, motor.CharacterUp);
              gravityComp.IsGrounded = false;

              if (entity.hasJump)
              {
                  entity.jump.IsJumping = true;
              }

              return true;
          } }
    };
}