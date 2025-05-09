using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class UnityInputController : MonoBehaviour, IInputController
{
    //private Contexts contexts;
    //private GameEntity entity;
    private IInputService inputService;

    public void Initialize(Contexts contexts, IEntity entity)
    {
        //this.contexts = contexts;
        //this.entity   = (GameEntity)entity;
        inputService  = contexts.input.inputService.instance;
    }

    public List<KeyRecord> GetCommands(long currentFrame)
    {
        List<KeyRecord> commands = new();
        AxisToCommands(currentFrame, commands);
        return commands;
    }

    void AxisToCommands(long frame, List<KeyRecord> commands)
    {
        Vector3 axis      = inputService.Axis;
        Vector3 inputDir  = CharacterRelativeFlatten(axis);
        float dotF        = Vector3.Dot(inputDir, transform.forward);
        float dotR        = Vector3.Dot(inputDir, transform.right);
        float invalidArea = 0.2f;
        bool xHasInput    = Mathf.Abs(dotR) >= invalidArea;
        bool yHasInput    = Mathf.Abs(dotF) >= invalidArea;
        if (!xHasInput && !yHasInput)
        {
            KeyRecord record = new KeyRecord(KeyMap.NoDir, frame);
            commands.Add(record);
        }
        else
        {
            KeyRecord record = new KeyRecord(KeyMap.DirInput, frame);
            commands.Add(record);
        }
    }

    /// <summary>
    /// 获取xz平面上的移动方向向量
    /// </summary>
    /// <param name="input">xz平面上的输入</param>
    /// <returns>返回移动方向单位向量</returns>
    Vector3 CharacterRelativeFlatten(Vector3 input)
    {
        Camera mainCamera = Camera.main;
        if (!mainCamera)
        {
            return Vector3.zero;
        }
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right   = mainCamera.transform.right;
        forward.y       = 0f;
        forward.Normalize();
        right.y = 0f;
        right.Normalize();
        Vector3 Move = forward * input.z + right * input.x;
        Move.Normalize();
        return Move;
    }
}