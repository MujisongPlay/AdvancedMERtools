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
    public GNDTO data = new GNDTO();
}

[Serializable]
public class GNDTO : DTO
{
    public List<GMDTO> Settings;
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

        List<GNDTO> groovies = new List<GNDTO> { };

        foreach (GroovyNoise GN in schematic.transform.GetComponentsInChildren<GroovyNoise>())
        {
            GN.data.ObjectId = PublicFunctions.FindPath(GN.transform);
            GN.data.Code = GN.GetInstanceID();
            groovies.Add(GN.data);
            //GNDTO dTO = new GNDTO
            //{
            //    Active = GN.Active
            //};
            //dTO.GMDTOs = new List<GMDTO> { };
            //GN.Groovies.ForEach(x => dTO.GMDTOs.Add(new GMDTO
            //{
            //    ActionDelay = x.ActionDelay,
            //    ChanceWeight = x.ChanceWeight,
            //    ForceExecute = x.ForceExecute,
            //    codes = x.Activator.Select(y => y.GetInstanceID()).ToList(),
            //    Enable = x.Enable
            //}));
            //dTO.Code = GN.GetInstanceID();
            //dTO.ObjectId = PublicFunctions.FindPath(GN.transform);
            //groovies.Add(dTO);
        }

        string serializedData = JsonConvert.SerializeObject(groovies, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-GroovyNoises.json"), serializedData);
        Debug.Log("Successfully Imported Groovy Noises!");
    }
}

