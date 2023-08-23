using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.ComponentModel;
using System.IO;
using System;
using System.Linq;
using Newtonsoft.Json;

public class CustomRGBA : MonoBehaviour
{
    public int R;
    public int G;
    public int B;
    public float A;
    public int R_max = 255;
    public int G_max = 255;
    public int B_max = 255;
    public float A_max = 1;
    public int R_min = 0;
    public int G_min = 0;
    public int B_min = 0;
    public float A_min = 0;

    float[] vs;

    private void OnDrawGizmos()
    {
        if (vs == null)
        {
            vs = new float[] { R, G, B, A };
        }
        else if (vs[0] != R || vs[1] != G || vs[2] != B || vs[3] != A)
        {
            foreach (PrimitiveComponent component in transform.GetComponentsInChildren<PrimitiveComponent>())
            {
                component.Color = new Color(R / 255f, G / 255f, B / 255f, A);
                MeshRenderer renderer = component.gameObject.GetComponent<MeshRenderer>();
                if (_sharedRegular == null)
                    _sharedRegular = new Material((Material)Resources.Load("Materials/Regular"));

                if (_sharedTransparent == null)
                    _sharedTransparent = new Material((Material)Resources.Load("Materials/Transparent"));

                renderer.sharedMaterial = component.Color.a >= 1f ? _sharedRegular : _sharedTransparent;
                renderer.sharedMaterial.color = component.Color;
            }
            vs = new float[] { R, G, B, A };
        }
    }

    private static readonly Config Config = SchematicManager.Config;

    [MenuItem("SchematicManager/Compile Custom RGBAs", priority = -12)]
    public static void Compile()
    {
        foreach (Schematic s in GameObject.FindObjectsOfType<Schematic>())
        {
            CompileSchematic(s);
        }
    }

    public static void CompileSchematic(Schematic schematic)
    {
        string parentDirectoryPath = Directory.Exists(Config.ExportPath)
            ? Config.ExportPath
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "MapEditorReborn_CompiledSchematics");

        string schematicDirectoryPath = Path.Combine(parentDirectoryPath, schematic.gameObject.name, $"{schematic.gameObject.name}.json");

        if (!Directory.Exists(parentDirectoryPath))
            return;
        if (!File.Exists(schematicDirectoryPath))
        {
            Debug.Log("You should first compile schematic then compile Custom RGBAs.");
            return;
        }

        SchematicObjectDataList list = JsonConvert.DeserializeObject<SchematicObjectDataList>(File.ReadAllText(schematicDirectoryPath));

        foreach (PrimitiveComponent primitive in schematic.GetComponentsInChildren<PrimitiveComponent>())
        {
            CustomRGBA custom = primitive.gameObject.GetComponentInParent<CustomRGBA>();
            if (custom != null)
            {
                SchematicBlockData data = list.Blocks.Find(x => x.ObjectId == primitive.transform.GetInstanceID());
                if (data == null)
                {
                    continue;
                }
                //SchematicBlockData blockData = new SchematicBlockData
                //{
                //    AnimatorName = data.AnimatorName,
                //    BlockType = data.BlockType,
                //    Name = data.Name,
                //    ObjectId = data.ObjectId,
                //    ParentId = data.ParentId,
                //    Position = data.Position,
                //    Rotation = data.Rotation,
                //    Scale = data.Scale,
                //    Properties = data.Properties
                //};
                if (data.Properties.ContainsKey("Color"))
                {
                    data.Properties["Color"] = string.Format("{0}:{1}:{2}:{3}", custom.R, custom.G, custom.B, custom.A);
                }
                //list.Blocks.Remove(data);
                //list.Blocks.Add(blockData);
            }
        }
        SchematicObjectDataList list1 = new SchematicObjectDataList
        {
            Blocks = list.Blocks,
            RootObjectId = list.RootObjectId
        };

        File.WriteAllText(schematicDirectoryPath, JsonConvert.SerializeObject(list1, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        Debug.Log("Successfully compiled Custom RGBAs");
    }

    private Material _sharedRegular;
    private Material _sharedTransparent;
}

[CustomEditor(typeof(CustomRGBA))]
public class SliderEditor : Editor
{
    SerializedProperty R;
    SerializedProperty G;
    SerializedProperty B;
    SerializedProperty A;
    SerializedProperty R_M;
    SerializedProperty G_M;
    SerializedProperty B_M;
    SerializedProperty A_M;
    SerializedProperty R_m;
    SerializedProperty G_m;
    SerializedProperty B_m;
    SerializedProperty A_m;

    private void OnEnable()
    {
        R = serializedObject.FindProperty("R");
        G = serializedObject.FindProperty("G");
        B = serializedObject.FindProperty("B");
        A = serializedObject.FindProperty("A");
        R_M = serializedObject.FindProperty("R_max");
        G_M = serializedObject.FindProperty("G_max");
        B_M = serializedObject.FindProperty("B_max");
        A_M = serializedObject.FindProperty("A_max");
        R_m = serializedObject.FindProperty("R_min");
        G_m = serializedObject.FindProperty("G_min");
        B_m = serializedObject.FindProperty("B_min");
        A_m = serializedObject.FindProperty("A_min");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(R);
        EditorGUILayout.PropertyField(G);
        EditorGUILayout.PropertyField(B);
        EditorGUILayout.PropertyField(A);

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(R_M);EditorGUILayout.PropertyField(R_m);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(G_M);EditorGUILayout.PropertyField(G_m);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(B_M);EditorGUILayout.PropertyField(B_m);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(A_M);EditorGUILayout.PropertyField(A_m);
        GUILayout.EndHorizontal();

        R.intValue = Mathf.RoundToInt(EditorGUILayout.Slider("R value", R.intValue, R_m.intValue, R_M.intValue));
        G.intValue = Mathf.RoundToInt(EditorGUILayout.Slider("G value", G.intValue, G_m.intValue, G_M.intValue));
        B.intValue = Mathf.RoundToInt(EditorGUILayout.Slider("B value", B.intValue, B_m.intValue, B_M.intValue));
        A.floatValue = EditorGUILayout.Slider("A value", A.floatValue, A_m.floatValue, A_M.floatValue);

        serializedObject.ApplyModifiedProperties();
    }
}
