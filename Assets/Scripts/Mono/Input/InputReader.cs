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
    public UnityAction<Vector2, InputActionPhase> Move = delegate {};
    public UnityAction<Vector2> Look = delegate {};
    public UnityAction<InputActionPhase> AttackA = delegate {};
    public UnityAction<InputActionPhase> AttackB = delegate {};
    public UnityAction<InputActionPhase> ButtonA = delegate {};
    public UnityAction<InputActionPhase> ButtonB = delegate {};
    public UnityAction<InputActionPhase> Jump = delegate {};

    PlayerInput input;
    public Vector3 Direction => input.Player.Move.ReadValue<Vector2>();

    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>(), context.phase);
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        Look.Invoke(context.ReadValue<Vector2>());
    }
    public void OnFire(InputAction.CallbackContext context)
    {

    }

    public void OnAttackA(InputAction.CallbackContext context)
    {
        AttackA.Invoke(context.phase);
    }
    public void OnAttackB(InputAction.CallbackContext context)
    {
        AttackB.Invoke(context.phase);
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

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump.Invoke(context.phase);
    }

    public void OnButtonA(InputAction.CallbackContext context)
    {
        ButtonA.Invoke(context.phase);
    }

    public void OnButtonB(InputAction.CallbackContext context)
    {
        ButtonB.Invoke(context.phase);
    }
}
