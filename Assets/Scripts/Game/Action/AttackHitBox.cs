using System;
using ACTGame;
using UnityEngine;

[Serializable]
public struct AttackHitBox
{
    public OrientedBox box;
    public CapsuleCollider capsule;
    public ActionChangeInfo selfActionChangeInfo;
    public ActionChangeInfo targetActionChangeInfo;
    public int priority;
}