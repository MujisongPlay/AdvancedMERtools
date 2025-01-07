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
    public HODTO data = new HODTO();
    public string[] formats = "DO NOT TOUCH THE VALUES.\n{attacker_i} = attacker's player id.\n{attacker_name}\n{a_pos} = attacker's position.\n{a_room} = attacker's room\n{a_zone} = attacker's zone\n{a_role} = attacker's role\n{s_pos} = schematic's exact position.\n{s_room} = schematic's exact room.\n{s_zone} = schematic's zone.\n{a_item} = attacker's current item.\n{damage}".Split('\n');
}

[Serializable]
public class HODTO : DTO
{
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

        List<HODTO> healthObjects = new List<HODTO> { };

        foreach (HealthObject health in schematic.transform.GetComponentsInChildren<HealthObject>())
        {
            health.data.AnimationModules.ForEach(x => 
            {
                x.AnimatorAdress = PublicFunctions.FindPath(x.Animator.transform);
            });
            health.data.ObjectId = PublicFunctions.FindPath(health.transform);
            health.data.Code = health.GetInstanceID();
            healthObjects.Add(health.data);
        }

        string serializedData = JsonConvert.SerializeObject(healthObjects, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-HealthObjects.json"), serializedData);
        Debug.Log("Successfully Imported HealthObjects.");
    }
}
