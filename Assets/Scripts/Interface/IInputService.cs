using UnityEngine;

public interface IInputService
{
    public Vector2 Axis { get; }
    public Vector3 Rotate { get; }
}