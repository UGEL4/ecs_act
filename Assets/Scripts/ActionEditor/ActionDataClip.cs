using System.Collections.Generic;
using ACT_SLATE;
using Slate;

namespace ACTGame
{
    //[Attachable(typeof(ActionDataTrack))]
    [Category("Custom/ActionData")]
    public class ActionDataClip : ACT_ParadoxNotionActionBase
    {
        public string ActionName;
        public string Animation;
        public int Priority;
        public NextAutoActionInfo AutoNextAction;
        public bool AutoTerminate;
        public bool KeepPlayingAnim;
        public bool ApplyGravity;
        public ScriptMethodInfo RootMotionTween;
        public ScriptMethodInfo EnterActionEvent;
        public List<ActionCommand> CommandList = new();
    }
}