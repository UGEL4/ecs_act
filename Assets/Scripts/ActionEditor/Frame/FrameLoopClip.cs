using Slate;
using UnityEngine;

namespace ACTGame
{
    [Attachable(typeof(FrameLoopTrack))]
    [Category("Custom/FrameLoop")]
    [Description("拖动改变长度，代表这一帧要循环的帧数，最少1帧")]
    public class FrameLoopClip : ActionClip
    {
        [SerializeField]
        [HideInInspector]
        private float _length = 1;

        public override float length
        {
            get {
                return _length;
            }
            set {
                _length = value;
            }
        }

        public override float blendIn
        {
            get {
                return length;
            }
        }
    }
}