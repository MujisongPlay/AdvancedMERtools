using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using NaughtyAttributes;
using System;
using System.IO;
using UnityEditor;
using Newtonsoft.Json;

public class InteractableObject : FakeMono
{
    public new IODTO data = new();
    public new FIODTO ScriptValueData = new();
    public override DTO _data { get => data; }
    public override DTO _ScriptValueData { get => ScriptValueData; }
}

[Serializable]
public class IODTO : DTO
{
    public override void OnValidate()
    {
        AnimationModules.ForEach(x => x.AnimatorAdress = PublicFunctions.FindPath(x.Animator));
    }

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
    public List<CFEModule> FunctionToCall;
}

[Serializable]
public class FIODTO : DTO
{
    public override void OnValidate()
    {
        InteractionMaxRange.OnValidate();
        Scp914Mode.OnValidate();
        warheadActionType.OnValidate();
        AnimationModules.ForEach(x => { x.parent = this; x.OnValidate(); x.AnimatorAdress = PublicFunctions.FindPath(x.Animator); });
        MessageModules.ForEach(x => x.OnValidate());
        dropItems.ForEach(x => x.OnValidate());
        commandings.ForEach(x => x.OnValidate());
        ExplodeModules.ForEach(x => x.OnValidate());
        effectGivingModules.ForEach(x => x.OnValidate());
        AudioModules.ForEach(x => x.OnValidate());
        GroovieNoiseToCall.ForEach(x => x.OnValidate());
        FunctionToCall.ForEach(x => x.OnValidate());
    }

    public KeyCode InputKeyCode;
    public ScriptValue InteractionMaxRange;
    public IPActionType ActionType;
    public ScriptValue Scp914Mode;
    public List<FAnimationDTO> AnimationModules;
    public ScriptValue warheadActionType;
    public List<FMessageModule> MessageModules;
    public List<FDropItem> dropItems;
    public List<FCommanding> commandings;
    public List<FExplodeModule> ExplodeModules;
    public List<FEffectGivingModule> effectGivingModules;
    public List<FAudioModule> AudioModules;
    public List<FCGNModule> GroovieNoiseToCall;
    public List<FCFEModule> FunctionToCall;
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

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-Objects.json"), JsonConvert.SerializeObject(schematic.transform.GetComponentsInChildren<InteractableObject>().Where(x => !x.UseScriptValue).Select(x => x.data), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-FObjects.json"), JsonConvert.SerializeObject(schematic.transform.GetComponentsInChildren<InteractableObject>().Where(x => x.UseScriptValue).Select(x => x.ScriptValueData).ToList(), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto }).Replace("Assembly-CSharp", "AdvancedMERTools"));
        Debug.Log("Successfully Imported Interactable Objects.");
    }
}
