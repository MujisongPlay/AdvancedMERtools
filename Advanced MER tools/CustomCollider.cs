using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CustomCollider : FakeMono
{
    public CCDTO data = new CCDTO();
}

[Serializable]
public class CCDTO : DTO
{
    public ColliderActionType ColliderActionType;
	public CollisionType CollisionType;
    public DetectType DetectType;
    public float ModifyHealthAmount;
	public List<AnimationDTO> AnimationModules;
	public WarheadActionType warheadActionType;
	public List<MessageModule> MessageModules;
	public List<DropItem> dropItems;
	public List<Commanding> commandings;
	public List<ExplodeModule> ExplodeModules;
	public List<EffectGivingModule> effectGivingModules;
    public List<AudioModule> AudioModules;
    public List<CGNModule> GroovieNoiseToCall;
}

public class CustomColliderCompiler
{
    private static readonly Config Config = SchematicManager.Config;

    [MenuItem("SchematicManager/Compile custom collider", priority = -11)]
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

        List<CCDTO> interactables = new List<CCDTO> { };

        foreach (CustomCollider ip in schematic.transform.GetComponentsInChildren<CustomCollider>())
        {
            ip.data.AnimationModules.ForEach(x => 
            {
                x.AnimatorAdress = PublicFunctions.FindPath(x.Animator.transform);
            });
            ip.data.Code = ip.GetInstanceID();
            ip.data.ObjectId = PublicFunctions.FindPath(ip.transform);
            interactables.Add(ip.data);
        }

        string serializedData = JsonConvert.SerializeObject(interactables, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-Colliders.json"), serializedData);
        Debug.Log("Successfully Imported Custom Colliders.");
    }
}
