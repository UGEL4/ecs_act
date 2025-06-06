using System;
using ACTGame;

[Serializable]
public struct AttackHitBox
{
    public OrientedBox box;
    public ActionChangeInfo selfActionChangeInfo;
    public ActionChangeInfo targetActionChangeInfo;
    public int priority;
}