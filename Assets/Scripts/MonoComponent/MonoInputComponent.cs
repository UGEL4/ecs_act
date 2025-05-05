using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonoInputComponent : MonoBehaviour, IInputService
{
    private Vector2 mAxis;
    public Vector2 Axis => mAxis;

    public Vector3 Rotate => throw new System.NotImplementedException();

    PlayerInput playerInput;
    private Contexts contexts;
    private InputEntity inputEntity;

    void Awake()
    {
        // contexts    = Contexts.sharedInstance;
        // inputEntity = contexts.input.CreateEntity();

        playerInput = new PlayerInput();

        playerInput.Player.Move.performed += OnMove;

        mAxis = Vector2.zero;
    }

    void OnEnable()
    {
        playerInput.Enable();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }

    void OnDestroy()
    {
        playerInput.Player.Move.performed -= OnMove;
    }
    
    void OnMove(InputAction.CallbackContext context)
    {
        mAxis = context.ReadValue<Vector2>();
    }

}
