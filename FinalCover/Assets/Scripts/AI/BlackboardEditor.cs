using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Blackboard))]
public class BlackboardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var bb = (Blackboard)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Known Targets (Runtime View)", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Only Updated in Playmode.", MessageType.Info);
            return;
        }

        if (bb.KnownTargets == null || bb.KnownTargets.Count == 0)
        {
            EditorGUILayout.LabelField("No known targets.");
            return;
        }

        foreach (var kvp in bb.KnownTargets)
        {
            var go = kvp.Key;
            var mem = kvp.Value;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.ObjectField("Target", go, typeof(GameObject), true);
            EditorGUILayout.Vector3Field("Last Pos", mem.LastKnownPos);
            EditorGUILayout.FloatField("Last Time", mem.LastKnownTime);
            EditorGUILayout.FloatField("Awareness", mem.LastKnownAwareness);
            EditorGUILayout.Toggle("Is Visible", mem.IsVisible);
            EditorGUILayout.EndVertical();
        }
    }
}
