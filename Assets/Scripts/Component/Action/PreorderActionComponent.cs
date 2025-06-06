using System.Collections.Generic;
using Entitas;

[Game]
public sealed class PreorderActionComponent : IComponent
{
    public List<PreorderActionInfo> value;
}