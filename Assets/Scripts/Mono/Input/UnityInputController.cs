using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UnityInputController : IInputController
{
    private MonoCharacter owner;
    private Vector3 moveInputVector  = Vector3.zero;
    public Vector3 MoveInputVector  => moveInputVector;

    private InputReader inputReader;

    //////////////////////
    private Vector2 leftStick;
    private InputActionPhase leftStickPhase;
    private Vector2 rightStick;
    private InputActionPhase rightStickPhase;
    private InputActionPhase button1Phase;
    private InputActionPhase button2Phase;
    private InputActionPhase button3Phase;
    private InputActionPhase button4Phase;
    private UnityAction<InputActionPhase> button4Callback;

    public void SetOwner(MonoCharacter owner)
    {
        this.owner = owner;
    }

    public void Initialize(Contexts contexts)
    {
        inputReader = Resources.Load<InputReader>("Prefabs/ScriptableObjects/InputReader");
        if (inputReader != null)
        {
            inputReader.LeftStick += OnLeftStickInput;
            inputReader.Button1 += OnButton1;
            inputReader.Button2 += OnButton2;
            inputReader.Button3 += OnButton3;
            inputReader.Button4 += OnButton4;
        }
    }

    public void Destroy()
    {
        if (inputReader != null)
        {
            inputReader.LeftStick -= OnLeftStickInput;
            inputReader.Button1 -= OnButton1;
            inputReader.Button2 -= OnButton2;
            inputReader.Button3 -= OnButton3;
            inputReader.Button4 -= OnButton4;
        }
        inputReader = null;
        owner       = null;
    }

    public List<KeyRecord> GetCommands(long currentFrame)
    {
        List<KeyRecord> commands = new();
        GetLeftStickCommand(currentFrame, commands);
        GetButtonCommand(currentFrame, commands);
        return commands;
    }

    void GetLeftStickCommand(long frame, List<KeyRecord> commands)
    {
        Vector3 inputDir  = CharacterRelativeFlatten(moveInputVector);
        float dotF        = Vector3.Dot(inputDir, owner.transform.forward);
        float dotR        = Vector3.Dot(inputDir, owner.transform.right);
        float invalidArea = 0.2f;
        bool xHasInput    = Mathf.Abs(dotR) >= invalidArea;
        bool yHasInput    = Mathf.Abs(dotF) >= invalidArea;
        if (xHasInput || yHasInput)
        {
            KeyRecord record = new KeyRecord(KeyMap.DirInput, frame);
            commands.Add(record);
        }
        else
        {
            if (leftStickPhase == InputActionPhase.Canceled)
            {
                KeyRecord record = new KeyRecord(KeyMap.NoDir, frame);
                commands.Add(record);
            }
        }
    }

    void GetButtonCommand(long frame, List<KeyRecord> commands)
    {
        //1
        if (button1Phase == InputActionPhase.Started || button1Phase == InputActionPhase.Performed)
        {
            KeyRecord record = new KeyRecord(KeyMap.ButtonX, frame);
            commands.Add(record);
        }
        //2
        if (button2Phase == InputActionPhase.Started || button2Phase == InputActionPhase.Performed)
        {
            KeyRecord record = new KeyRecord(KeyMap.ButtonY, frame);
            commands.Add(record);
        }
        //3
        if (button3Phase == InputActionPhase.Started || button3Phase == InputActionPhase.Performed)
        {
            KeyRecord record = new KeyRecord(KeyMap.ButtonB, frame);
            commands.Add(record);
        }
        //4
        if (button4Phase == InputActionPhase.Started || button4Phase == InputActionPhase.Performed)
        {
            KeyRecord record = new KeyRecord(KeyMap.ButtonA, frame);
            commands.Add(record);
        }
    }
    /// <summary>
    /// 获取xz平面上的移动方向向量
    /// </summary>
    /// <param name="input">xz平面上的输入</param>
    /// <returns>返回移动方向单位向量</returns>
    public Vector3 CharacterRelativeFlatten(Vector3 input)
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

    void OnLeftStickInput(Vector2 input, InputActionPhase phase)
    {
        leftStick         = input;
        leftStickPhase    = phase;
        moveInputVector.x = leftStick.x;
        moveInputVector.y = 0;
        moveInputVector.z = input.y;
    }

    void OnButton1(InputActionPhase phase)
    {
        button1Phase = phase;
    }

    void OnButton2(InputActionPhase phase)
    {
        button2Phase = phase;
    }

    void OnButton3(InputActionPhase phase)
    {
        button3Phase = phase;
    }

    void OnButton4(InputActionPhase phase)
    {
        button4Phase = phase;

        button4Callback?.Invoke(phase);
    }

    public void AddButton4Callback(UnityAction<InputActionPhase> callback)
    {
        button4Callback += callback;
    }

    public void RemoveButton4Callback(UnityAction<InputActionPhase> callback)
    {
        button4Callback -= callback;
    }
}