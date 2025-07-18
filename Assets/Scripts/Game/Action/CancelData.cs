
using System;
[Serializable]
public class CancelData
{
    public string[] tags = new string[0];
    public int priority = 0;
    public int startFrame = 0;
    public TempBeCancelledTag[] tempCancelTags = new TempBeCancelledTag[0];
}