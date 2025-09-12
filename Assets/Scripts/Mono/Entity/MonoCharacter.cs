using System.Collections.Generic;
using ACTGame.Action;
using Entitas;
using Entitas.Unity;
using UnityEngine;

public class MonoCharacter : MonoBehaviour, IViewController
{
    private GameEntity gameEntity;
    public GameEntity GameEntity => gameEntity;
    private Contexts contexts;
    public Contexts Contexts => contexts;

    private CameraController01 cameraController;

    [SerializeField]
    private ACTGame.CharacterController characterController;

    [SerializeField]
    private GameObject cameraTarget;

    private AnimationController animationController;
    public AnimationController AnimationController => animationController;

    public Vector3 Position
    {
        get {
            return transform.position;
        }
        set {
            transform.position = value;
        }
    }

    public Vector3 Rotation
    {
        get {
            return transform.rotation.eulerAngles;
        }
        set {
            transform.rotation = Quaternion.Euler(value);
        }
    }

    public Vector3 Scale
    {
        get {
            return transform.localScale;
        }
        set {
            transform.localScale = value;
        }
    }

    public bool Active
    {
        get {
            return gameObject.activeSelf;
        }
        set {
            gameObject.SetActive(value);
        }
    }

    public void DestroyView()
    {
        Destroy(gameObject);
    }

    public IEntity GetEntity()
    {
        return gameEntity;
    }

    public void InitializeView(Contexts contexts, IEntity Entity)
    {
        this.contexts   = contexts;
        this.gameEntity = (GameEntity)Entity;
        //gameObject.Link(Entity);
        OnCreate();

        var cameraRes    = Resources.Load<GameObject>("Prefabs/Camera/Camera01");
        var cameraObj    = Instantiate(cameraRes);
        cameraController = cameraObj.GetComponent<CameraController01>();
        cameraController.SetCameraFollowTarget(cameraTarget.transform);

        var animator        = GetComponentInChildren<Animator>(true);
        animationController = new AnimationController(this, animator);
        gameEntity.AddAnimationController(animationController);
    }

    public void Link(IEntity entity)
    {
        gameObject.Link(entity);
    }

    public void Unlink()
    {
        gameObject.Unlink();
    }

    void OnCreate()
    {
        long levelId    = this.gameEntity.entity.levelId;
        var levelEntity = contexts.game.GetEntityWithId(levelId);
        if (levelEntity == null)
        {
            Debug.LogError("Could not find levelEntity with id:" + levelId);
            return;
        }
        var worldObj = MonoGameObjectInstance.GetWorld(levelEntity.level.worldId);
        if (worldObj == null)
        {
            Debug.LogError("Could not find world with id:" + levelEntity.level.worldId);
            return;
        }

        var controller = new UnityInputController();
        controller.Initialize(contexts);
        controller.SetOwner(this);
        gameEntity.AddInputController(controller);

        var motor = gameObject.GetComponent<ACTGame.KCC.KinematicCharacterMotor>();
        gameEntity.AddACTGameKCCMotor(motor);
        characterController.SetOwner(this);
        motor.CharacterController = characterController;

        gameEntity.AddRootMotion(Vector3.zero);

        var position = gameEntity.position.value;
        motor.SetPosition(position);
        var unityWorld = worldObj.GetComponent<MonoWorld>();
        if (unityWorld != null)
        {
            var levelRootTransform = unityWorld.GetLevelRoot();
            motor.Transform.SetParent(levelRootTransform);
        }
        else
        {
            motor.Transform.SetParent(unityWorld.transform);
        }
        motor.SetPosition(position);

        long entityId = gameEntity.id.value;
        gameEntity.ReplaceInputToCommand(entityId, new List<KeyRecord>());

        // updateLogic = new MainRoleUpdater(contexts, gameEntity);
        var sceneTimer = contexts.game.timerManager.GetSceneTimer(contexts.game);
        if (sceneTimer != null)
        {
            var index             = GameComponentsLookup.Timer;
            var component         = (TimerComponent)gameEntity.CreateComponent(index, typeof(TimerComponent));
            component.curFrame    = sceneTimer.GetNow();
            component.hertz       = 60;
            component.accumulator = 0;
            component.isUnitTimer = true;
            // component.NewFrameTimer(InvokeType.EntityUpdate, updateLogic);
            gameEntity.ReplaceComponent(index, component);
            contexts.game.timerManager.OnAddUnitTimer(entityId);
        }

        float jumpTime   = characterController.MaxJumpTime;
        float jumpHeight = characterController.JumpHeight;
        float time       = jumpTime * 0.5f;
        float gravity    = 2 * jumpHeight / Mathf.Pow(time, 2);
        gameEntity.AddGravity(gravity, gravity, true, true);
        gameEntity.ReplaceJump(false);

        AddTestActions();
    }

