using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(ActionEditor))]
public class ActionEditorInspector : Editor
{
    public VisualTreeAsset InspectorUXML;
    private int currentFrame = 0;
    private int maxFrame     = 60;

    void OnEnable()
    {
        // if (actionInfo == null)
        // {
        //     actionInfo = new EditActionObject();
        // }
    }

    void OnDisable()
    {
        currentFrame = 0;
        maxFrame     = 0;
        // actionInfo   = null;
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement inspector = new VisualElement();

        inspector.Add(new Label("Action Editor"));

        //VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ActionEditorInspector.uxml");
        InspectorUXML.CloneTree(inspector);

        var inspectorFoldout = inspector.Q("Default_Inspector");
        InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);

        return inspector;
    }

    void Bake()
    {
        // if (actionInfo == null)
        // {
        //     Debug.LogError("没有选择动作文件");
        //     return;
        // }

        // maxFrame = actionInfo.value.FrameCount;
    }
}
