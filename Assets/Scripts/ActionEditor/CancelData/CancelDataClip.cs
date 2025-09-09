using System.Collections.Generic;
using Slate;
using UnityEngine;

namespace ACTGame
{
    [Attachable(typeof(CancelDataTrack))]
    [Category("Custom/CancelData")]
    public class CancelDataClip : BaseActionClip
    {
        public List<CancelData> cancelData = new();


        #if UNITY_EDITOR
        override public void OnStartTimeChanged(float old)
        {
            //Debug.Log($"CancelData OnStartTimeChanged: {old}, {startTime}")
        }

        override public void OnLengthChanged(float old)
        {
            //Debug.Log($"CancelData OnStartTimeChanged: {old}, {length}");
        }
        #endif
    }
}