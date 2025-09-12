
using System;

[Serializable]
public struct CancelData
{
    public string[] Tags;
    public int Priority;
    public int StartFrame;
    public float FadeInPercentage;
    public float AnimStartFromPercentage;
    public TempBeCancelledTag[] TempCancelTags;
}