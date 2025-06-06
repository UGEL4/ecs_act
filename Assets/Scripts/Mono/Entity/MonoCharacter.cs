using System.Collections.Generic;
using Entitas;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MonoCharacter : MonoBehaviour, IViewController
{
    private GameEntity gameEntity;
    private Contexts contexts;
    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public Vector3 Rotation
    {
        get { return transform.rotation.eulerAngles; }
        set { transform.rotation = Quaternion.Euler(value); }
    }

    public Vector3 Scale
    {
        get { return transform.localScale; }
        set { transform.localScale = value; }
    }

    public bool Active { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }

    public void DestroyView()
    {
        throw new System.NotImplementedException();
    }

    public void InitializeView(Contexts contexts, IEntity Entity)
    {
        this.contexts = contexts;
        this.gameEntity = (GameEntity)Entity;
        OnCreate();
    }

    void OnCreate()
    {
        long levelId = this.gameEntity.entity.levelId;
        var levelObj = MonoGameObjectInstance.GetLevel(levelId);
        if (levelObj == null)
        {
            Debug.LogError("Could not find level with id:" + levelId);
            return;
        }
        var position = gameEntity.position.value;
        var unityLevel = levelObj.GetComponent<MonoLevel>();
        if (unityLevel != null)
        {
            var levelRootTransform = unityLevel.GetLevelRoot();
            gameObject.transform.SetParent(levelRootTransform);
        }
        else
        {
            gameObject.transform.SetParent(unityLevel.transform);
        }
        gameObject.transform.localPosition = position;

        var controller = gameObject.AddComponent<UnityInputController>();
        controller.Initialize(contexts, gameEntity);
        gameEntity.AddInputController(controller);

        long entityId = gameEntity.id.value;
        gameEntity.AddInputToCommand(entityId, new List<KeyRecord>());

        //updateLogic = new MainRoleUpdater(contexts, gameEntity);
        var sceneTimer = contexts.game.timerManager.GetSceneTimer(contexts);
        if (sceneTimer != null)
        {
            var index = GameComponentsLookup.Timer;
            var component = (TimerComponent)gameEntity.CreateComponent(index, typeof(TimerComponent));
            component.curFrame = sceneTimer.GetNow();
            component.hertz = 60;
            component.accumulator = 0;
            component.isUnitTimer = true;
            //component.NewFrameTimer(InvokeType.EntityUpdate, updateLogic);
            gameEntity.AddComponent(index, component);
            contexts.game.timerManager.OnAddUnitTimer(entityId);
        }
        AddTestActions();
    }

    void AddTestActions()
    {
        List<ActionInfo> actionList = new();
        ActionInfo action = new ActionInfo();
        action.name = "Idle";
        for (int i = 0; i < 30; i++)
        {
            action.actionFrameInfos.Add(new ActionFrameInfo() { frameId = i });
        }
        action.commandList.Add(new ActionCommand() { keySequences = new KeyMap[0], validInFrameCount = 1 });
        action.priority = 0;
        action.keepPlayingAnim = true;
        action.autoNextActionName = "Idle";
        //canceltag
        action.beCanceledTags = new BeCanceledTag[] {
            new BeCanceledTag() { cancelTag = new string[] { "IdleAction" }, priority = 0, validRange = new FrameIndexRange(0, 30) }
        };
        actionList.Add(action);

        action = new ActionInfo();
        action.name = "Walk";
        for (int i = 0; i < 30; i++)
        {
            action.actionFrameInfos.Add(new ActionFrameInfo() { frameId = i });
        }
        action.commandList.Add(new ActionCommand() { keySequences = new[] { KeyMap.DirInput }, validInFrameCount = 1 });
        action.priority = 1;
        action.keepPlayingAnim = true;
        action.autoNextActionName = "Idle";
        action.autoTerminate = true;
        action.cancelTags = new CancelTag[] {
            new CancelTag() { tag = "IdleAction", priority = 1, startFromFrameIndex = 0},
            new CancelTag() { tag = "RunAction", priority = 1, startFromFrameIndex = 0}
        };
        action.beCanceledTags = new BeCanceledTag[] {
            new BeCanceledTag() { cancelTag = new string[] { "RunAction" }, priority = 0, validRange = new FrameIndexRange(0, 30) }
        };
        actionList.Add(action);
        gameEntity.AddAction(gameEntity.id.value, actionList);
        gameEntity.AddCurrentAction(actionList[0], new List<BeCanceledTag>() { actionList[0].beCanceledTags[0] }, 0);

        gameEntity.AddPreorderAction(new());

        gameEntity.AddHitRecord(new List<HitRecordInfo>());
        gameEntity.AddAttackHitResult(new());
        gameEntity.AddTempBeCancelTag(new());

        gameEntity.AddAttackHitBox(new());
        gameEntity.AddHitBox(new());
    }

    //private EntityUpdateLogic updateLogic;
}