using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute attr = (ShowIfAttribute)attribute;

        if (ShouldShow(property, attr))
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        return 0f; // ”ñ•\Ž¦
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute attr = (ShowIfAttribute)attribute;

        if (!ShouldShow(property, attr))
        {
            return;
        }

        EditorGUI.PropertyField(position, property, label, true);
    }

    private bool ShouldShow(SerializedProperty property, ShowIfAttribute attr)
    {
        SerializedProperty boolProp = property.serializedObject.FindProperty(attr.boolName);

        if (boolProp == null)
        {
            Debug.LogWarning($"bool '{attr.boolName}' ‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ");
            return true;
        }

        return boolProp.boolValue;
    }
}