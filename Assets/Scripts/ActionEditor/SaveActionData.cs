using Slate;
using UnityEngine;

public class SaveActionData : MonoBehaviour
{
    public Cutscene Action {
        get {
            var comp = gameObject.GetComponent<Cutscene>();
            return comp;
        }
    }
}