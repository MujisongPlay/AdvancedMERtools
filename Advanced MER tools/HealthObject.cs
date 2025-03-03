using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class HealthObject : FakeMono
{
    public new HODTO data = new();
    public new FHODTO ScriptValueData = new();
    public override DTO _data { get => data; }
    public override DTO _ScriptValueData { get => ScriptValueData; }
    public string[] formats = "DO NOT TOUCH THE VALUES.\n{attacker_i} = attacker's player id.\n{attacker_name}\n{a_pos} = attacker's position.\n{a_room} = attacker's room\n{a_zone} = attacker's zone\n{a_role} = attacker's role\n{s_pos} = schematic's exact position.\n{s_room} = schematic's exact room.\n{s_zone} = schematic's zone.\n{a_item} = attacker's current item.\n{damage}".Split('\n');
}

[Serializable]
public class HODTO : DTO
{
    public override void OnValidate()
    {
        AnimationModules.ForEach(x => x.AnimatorAdress = PublicFunctions.FindPath(x.Animator));
    }

    public float Health;
    [Range(0, 100)]
    public int ArmorEfficient;
    public DeadType DeadType;
    public float DeadActionDelay;
    public float ResetHPTo;
    public bool DoNotDestroyAfterDeath;
    public List<WhitelistWeapon> whitelistWeapons;
    public List<AnimationDTO> AnimationModules;
    public WarheadActionType warheadAction;
    public List<MessageModule> MessageModules;
    public List<DropItem> DropItems;
    public List<Commanding> Commandings;
    public List<ExplodeModule> ExplodeModules;
    public List<EffectGivingModule> effectGivingModules;
    public List<AudioModule> AudioModules;
    public List<CGNModule> GroovieNoiseToCall;
    public List<CFEModule> FunctionToCall;
}

[Serializable]
public class FHODTO : DTO
{
    public override void OnValidate()
    {
        Health.OnValidate();
        ArmorEfficient.OnValidate();
        DeadActionDelay.OnValidate();
        ResetHPTo.OnValidate();
        DoNotDestroyAfterDeath.OnValidate();
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

    public ScriptValue Health;
    [Range(0, 100)]
    public ScriptValue ArmorEfficient;
    public DeadType DeadType;
    public ScriptValue DeadActionDelay;
    public ScriptValue ResetHPTo;
    public ScriptValue DoNotDestroyAfterDeath;
    public List<FWhitelistWeapon> whitelistWeapons;
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

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-HealthObjects.json"), JsonConvert.SerializeObject(schematic.transform.GetComponentsInChildren<HealthObject>().Where(x => !x.UseScriptValue).Select(x => x.data), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-FHealthObjects.json"), JsonConvert.SerializeObject(schematic.transform.GetComponentsInChildren<HealthObject>().Where(x => x.UseScriptValue).Select(x => x.ScriptValueData), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto }).Replace("Assembly-CSharp", "AdvancedMERTools"));
        Debug.Log("Successfully Imported HealthObjects.");
    }
}