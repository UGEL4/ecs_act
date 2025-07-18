using Entitas;

public class KCCSystem : IExecuteSystem
{
    private readonly Contexts contexts;
    private readonly IGroup<GameEntity> motorEntities;
    private readonly IGroup<GameEntity> physicalMoverEntities;
    public KCCSystem(Contexts contexts)
    {
        this.contexts         = contexts;
        motorEntities         = contexts.game.GetGroup(GameMatcher.AllOf(
        GameMatcher.ACTGameKCCMotor,
        GameMatcher.Timer));
        physicalMoverEntities = contexts.game.GetGroup(GameMatcher.AllOf(
        GameMatcher.Timer,
        GameMatcher.ACTGameKCCPhysicalMover));
    }

    public void Execute()
    {
        var motors      = motorEntities.GetEntities();
        var movers      = physicalMoverEntities.GetEntities();
        var timer       = contexts.game.timerManager.GetSceneTimer(contexts.game);
        float deltaTime = 1.0f / timer.hertz;
        PreSimulationInterpolationUpdate(deltaTime, motors, movers);
        Simulate(deltaTime, motors, movers);
        PostSimulationInterpolationUpdate(deltaTime, motors, movers);
    }

    /// <summary>
    /// Remembers the point to interpolate from for KinematicCharacterMotors and PhysicsMovers
    /// </summary>
    void PreSimulationInterpolationUpdate(float deltaTime, GameEntity[] motorEntities, GameEntity[] moverEntities)
    {
        // Save pre-simulation poses and place transform at transient pose
        for (int i = 0; i < motorEntities.Length; i++)
        {
            var motor = motorEntities[i].aCTGameKCCMotor.value;

            motor.InitialTickPosition = motor.TransientPosition;
            motor.InitialTickRotation = motor.TransientRotation;

            motor.Transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
        }

        for (int i = 0; i < moverEntities.Length; i++)
        {
            if (!moverEntities[i].hasACTGameKCCPhysicalMover) continue;
            var mover = moverEntities[i].aCTGameKCCPhysicalMover.value;

            mover.InitialTickPosition = mover.TransientPosition;
            mover.InitialTickRotation = mover.TransientRotation;

            mover.Transform.SetPositionAndRotation(mover.TransientPosition, mover.TransientRotation);
            mover.Rigidbody.position = mover.TransientPosition;
            mover.Rigidbody.rotation = mover.TransientRotation;
        }
    }

    /// <summary>
    /// Ticks characters and/or movers
    /// </summary>
    void Simulate(float deltaTime, GameEntity[] motorEntities, GameEntity[] moverEntities)
    {
#pragma warning disable 0162
        // Update PhysicsMover velocities
        for (int i = 0; i < moverEntities.Length; i++)
        {
            if (!moverEntities[i].hasACTGameKCCPhysicalMover) continue;
            var mover = moverEntities[i].aCTGameKCCPhysicalMover.value;
            mover.VelocityUpdate(deltaTime);
        }

        // Character controller update phase 1
        for (int i = 0; i < motorEntities.Length; i++)
        {
            var motor = motorEntities[i].aCTGameKCCMotor.value;
            //motor.BaseVelocity = motorEntities[i].velocity.value;
            motor.UpdatePhase1(deltaTime);
        }

        // Simulate PhysicsMover displacement
        for (int i = 0; i < moverEntities.Length; i++)
        {
            if (!moverEntities[i].hasACTGameKCCPhysicalMover) continue;
            var mover = moverEntities[i].aCTGameKCCPhysicalMover.value;

            mover.Transform.SetPositionAndRotation(mover.TransientPosition, mover.TransientRotation);
            mover.Rigidbody.position = mover.TransientPosition;
            mover.Rigidbody.rotation = mover.TransientRotation;
        }

        // Character controller update phase 2 and move
        for (int i = 0; i < motorEntities.Length; i++)
        {
            var motor = motorEntities[i].aCTGameKCCMotor.value;

            motor.UpdatePhase2(deltaTime);

            motor.Transform.SetPositionAndRotation(motor.TransientPosition, motor.TransientRotation);
            //motorEntities[i].velocity.value = motor.BaseVelocity;
        }
#pragma warning restore 0162
    }

    /// <summary>
    /// Initiates the interpolation for KinematicCharacterMotors and PhysicsMovers
    /// </summary>
    void PostSimulationInterpolationUpdate(float deltaTime, GameEntity[] motorEntities, GameEntity[] moverEntities)
    {
        //_lastCustomInterpolationStartTime = Time.time;
        //_lastCustomInterpolationDeltaTime = deltaTime;

        // Return interpolated roots to their initial poses
        for (int i = 0; i < motorEntities.Length; i++)
        {
            var motor = motorEntities[i].aCTGameKCCMotor.value;

            motor.Transform.SetPositionAndRotation(motor.InitialTickPosition, motor.InitialTickRotation);
            motorEntities[i].position.value       = motor.Transform.localPosition;
            motorEntities[i].rotation.value       = motor.Transform.localRotation;
            motorEntities[i].scale.value          = motor.Transform.localScale;
            motorEntities[i].worldTransform.value = motor.Transform.localToWorldMatrix;
        }

        for (int i = 0; i < moverEntities.Length; i++)
        {
            if (!moverEntities[i].hasACTGameKCCPhysicalMover) continue;
            var mover = moverEntities[i].aCTGameKCCPhysicalMover.value;

            if (mover.MoveWithPhysics)
            {
                mover.Rigidbody.position = mover.InitialTickPosition;
                mover.Rigidbody.rotation = mover.InitialTickRotation;

                mover.Rigidbody.MovePosition(mover.TransientPosition);
                mover.Rigidbody.MoveRotation(mover.TransientRotation);
            }
            else
            {
                mover.Rigidbody.position = mover.TransientPosition;
                mover.Rigidbody.rotation = mover.TransientRotation;
            }
        }
    }
}