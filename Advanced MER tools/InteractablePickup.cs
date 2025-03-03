using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
//using NaughtyAttributes;
using System;
using System.IO;
using UnityEditor;
using Newtonsoft.Json;
using System.Linq;

public class InteractablePickup : FakeMono
{
    public new IPDTO data = new IPDTO();
    public new FIPDTO ScriptValueData = new FIPDTO();
    public override DTO _data { get => data; }
    public override DTO _ScriptValueData { get => ScriptValueData; }
}

[Serializable]
public class IPDTO : DTO
{
    public override void OnValidate()
    {
        AnimationModules.ForEach(x => x.AnimatorAdress = PublicFunctions.FindPath(x.Animator));
    }

    public InvokeType InvokeType;
    public IPActionType ActionType;
    public bool CancelActionWhenActive;
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
public class FIPDTO : DTO
{
    public override void OnValidate()
    {
        CancelActionWhenActive.OnValidate();
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

    public InvokeType InvokeType;
    public IPActionType ActionType;
    public ScriptValue CancelActionWhenActive;
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

public class InteractablePickupCompiler
{
    private static readonly Config Config = SchematicManager.Config;

    [MenuItem("SchematicManager/Compile Interactable pickups", priority = -11)]
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

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-Pickups.json"), JsonConvert.SerializeObject(schematic.transform.GetComponentsInChildren<InteractablePickup>().Where(x => !x.UseScriptValue).Select(x => x.data), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-FPickups.json"), JsonConvert.SerializeObject(schematic.transform.GetComponentsInChildren<InteractablePickup>().Where(x => x.UseScriptValue).Select(x => x.ScriptValueData).ToList(), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto }).Replace("Assembly-CSharp", "AdvancedMERTools"));
        Debug.Log("Successfully Imported Interactable Pickups.");
    }
}