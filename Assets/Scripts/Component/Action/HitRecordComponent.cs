using System.Collections.Generic;
using Entitas;

[Game]
public class HitRecordComponent : IComponent
{
    public List<HitRecordInfo> values;
}