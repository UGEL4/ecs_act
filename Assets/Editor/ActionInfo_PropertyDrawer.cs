using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ActionInfo))]
public class ActionInfo_PropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new VisualElement();

        // Create drawer UI using C#
        var popup = new UnityEngine.UIElements.PopupWindow();
        popup.text = "Action Details";
        popup.Add(new PropertyField(property.FindPropertyRelative("name"), "Action Name"));
        popup.Add(new PropertyField(property.FindPropertyRelative("animation"), "Animation"));
        container.Add(popup);

        return container;
    }
}
