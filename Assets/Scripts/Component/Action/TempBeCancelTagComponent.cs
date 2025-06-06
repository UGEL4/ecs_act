using System.Collections.Generic;
using Entitas;

[Game]
public class TempBeCancelTagComponent : IComponent
{
    //动作切换之后，开启的tempBeCancelTag，这是根据ActionChangeInfo的数据添加进来的
    public List<TempBeCancelledTag> values;
}