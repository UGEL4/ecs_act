using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static PlayerInput;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public UnityAction<Vector2, InputActionPhase> LeftStick = delegate {};
    public UnityAction<Vector2, InputActionPhase> RightStick = delegate {};
    public UnityAction<InputActionPhase> Button1 = delegate {};
    public UnityAction<InputActionPhase> Button2 = delegate {};
    public UnityAction<InputActionPhase> Button3 = delegate {};
    public UnityAction<InputActionPhase> Button4 = delegate {};

    PlayerInput input;

    public void OnLeftStick(InputAction.CallbackContext context)
    {
        LeftStick.Invoke(context.ReadValue<Vector2>(), context.phase);
    }
    public void OnRightStick(InputAction.CallbackContext context)
    {
        RightStick.Invoke(context.ReadValue<Vector2>(), context.phase);
    }

    public void OnButton1(InputAction.CallbackContext context)
    {
        Button1.Invoke(context.phase);
    }

    public void OnButton2(InputAction.CallbackContext context)
    {
        Button1.Invoke(context.phase);
    }
    
    public void OnButton3(InputAction.CallbackContext context)
    {
        Button3.Invoke(context.phase);
    }

    public void OnButton4(InputAction.CallbackContext context)
    {
        Button4.Invoke(context.phase);
    }

    private void OnEnable()
    {
        if (input == null)
        {
            input = new PlayerInput();
            input.Player.SetCallbacks(this);
        }
        input.Enable();
    }

    private void OnDisable()
    {
        if (input != null) input.Disable();
    }
}
