using System.Collections.Generic;
using Entitas;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public interface IInputService
{
    public Vector3 Axis { get; }
    public Vector3 Rotate { get; }

    public void Initialize(Contexts contexts, IEntity entity);
    public void OnEnable();
    public void OnDisable();
    public void Destroy();
    public void AddOnAxisListener(UnityAction<Vector3, InputActionPhase> listener);
    public void RemoveOnAxisListener(UnityAction<Vector3, InputActionPhase> listener);
    
}