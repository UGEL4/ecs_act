using UnityEngine;

[CreateAssetMenu(fileName = "ScriptMethodInfo", menuName = "脚本函数/ScriptMethodInfo")]
public class ScriptMethodInfo : ScriptableObject
{
    public string MethodName;
    public string[] Params;
}