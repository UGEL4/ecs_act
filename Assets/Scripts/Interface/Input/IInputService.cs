using System.Collections.Generic;
using Entitas;
using UnityEngine;

public interface IInputService
{
    public Vector2 Axis { get; }
    public Vector3 Rotate { get; }

    public void Initialize(Contexts contexts, IEntity entity);
    public void OnEnable();
    public void OnDisable();
    public void Destroy();
}