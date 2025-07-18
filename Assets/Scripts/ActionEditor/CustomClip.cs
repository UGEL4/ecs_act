using Slate;
using Slate.ActionClips;

[Attachable(typeof(CustomTrack))]
[Category("Custom")]
public class CustomClip : ActorActionClip
{
    public int value;
}