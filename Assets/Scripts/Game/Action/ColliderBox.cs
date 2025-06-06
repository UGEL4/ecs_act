using System;
using Unity.Mathematics;

[Serializable]
public struct ColliderBox
{
    public float3 size;
    public float3 center;
    public float3 rotation;
    public float3 position;
}