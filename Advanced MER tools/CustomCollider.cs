using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CustomCollider : FakeMono
{
    public new CCDTO data = new CCDTO();
    public new FCCDTO ScriptValueData = new FCCDTO();
    public override DTO _data { get => data; }
    public override DTO _ScriptValueData { get => ScriptValueData; }
}

[Serializable]
public class CCDTO : DTO
{
    public override void OnValidate()
    {
        AnimationModules.ForEach(x => x.AnimatorAdress = PublicFunctions.FindPath(x.Animator));
    }

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
    public List<CFEModule> FunctionToCall;
}

[Serializable]
public class FCCDTO : DTO
{
    public override void OnValidate()
    {
        CollisionType.OnValidate();
        DetectType.OnValidate();
        ModifyHealthAmount.OnValidate();
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

    public ColliderActionType ColliderActionType;
    public ScriptValue CollisionType;
    public ScriptValue DetectType;
    public ScriptValue ModifyHealthAmount;
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

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-Colliders.json"), JsonConvert.SerializeObject(schematic.transform.GetComponentsInChildren<CustomCollider>().Where(x => !x.UseScriptValue).Select(x => x.data), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-FColliders.json"), JsonConvert.SerializeObject(schematic.transform.GetComponentsInChildren<CustomCollider>().Where(x => x.UseScriptValue).Select(x => x.ScriptValueData).ToList(), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto }).Replace("Assembly-CSharp", "AdvancedMERTools"));
        Debug.Log("Successfully Imported Custom Colliders.");
    }
}