using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

[Game]
public sealed class InputToCommandComponent : IComponent
{
    public long entityId;
    public List<KeyRecord> commands;
}