using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HorizontalArray))]
public class HorizontalArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty valuesProperty = property.FindPropertyRelative("line");

        valuesProperty.arraySize = 3;
        position = EditorGUI.PrefixLabel(position, label);
        Rect fieldRect = new Rect(position.x, position.y, 20, position.height);

        // Loop through the array and display each element horizontally
        for (int i = 0; i < valuesProperty.arraySize; i++)
        {
            SerializedProperty element = valuesProperty.GetArrayElementAtIndex(i);
            EditorGUI.PropertyField(fieldRect, element, GUIContent.none);

            fieldRect.x += 25;
        }
    }
}

