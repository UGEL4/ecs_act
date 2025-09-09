using Slate;
using UnityEngine;

namespace ACTGame
{
    public class BaseActionClip : ActionClip
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

        public int TotalFrame
        {
            get {
                int frameRate  = Prefs.frameRate;
                int totalFrame = (int)(length * frameRate);
                return totalFrame;
            }
        }

        public int StartFrame
        {
            get {
                int frameRate  = Prefs.frameRate;
                int startFrame = (int)(startTime * frameRate);
                return startFrame;
            }
        }
    }
}