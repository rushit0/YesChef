using UnityEditor;

namespace YesChef.TutorialInfo.Editor
{
    /// <summary>
    /// Minimal custom editor placeholder for the legacy Unity template readme.
    /// Keeping this tiny avoids accidental editor-time null references from stale project entries.
    /// </summary>
    [CustomEditor(typeof(Readme))]
    public sealed class ReadmeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Template readme placeholder.", MessageType.Info);
            DrawDefaultInspector();
        }
    }
}