    void AddTestActions()
    {
        var CharacterCfg = Resources.Load<ACTGame.View.MonoCharacterConfig>("Prefabs/ScriptableObjects/MonoCharacterConfig/ch_02");
        List<ActionInfo> actionList = new();
        foreach (var path in CharacterCfg.ActionList)
        {
            var ActionCfg = Resources.Load<ActionConfig>(path);
            if (ActionCfg)
            {
                ActionInfo action = ActionUtility.CreateActionInfo(ActionCfg);
                actionList.Add(action);
            }
        }
        //var Idle = Resources.Load<EditActionInfo>("Prefabs/ScriptableObjects/ActionInfo/Idle");
        /*var Idle = Resources.Load<ActionConfig>("Prefabs/ScriptableObjects/ActionConfig/ch_02/Idle");
        if (Idle)
        {
            ActionInfo action = ActionUtility.CreateActionInfo(Idle);
            actionList.Add(action);
        }*/
        // ActionInfo action           = new ActionInfo();
        // action.name                 = "Idle";
        // action.animation            = "pl0000_00000";
        // for (int i = 0; i < 31; i++)
        // {
        //     action.actionFrameInfos.Add(new ActionFrameInfo() { frameId = i });
        // }
        // action.commandList.Add(new ActionCommand() { keySequences = new KeyMap[0], validInFrameCount = 1 });
        // action.priority           = 0;
        // action.keepPlayingAnim    = true;
        // action.autoNextActionName = "Idle";
        // // canceltag
        // action.beCanceledTags = new BeCanceledTag[] {
        //     new BeCanceledTag() { cancelTag = new string[] { "IdleAction" }, priority = 0, validRange = new FrameIndexRange(0, 30) }
        // };
        // action.ApplyGravity = true;
        // actionList.Add(action);

        /*var Run = Resources.Load<ActionConfig>("Prefabs/ScriptableObjects/ActionConfig/ch_02/Run");
        if (Run)
        {
            ActionInfo action = ActionUtility.CreateActionInfo(Run);
            actionList.Add(action);
        }*/
        // action           = new ActionInfo();
        // action.name      = "Walk";
        // action.animation = "pl0000_00002";
        // for (int i = 0; i < 67; i++)
        // {
        //     action.actionFrameInfos.Add(new ActionFrameInfo() { frameId = i });
        // }
        // action.commandList.Add(new ActionCommand() { keySequences = new[] { KeyMap.DirInput }, validInFrameCount = 1 });
        // action.priority           = 1;
        // action.keepPlayingAnim    = true;
        // action.autoNextActionName = "Idle";
        // action.autoTerminate      = true;
        // action.cancelTags         = new CancelTag[] {
        //     new CancelTag() { tag = "IdleAction", priority = 0, startFromFrameIndex = 0 },
        //     new CancelTag() { tag = "RunAction", priority = 0, startFromFrameIndex = 0 },
        //     new CancelTag() { tag = "JumpLand", priority = 2, startFromFrameIndex = 0 },
        // };
        // action.beCanceledTags = new BeCanceledTag[] {
        //     new BeCanceledTag() { cancelTag = new string[] { "RunAction" }, priority = 1, validRange = new FrameIndexRange(0, 66) }
        // };
        // var rootMotionMethod        = Resources.Load<ScriptMethodInfo>("Prefabs/ScriptableObjects/ScriptMethodInfo/RootMotion/GoStraight");
        // action.rootMotionTween      = rootMotionMethod;
        // action.ApplyGravity         = true;
        // action.MoveInputAcceptances = new MoveInputAcceptance[] {
        //     new MoveInputAcceptance() { FrameRange = new FrameIndexRange(0, 66), Rate = 1f }
        // };
        // actionList.Add(action);

        /*var JumpStart = Resources.Load<ActionConfig>("Prefabs/ScriptableObjects/ActionConfig/ch_02/JumpStart");
        if (JumpStart)
        {
            ActionInfo action = ActionUtility.CreateActionInfo(JumpStart);
            actionList.Add(action);
        }*/
        // action           = new ActionInfo();
        // action.name      = "JumpStart";
        // action.animation = "pl0000_00016";
        // for (int i = 0; i < 56; i++)
        // {
        //     action.actionFrameInfos.Add(new ActionFrameInfo() { frameId = i });
        // }
        // action.commandList.Add(new ActionCommand() { keySequences = new[] { KeyMap.ButtonA }, validInFrameCount = 2 });
        // action.priority           = 1;
        // action.keepPlayingAnim    = false;
        // action.autoNextActionName = "JumpLoop";
        // action.autoTerminate      = false;
        // action.cancelTags         = new CancelTag[] {
        //     new CancelTag() { tag = "IdleAction", priority = 1, startFromFrameIndex = 0 },
        //     new CancelTag() { tag = "RunAction", priority = 2, startFromFrameIndex = 0 }
        // };
        // action.beCanceledTags = new BeCanceledTag[] {
        //     new BeCanceledTag() { cancelTag = new string[] { "JumpStartLand" }, priority = 1, validRange = new FrameIndexRange(0, 55) }
        // };
        // var enterActionMethod   = Resources.Load<ScriptMethodInfo>("Prefabs/ScriptableObjects/ScriptMethodInfo/ActionEvent/EnterJump");
        // action.enterActionEvent = enterActionMethod;
        // action.ApplyGravity         = true;
        // action.MoveInputAcceptances = new MoveInputAcceptance[] {
        //     new MoveInputAcceptance() { FrameRange = new FrameIndexRange(0, 55), Rate = 1f }
        // };
        // actionList.Add(action);

        /*var JumpLoop = Resources.Load<ActionConfig>("Prefabs/ScriptableObjects/ActionConfig/ch_02/JumpLoop");
        if (JumpLoop)
        {
            ActionInfo action = ActionUtility.CreateActionInfo(JumpLoop);
            actionList.Add(action);
        }*/
        // action           = new ActionInfo();
        // action.name      = "JumpLoop";
        // action.animation = "pl0000_00017";
        // for (int i = 0; i < 10; i++)
        // {
        //     action.actionFrameInfos.Add(new ActionFrameInfo() { frameId = i });
        // }
        // //action.commandList.Add(new ActionCommand() { keySequences = new[] { KeyMap.ButtonA }, validInFrameCount = 1 });
        // action.priority           = 1;
        // action.keepPlayingAnim    = true;
        // action.autoNextActionName = "JumpLoop";
        // action.autoTerminate      = true;
        // action.cancelTags         = new CancelTag[] {
        //     //new CancelTag() { tag = "IdleAction", priority = 1, startFromFrameIndex = 0 },
        //     //new CancelTag() { tag = "RunAction", priority = 1, startFromFrameIndex = 0 }
        // };
        // action.beCanceledTags = new BeCanceledTag[] {
        //     new BeCanceledTag() { cancelTag = new string[] { "JumpLoopLand" }, priority = 1, validRange = new FrameIndexRange(0, 9) }
        // };
        // action.ApplyGravity    = true;
        // actionList.Add(action);

        /*var JumpLand = Resources.Load<ActionConfig>("Prefabs/ScriptableObjects/ActionConfig/ch_02/JumpLand");
        if (JumpLand)
        {
            ActionInfo action = ActionUtility.CreateActionInfo(JumpLand);
            actionList.Add(action);
        }*/
        // action           = new ActionInfo();
        // action.name      = "JumpLand";
        // action.animation = "pl0000_00018";
        // for (int i = 0; i < 251; i++)
        // {
        //     action.actionFrameInfos.Add(new ActionFrameInfo() { frameId = i });
        // }
        // //action.commandList.Add(new ActionCommand() { keySequences = new[] { KeyMap.ButtonA }, validInFrameCount = 1 });
        // action.priority           = 2;
        // action.keepPlayingAnim    = false;
        // action.autoNextActionName = "Idle";
        // action.autoTerminate      = false;
        // action.cancelTags         = new CancelTag[] {
        //     new CancelTag() { tag = "JumpStartLand", priority = 1, startFromFrameIndex = 0 },
        //     new CancelTag() { tag = "JumpLoopLand", priority = 1, startFromFrameIndex = 0 }
        // };
        // action.beCanceledTags = new BeCanceledTag[] {
        //     new BeCanceledTag() { cancelTag = new string[] { "JumpLand" }, priority = 1, validRange = new FrameIndexRange(0, 250) }
        // };
        // action.ApplyGravity    = true;
        // rootMotionMethod       = Resources.Load<ScriptMethodInfo>("Prefabs/ScriptableObjects/ScriptMethodInfo/RootMotion/JumpLand");
        // action.rootMotionTween = rootMotionMethod;
        // actionList.Add(action);


        gameEntity.AddAction(gameEntity.id.value, actionList);
        var component   = gameEntity.CreateCurrentAction();
        component.value = actionList[0];
        //component.beCanceledTags.Add(actionList[0].beCanceledTags[0]);
        gameEntity.ReplaceCurrentAction(component);

        gameEntity.AddPreorderAction(new());

        gameEntity.AddHitRecord(new List<HitRecordInfo>());
        gameEntity.AddAttackHitResult(new());
        gameEntity.AddTempBeCancelTag(new());

        gameEntity.AddAttackHitBox(new());
        gameEntity.AddHitBox(new());
    }

