using System.Collections.Generic;
using Slate;
using UnityEngine;

namespace ACTGame
{
    [Attachable(typeof(CancelTagTrack))]
    [Category("Custom/CancelTag")]
    public class CancelTagClip : BaseActionClip
    {
        public List<CancelTag> beCancelTag = new();


        #if UNITY_EDITOR
        override public void OnStartTimeChanged(float old)
        {
            //Debug.Log($"CancelData OnStartTimeChanged: {old}, {startTime}");
        }

        override public void OnLengthChanged(float old)
        {
            //Debug.Log($"CancelData OnStartTimeChanged: {old}, {length}");
        }
        #endif
    }
}