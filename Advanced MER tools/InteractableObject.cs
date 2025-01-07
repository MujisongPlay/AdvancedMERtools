using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
//using NaughtyAttributes;
using System;
using System.IO;
using UnityEditor;
using Newtonsoft.Json;

public class InteractableObject : FakeMono
{
    public IODTO data = new IODTO();
}

[Serializable]
public class IODTO : DTO
{
    public KeyCode InputKeyCode;
    public float InteractionMaxRange;
    public IPActionType ActionType;
    public List<AnimationDTO> AnimationModules;
    public WarheadActionType warheadActionType;
    public List<MessageModule> MessageModules;
    public List<DropItem> dropItems;
    public List<Commanding> commandings;
    public Scp914Mode Scp914Mode;
    public List<ExplodeModule> ExplodeModules;
    public List<EffectGivingModule> effectGivingModules;
    public List<AudioModule> AudioModules;
    public List<CGNModule> GroovieNoiseToCall;
}

public class InteractableObjectCompiler
{
    private static readonly Config Config = SchematicManager.Config;

    [MenuItem("SchematicManager/Compile Interactable Objects", priority = -11)]
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

        List<IODTO> interactables = new List<IODTO> { };

        foreach (InteractableObject ip in schematic.transform.GetComponentsInChildren<InteractableObject>())
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

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-Objects.json"), serializedData);
        Debug.Log("Successfully Imported Interactable Objects.");
    }
}