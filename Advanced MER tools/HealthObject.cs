using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class HealthObject : MonoBehaviour
{
    public float Health = 0;
    public int ArmorEfficient = 0;
    public DeadType DeadType = DeadType.Disappear;
}

[Serializable]
public class HealthObjectDTO
{
    public float Health;
    public int ArmorEfficient;
    public DeadType DeadType;
    public string ObjectId;
}

[Serializable]
public enum DeadType
{
    Disappear,
    GetRigidbody,
    DynamicDisappearing,
    Explode,
    ResetHP
}

public class HealthObjectCompiler : MonoBehaviour
{
    private static readonly Config Config = SchematicManager.Config;

    [MenuItem("SchematicManager/Compile Health Schematics", priority = -11)]
    public static void OnCompile()
    {
        foreach (Schematic schematic in GameObject.FindObjectsOfType<Schematic>())
        {
            Compile(schematic);
        }
    }

    public static void Compile(Schematic schematic)
    {
        string parentDirectoryPath = Directory.Exists(Config.ExportPath)
            ? Config.ExportPath
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "MapEditorReborn_CompiledSchematics");
        string schematicDirectoryPath = Path.Combine(parentDirectoryPath, schematic.gameObject.name);

        if (!Directory.Exists(parentDirectoryPath))
        {
            Debug.LogError("Could Not find root object's compiled directory!");
            return;
        }

        Directory.CreateDirectory(schematicDirectoryPath);

        List<HealthObjectDTO> healthObjects = new List<HealthObjectDTO> { };

        foreach (HealthObject health in schematic.transform.GetComponentsInChildren<HealthObject>())
        {
            string path = "";
            Transform transform = health.transform;
            while (true)
            {
                if (transform.parent == null) break;
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    if (transform.parent.GetChild(i) == transform)
                    {
                        path += i.ToString();
                    }
                }
                transform = transform.parent;
            }
            HealthObjectDTO dTO = new HealthObjectDTO
            {
                Health = health.Health,
                ArmorEfficient = health.ArmorEfficient,
                DeadType = health.DeadType,
                ObjectId = path
            };
            healthObjects.Add(dTO);
        }

        string serializedData = JsonConvert.SerializeObject(healthObjects, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-HealthObjects.json"), serializedData);
        Debug.Log("Successfully Imported HealthObjects.");
    }
}