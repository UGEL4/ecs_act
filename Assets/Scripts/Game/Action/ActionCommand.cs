using System;

[Serializable]
public struct ActionCommand
{
    public KeyMap[] keySequences;
    public int validInFrameCount;
}