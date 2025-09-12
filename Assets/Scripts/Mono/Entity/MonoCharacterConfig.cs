using System.Collections.Generic;
using UnityEngine;

namespace ACTGame.View
{
    [CreateAssetMenu(fileName = "MonoCharacterConfig", menuName = "角色数据")]
    public class MonoCharacterConfig : ScriptableObject
    {
        public List<string> ActionList = new();
    }
}