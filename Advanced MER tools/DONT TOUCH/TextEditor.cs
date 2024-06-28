using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Text))]
public class TextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Text textScript = (Text)target;
        if (GUILayout.Button("Add Letters From Folder"))
        {
            textScript.AddLettersFromFolder();
        }

        if (GUILayout.Button("Add Numbers From Folder"))
        {
            textScript.AddNumbersFromFolder();
        }
    }
}
