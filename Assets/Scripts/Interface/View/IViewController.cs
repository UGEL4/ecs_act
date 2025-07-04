using Entitas;
using UnityEngine;

public interface IViewController
{
    Vector3 Position {get; set;}
    Vector3 Rotation {get; set;}
    Vector3 Scale {get; set;}
    bool Active {get; set;}
    void InitializeView(Contexts contexts, IEntity Entity);
    void Link(IEntity entity);
    void Unlink();
    void DestroyView();
    IEntity GetEntity();
}