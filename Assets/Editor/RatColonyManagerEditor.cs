using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RatColonyManager))]
public class RatColonyManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RatColonyManager manager = (RatColonyManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Rat Colonies (Debug View)", EditorStyles.boldLabel);

        for (int i = 0; i < manager.ratColonies.Count; i++)
        {
            EditorGUILayout.LabelField($"Colony {i} ({manager.ratColonies[i].Count} rats):");
            EditorGUI.indentLevel++;
            foreach (var rat in manager.ratColonies[i])
            {
                EditorGUILayout.ObjectField(rat != null ? rat.gameObject.name : "null", rat, typeof(Rat), true);
            }
            EditorGUI.indentLevel--;
        }
    }
}