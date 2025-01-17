using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SlotCombinationSO))]
public class CombinationLineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
        GUI.enabled = false; // Make the field non-editable
        EditorGUILayout.PropertyField(scriptProperty, true);
        GUI.enabled = true; 


        SlotCombinationSO data = (SlotCombinationSO)target;

        // Column Count Field
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Column Count", GUILayout.Width(100));
        data.columnCount = Mathf.Max(1, EditorGUILayout.IntField(data.columnCount)); // Ensure >= 1
        EditorGUILayout.EndHorizontal();

        // Add spacing for better visuals
        EditorGUILayout.Space();

        // Combination Table Label
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Combination Table", EditorStyles.boldLabel, GUILayout.Width(175f));
        EditorGUILayout.LabelField("Columns", GUILayout.Width(Mathf.Max(EditorGUIUtility.currentViewWidth-225f,75f)));
        
        //int newSize = Mathf.Max(1, EditorGUILayout.IntField(data.combinations.Length)); // Ensure >= 1
         
        //for (int i = data.combinations.Length; i < newSize; i++)
        //{
        //    ArrayUtility.Add(ref data.combinations, new CombinationEntry
        //    {
        //        combinationCode = 0,
        //        rowIndex = new int[data.columnCount]
        //    });
        //}

        EditorGUILayout.LabelField($"{data.combinations.Length}");

        EditorGUILayout.EndHorizontal();
        // Ensure all rows match column count
        foreach (var entry in data.combinations)
        {
            if (entry.rowIndex == null || entry.rowIndex.Length != data.columnCount)
            {
                entry.rowIndex = new int[data.columnCount];
            }
        }

        // Display Combination Table
        for (int i = 0; i < data.combinations.Length; i++)
        {
            var entry = data.combinations[i];

            EditorGUILayout.BeginHorizontal();

            // Calculate dynamic widths
            float totalWidth = EditorGUIUtility.currentViewWidth; // Total available width
            float codeLabelWidth = 40f; // Fixed width for "Code" label
            float codeFieldWidth = 50f; // Fixed width for the input field
            float rowIndexLabelWidth = 70f; // Fixed width for "Row Index" label
            float spacing = 10f; // Spacing between sections
            float remainingWidth = totalWidth - (codeLabelWidth + codeFieldWidth + rowIndexLabelWidth + spacing * 2 + 50f); // 50f buffer for scrollbar
            float fieldWidth = remainingWidth / data.columnCount; // Divide remaining space among columns

            // Code Field (Renamed from Combination Code)
            EditorGUILayout.LabelField("Code", GUILayout.Width(codeLabelWidth)); // Compact label
            entry.combinationCode = EditorGUILayout.IntField(entry.combinationCode, GUILayout.Width(codeFieldWidth));

            EditorGUILayout.Space(spacing); // Add spacing between Code and Row Index

            // Row Index Label
            EditorGUILayout.LabelField("Row Index", GUILayout.Width(rowIndexLabelWidth));

            // Row Index as Horizontal Array
            for (int j = 0; j < entry.rowIndex.Length; j++)
            {
                entry.rowIndex[j] = EditorGUILayout.IntField(entry.rowIndex[j], GUILayout.Width(fieldWidth));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5); // Add space between rows
        }

        // "+" Button to Add Rows
        if (GUILayout.Button("+"))
        {
            ArrayUtility.Add(ref data.combinations, new CombinationEntry
            {
                combinationCode = 0,
                rowIndex = new int[data.columnCount]
            });
        }

        // "-" Button to Remove Rows
        if (GUILayout.Button("-"))
        {
            if (data.combinations.Length > 0)
            {
                ArrayUtility.RemoveAt(ref data.combinations, data.combinations.Length - 1);
            }
        }

        // Apply Changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
