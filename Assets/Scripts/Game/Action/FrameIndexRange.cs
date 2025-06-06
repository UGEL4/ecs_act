using System;

[Serializable]
public struct FrameIndexRange
{
    public int min;
    public int max;

    public FrameIndexRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}