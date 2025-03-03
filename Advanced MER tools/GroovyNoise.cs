using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.ComponentModel;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

public class GroovyNoise : FakeMono
{
    public new GNDTO data = new();
    public new FGNDTO ScriptValueData = new();
    public override DTO _data { get => data; }
    public override DTO _ScriptValueData { get => ScriptValueData; }
}

[Serializable]
public class GNDTO : DTO
{
    public override void OnValidate()
    {
    }

    public List<GMDTO> Settings;
}

[Serializable]
public class FGNDTO : DTO
{
    public override void OnValidate()
    {
        Settings.ForEach(x => x.OnValidate());
    }

    public List<FGMDTO> Settings;
}

public class GroovyNoiseCompiler : MonoBehaviour
{
    private static readonly Config Config = SchematicManager.Config;

    [MenuItem("SchematicManager/Compile Groovy Noise", priority = -11)]
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

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-GroovyNoises.json"), JsonConvert.SerializeObject(schematic.transform.GetComponentsInChildren<GroovyNoise>().Where(x => !x.UseScriptValue).Select(x => x.data), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-FGroovyNoises.json"), JsonConvert.SerializeObject(schematic.transform.GetComponentsInChildren<GroovyNoise>().Where(x => x.UseScriptValue).Select(x => x.ScriptValueData), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto }).Replace("Assembly-CSharp", "AdvancedMERTools"));
        Debug.Log("Successfully Imported Groovy Noises!");
    }
}

