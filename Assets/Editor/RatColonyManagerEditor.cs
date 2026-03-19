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
            RatColony colony = manager.ratColonies[i];
            EditorGUILayout.LabelField($"Colony {colony.id} ({colony.members.Count} rats):");
            EditorGUI.indentLevel++;
            foreach (Rat rat in colony.members)
            {
                EditorGUILayout.ObjectField(rat != null ? rat.gameObject.name : "null", rat, typeof(Rat), true);
            }
            EditorGUI.indentLevel--;
        }
    }
}