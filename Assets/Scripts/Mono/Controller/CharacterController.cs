using UnityEngine;
using ACTGame.KCC;

namespace ACTGame
{
    public class CharacterController : MonoBehaviour, ICharacterController
    {
        private MonoCharacter owner;

        public float StableMovementSharpness = 15f;
        public float MaxStableMoveSpeed      = 10f;
        public float OrientationSharpness    = 10f;

        /// jump
        [Header("跳跃")]
        public float Weight                 = 0.05f;
        public float MaxAirMoveSpeed        = 10f;
        public float AirAccelerationSpeed   = 5f;
        public float Drag                   = 0.1f;
        public bool AllowJumpingWhenSliding = true;
        public float MaxJumpTime            = 1f;
        public float JumpHeight             = 2f;
        /// jump

        public void SetOwner(MonoCharacter owner)
        {
            this.owner = owner;
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            var gameEntity      = owner.GameEntity;
            var inputController = gameEntity.inputController.instance as UnityInputController;
            if (inputController.MoveInputVector != Vector3.zero && OrientationSharpness > 0f)
            {
                Vector3 dir = inputController.CharacterRelativeFlatten(inputController.MoveInputVector);
                var Motor   = gameEntity.aCTGameKCCMotor.value;
                // Smoothly interpolate from current to target look direction
                Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, dir, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;
                currentRotation                    = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            var gameEntity = owner.GameEntity;
            if (gameEntity.hasForceMove && gameEntity.forceMove.force_move)
            {
                return;
            }

            var MoveInputAcceptances = gameEntity.currentAction.MoveInputAcceptance;
            var Motor                = gameEntity.aCTGameKCCMotor.value;

            //var velocityComp = gameEntity.velocity;
            var gravityComp = gameEntity.gravity;

            //if (Motor.GroundingStatus.IsStableOnGround)
            if ((gravityComp.IsGrounded && Motor.GroundingStatus.IsStableOnGround)
             || (gravityComp.IsGrounded && !Motor.LastGroundingStatus.IsStableOnGround))
            {
                // rootMotion只是移动距离，需要根据移动方向来叠加
                Vector3 rootMotion = gameEntity.hasRootMotion ? gameEntity.rootMotion.value : Vector3.zero;
                if (rootMotion != Vector3.zero)
                {
                    // The final velocity is the velocity from root motion reoriented on the ground plane
                    rootMotion      = Motor.Transform.TransformDirection(rootMotion) / deltaTime;
                    currentVelocity = rootMotion;
                }
                
                // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
                currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                // Calculate target velocity
                Vector3 _moveInputVector       = owner.GetMoveInputVector();
                Vector3 inputRight             = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                Vector3 reorientedInput        = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed * MoveInputAcceptances;

                // Smooth movement Velocity
                currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
            }
            else
            {
                // rootMotion只是移动距离，需要根据移动方向来叠加
                Vector3 rootMotion = gameEntity.hasRootMotion ? gameEntity.rootMotion.value : Vector3.zero;
                if (rootMotion != Vector3.zero)
                {
                    rootMotion         = Motor.Transform.TransformDirection(rootMotion) / deltaTime;

                }
                else
                {
                    //currentVelocity = velocityComp.value;
                    Vector3 _moveInputVector = owner.GetMoveInputVector();
                    if (_moveInputVector.magnitude > 0f)
                    {
                        var targetMovementVelocity = _moveInputVector * MoveInputAcceptances * MaxAirMoveSpeed;

                        // Prevent climbing on un-stable slopes with air movement
                        if (Motor.GroundingStatus.FoundAnyGround)
                        {
                            Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                            targetMovementVelocity                 = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                        }

                        Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, -Vector3.up);
                        currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                    }
                }
                // Drag
                currentVelocity *= (1f / (1f + (Drag * deltaTime)));
            }
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            var gameEntity = owner.GameEntity;
            var Motor      = gameEntity.aCTGameKCCMotor.value;
            // 获取当前和上一帧的稳定状态
            bool isStable = Motor.GroundingStatus.IsStableOnGround;
            bool wasStable = Motor.LastGroundingStatus.IsStableOnGround;

            if (wasStable && !isStable)
            {
                if (IsStepping(Motor))
                {
                    gameEntity.gravity.IsGrounded = false;
                }
                else if (IsWalkingOffLedge(Motor))
                {
                    gameEntity.gravity.IsGrounded = false;
                    if (!gameEntity.jump.IsJumping)
                    {
                        gameEntity.preorderAction.value.Add(new PreorderActionInfo("JumpLoop", 3));
                    }
                }
                else
                {
                    if (!gameEntity.jump.IsJumping)
                    {
                        gameEntity.gravity.IsGrounded = false;
                        gameEntity.preorderAction.value.Add(new PreorderActionInfo("JumpLoop", 3));
                    }
                }
            }

            if (isStable && !wasStable)
            {
                if (gameEntity.hasGravity && !gameEntity.gravity.IsGrounded)
                {
                    var gravityComp        = gameEntity.gravity;
                    gravityComp.IsGrounded = true;
                    gravityComp.Gravity    = gravityComp.DefaultGravity;
                    gameEntity.preorderAction.value.Add(new PreorderActionInfo("JumpLand"));
                }
            }

            if (gameEntity.jump.IsJumping)
            {
                gameEntity.jump.IsJumping = false;
            }
        }

        bool IsStepping(KinematicCharacterMotor motor)
        {
            return motor.GroundingStatus.ValidStepDetected;
        }

        bool IsWalkingOffLedge(KinematicCharacterMotor motor)
        {
            return motor.GroundingStatus.LedgeDetected && motor.GroundingStatus.IsMovingTowardsEmptySideOfLedge;
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void PostGroundingUpdate(float deltaTime)
        {
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        void OnDestroy()
        {
            owner = null;
        }

        public void OnDestroyView()
        {
        }
    }
}