    void OnDestroy()
    {
        characterController.OnDestroyView();
        if (gameEntity != null)
        {
            contexts.game.timerManager.OnRemoveUnitTimer(gameEntity.id.value);
            gameEntity.ReplaceAnimationController(null);
            gameEntity.ReplaceACTGameKCCMotor(null);
            gameEntity.ReplaceInputToCommand(0, new());
            gameEntity.ReplaceAction(0, new());
            gameEntity.ReplaceCurrentAction(null);
            gameEntity.ReplacePreorderAction(new());
            gameEntity.ReplaceHitRecord(new());
            gameEntity.ReplaceAttackHitResult(new());
            gameEntity.ReplaceTempBeCancelTag(new());
            gameEntity.ReplaceAttackHitBox(new());
            gameEntity.ReplaceHitBox(new());
            gameEntity.inputController.instance.Destroy();
            gameEntity.ReplaceInputController(null);
            //gameObject.Unlink();
        }
        gameEntity = null;
        contexts   = null;

        animationController?.OnDestroy();
        animationController = null;
    }

    public Vector3 GetMoveInputVector()
    {
        var inputController = gameEntity.inputController.instance as UnityInputController;
        // Move and look inputs
        var _moveInputVector = inputController.CharacterRelativeFlatten(inputController.MoveInputVector);
        return _moveInputVector;
    }

    private void LateUpdate()
    {
    }
}