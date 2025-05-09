using System;

[Serializable]
public enum KeyMap
{
    NoDir = 0,
    NoInput = 1,
    Back,
    Forward,
    Left,
    Right,
    DirInput,
    ButtonX,
    ButtonY,
    ButtonA,
    ButtonB,
}

public class KeyRecord
{
    public long frame;
    public KeyMap key;
    public KeyRecord(KeyMap key, long frame)
    {
        this.key   = key;
        this.frame = frame;
    }
}