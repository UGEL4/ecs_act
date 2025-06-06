using System;
using ACTGame;

[Serializable]
public struct HitBox
{
    public OrientedBox box;
    public ActionChangeInfo selfActionChangeInfo;
    public ActionChangeInfo attackerActionChangeInfo;
    public int priority;
}