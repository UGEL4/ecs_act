
using System;

[Serializable]
public struct CancelData
{
    public string[] tags;
    public int priority;
    public int startFrame;
    public TempBeCancelledTag[] tempCancelTags;
}