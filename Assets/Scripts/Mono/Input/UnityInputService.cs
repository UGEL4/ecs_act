using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnityInputService : IInputService
{
    private Vector2 mAxis;
    public Vector2 Axis => mAxis;

    public Vector3 Rotate => throw new System.NotImplementedException();

    private PlayerInput playerInput;
    private Contexts contexts;
    private InputEntity inputEntity;

    public void Initialize(Contexts contexts, IEntity entity)
    {
        this.contexts = contexts;
        inputEntity   = (InputEntity)entity;

        playerInput = new PlayerInput();
        playerInput.Player.Move.performed += OnAxis;

        mAxis = Vector2.zero;
    }

    public void OnEnable()
    {
        playerInput.Enable();
    }

    public void OnDisable()
    {
        playerInput.Disable();
    }

    public void Destroy()
    {
        playerInput.Player.Move.performed -= OnAxis;
    }
    
    void OnAxis(InputAction.CallbackContext context)
    {
        mAxis = context.ReadValue<Vector2>();
    }
}