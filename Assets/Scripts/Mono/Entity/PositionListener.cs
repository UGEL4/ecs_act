using Entitas;
using Unity.Mathematics;
using UnityEngine;

public class PositionListener : MonoBehaviour, IEventListener, IPositionListener
{
    GameEntity gameEntity;
    public void OnPosition(GameEntity entity, Vector3 value)
    {
        transform.localPosition = value;
    }

    public void OnPosition(GameEntity entity, float3 value)
    {
        //throw new System.NotImplementedException();
    }

    public void RegisterListeners(IEntity entity)
    {
        gameEntity = (GameEntity)entity;
        gameEntity.AddPositionListener(this);
    }
}