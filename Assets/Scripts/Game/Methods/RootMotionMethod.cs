using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动作的RootMotion的移动函数
/// 参数1:float：当前动作进行到的百分比
/// 参数2:string[]：配置在ActionInfo表里actionInfo.rootMotionTween的param部分
/// 返回值：Vector3，偏移量，假设起始的时候坐标为zero，到normalized==参数float的时候，当时的偏移值
/// </summary>
public static class RootMotionMethod
{
    public static Dictionary<string, Func<int, string[], Vector3>> Methods =
    new Dictionary<string, Func<int, string[], Vector3>> {
        //---------------------------直线向前----------------------------------------
        {
            "GoStraight",
            (frame, param) =>
            {
                float totalDis = param.Length > 0 ? float.Parse(param[0]) : 0;
                int startFrame = param.Length > 1 ? int.Parse(param[1]) : 0;
                int endFrame   = param.Length > 2 ? int.Parse(param[2]) : 1;
                int totalFrame = endFrame - startFrame;
                float pec      = (frame - startFrame) * 1f / totalFrame;
                return frame <= startFrame ? Vector3.zero :
                       frame >= endFrame   ? new Vector3(0, 0, totalDis) :
                                             new Vector3(0, 0, pec * totalDis);
            }
        },
        {
            "JumpLand",
            (frame, param) =>
            {
                float totalDis = param.Length > 0 ? float.Parse(param[0]) : 0;
                int startFrame = param.Length > 1 ? int.Parse(param[1]) : 0;
                int endFrame   = param.Length > 2 ? int.Parse(param[2]) : 1;
                int totalFrame = endFrame - startFrame;
                float pec      = frame * 1f / totalFrame;
                return frame <= startFrame ? Vector3.zero :
                       frame >= endFrame   ? new Vector3(0, 0, totalDis) :
                                             new Vector3(0, 0, pec * totalDis);
            }
        },
    };

    public static Vector3 Jump(int frame, string param)
    {
        float mTimeToJumpApex = 0.5f;
        float mJumpHeight     = 2f;
        float time            = mTimeToJumpApex * 0.5f;
        float Gravity         = -2 * mJumpHeight / Mathf.Pow(time, 2);
        float mJumpVelocity   = 2 * mJumpHeight / time;
        return new Vector3(0, mJumpVelocity, 0);
    }
}