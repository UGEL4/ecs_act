using UnityEngine;
using ACTGame.KCC;
using UnityEngine.InputSystem;

namespace ACTGame
{
    public class CharacterController : MonoBehaviour, ICharacterController
    {
        private MonoCharacter owner;

        public float StableMovementSharpness = 15f;
        public float MaxStableMoveSpeed = 10f;
        public float OrientationSharpness = 10f;

        /// jump
        [Header("跳跃")]
        [Tooltip("重量，影响在空中时每帧的下落速度，配合跳跃速度可以控制跳跃")]
        public float Weight = 0.05f;
        public float MaxAirMoveSpeed = 10f;
        public float AirAccelerationSpeed = 5f;
        public float Drag = 0.1f;
        public bool AllowJumpingWhenSliding = true;
        [Tooltip("跳跃速度，起跳时的加速度")]
        public float JumpSpeed = 10f;
        public float JumpPreGroundingGraceTime = 0.1f;
        public float JumpPostGroundingGraceTime = 0.1f;

        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;

        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        /// jump

        public void SetOwner(MonoCharacter owner)
        {
            this.owner          = owner;
            var inputController = owner.GameEntity.inputController.instance as UnityInputController;
            inputController.AddButton4Callback(Jump);
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            var gameEntity = owner.GameEntity;
            var inputController = gameEntity.inputController.instance as UnityInputController;
            if (inputController.MoveInputVector != Vector3.zero && OrientationSharpness > 0f)
            {
                Vector3 dir = inputController.CharacterRelativeFlatten(inputController.MoveInputVector);
                var Motor = gameEntity.aCTGameKCCMotor.value;
                // Smoothly interpolate from current to target look direction
                Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, dir, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;
                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            var gameEntity = owner.GameEntity;
            if (gameEntity.hasForceMove && gameEntity.forceMove.force_move)
            {
                return;
            }

            var Motor = gameEntity.aCTGameKCCMotor.value;
            if (Motor.GroundingStatus.IsStableOnGround)
            {
                // rootMotion只是移动距离，需要根据移动方向来叠加
                Vector3 rootMotion = gameEntity.hasRootMotion ? gameEntity.rootMotion.value : Vector3.zero;
                // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
                currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                // Calculate target velocity
                Vector3 _moveInputVector = owner.GetMoveInputVector();
                Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                // Smooth movement Velocity
                rootMotion = Motor.Transform.TransformDirection(rootMotion) / deltaTime;
                currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime)) + rootMotion;
            }
            else
            {
                Vector3 _moveInputVector = owner.GetMoveInputVector();
                // rootMotion只是移动距离，需要根据移动方向来叠加
                Vector3 rootMotion = gameEntity.hasRootMotion ? gameEntity.rootMotion.value : Vector3.zero;
                var gravityComp    = gameEntity.hasGravity ? gameEntity.gravity : null;
                if (_moveInputVector.magnitude > 0f)
                {
                    var targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

                    // Prevent climbing on un-stable slopes with air movement
                    if (Motor.GroundingStatus.FoundAnyGround)
                    {
                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                        targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                    }

                    rootMotion = Motor.Transform.TransformDirection(rootMotion) / deltaTime;
                    currentVelocity += rootMotion;

                    Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                    currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                }
                // Gravity
                if (gravityComp != null)
                {
                    if (gravityComp.ApplyGravity)
                    {
                        if (gravityComp.IsGrounded)
                        {
                            gravityComp.IsGrounded = false;
                        }
                        // currentVelocity += Gravity * deltaTime;
                        currentVelocity.y -= gravityComp.CurrentWeight;
                        //Debug.Log($"gravityComp.CurrentWeight:{gravityComp.CurrentWeight} {currentVelocity}");

                        // Drag
                        currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                    }
                }
            }

            // Handle jumping
            _jumpedThisFrame = false;
            _timeSinceJumpRequested += deltaTime;
            if (_jumpRequested)
            {
                // See if we actually are allowed to jump
                if (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
                {
                    // Calculate jump direction before ungrounding
                    Vector3 jumpDirection = Motor.CharacterUp;
                    if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                    {
                        jumpDirection = Motor.GroundingStatus.GroundNormal;
                    }

                    // Makes the character skip ground probing/snapping on its next update.
                    // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                    Motor.ForceUnground(0.1f);

                    // Add to the return velocity and reset jump state
                    currentVelocity += (jumpDirection * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                    _jumpRequested = false;
                    _jumpConsumed = true;
                    _jumpedThisFrame = true;

                    if (gameEntity.hasGravity)
                    {
                        var gravityComp = gameEntity.gravity;
                        gravityComp.IsGrounded = false;
                        gravityComp.CurrentWeight = 0;
                        gravityComp.Ticked = 0;
                    }
                }
            }
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            // Handle jump-related values
            {
                // Handle jumping pre-ground grace period
                if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                {
                    _jumpRequested = false;
                }

                var gameEntity = owner.GameEntity;
                var Motor = gameEntity.aCTGameKCCMotor.value;
                // Handle jumping while sliding
                if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                {
                    // If we're on a ground surface, reset jumping values
                    if (!_jumpedThisFrame)
                    {
                        _jumpConsumed = false;

                        if (gameEntity.hasGravity && !gameEntity.gravity.IsGrounded)
                        {
                            var gravityComp = gameEntity.gravity;
                            gravityComp.IsGrounded = true;
                            gravityComp.CurrentWeight = 0;
                            gravityComp.Ticked = 0;
                            gameEntity.preorderAction.value.Add(new PreorderActionInfo("JumpLand"));
                        }
                    }
                    _timeSinceLastAbleToJump = 0f;
                }
                else
                {
                    // Keep track of time since we were last able to jump (for grace period)
                    _timeSinceLastAbleToJump += deltaTime;
                }
            }
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

        void Jump(InputActionPhase phase)
        {
            if (phase == InputActionPhase.Started)
            {
                _jumpRequested = true;
                _timeSinceJumpRequested = 0f;
            }
        }

        void OnDestroy()
        {
            owner    = null;
            MeshRoot = null;
        }

        public void OnDestroyView()
        {
            var inputController = owner.GameEntity.inputController.instance as UnityInputController;
            inputController.RemoveButton4Callback(Jump);
        }
    }
}