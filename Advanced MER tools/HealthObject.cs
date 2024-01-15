using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;


public class HealthObject : MonoBehaviour
{
    public float Health;
    public int ArmorEfficient;
    public DeadType DeadType;
    public float DeadActionDelay;
    [ShowIf("DeadType", DeadType.Explode)]
    [ReorderableList]
    public List<ExplodeModule> ExplodeModules;
    [ShowIf("KillCheck")]
    public bool DoNotRemoveAfterDeath;
    [ShowIf("DeadType", DeadType.PlayAnimation)]
    [ReorderableList]
    public List<AnimationModule> AnimationModules;
    [ShowIf("DeadType", DeadType.Warhead)]
    public WarheadActionType warheadAction;
    [ShowIf("DeadType", DeadType.ResetHP)]
    public float ResetHPTo;
    [ReorderableList]
    public List<WhitelistWeapon> whitelistWeapons;
    [ShowIf("DeadType", DeadType.SendMessage)]
    [Tooltip("{attacker_i} = attacker's player id.\n{attacker_name}\n{a_pos} = attacker's position.\n{a_room} = attacker's room\n{a_zone} = attacker's zone\n{a_role} = attacker's role\n{s_pos} = schematic's exact position.\n{s_room} = schematic's exact room.\n{s_zone} = schematic's zone.\n{a_item} = attacker's current item.\n{damage}")]
    [ReorderableList]
    public List<MessageModule> MessageModules;
    [ShowIf("DeadType", DeadType.DropItems)]
    [ReorderableList]
    public List<DropItem> DropItems;
    [ShowIf("DeadType", DeadType.SendCommand)]
    [ReorderableList]
    [Label("There's many formats you can see when you put on curser to 'command context'")]
    public List<Commanding> Commandings;
    [ShowIf("DeadType", DeadType.GiveEffect)]
    [ReorderableList]
    public List<EffectGivingModule> effectGivingModules;

    public bool KillCheck()
    {
        return DeadType == DeadType.Disappear || DeadType == DeadType.Explode || DeadType == DeadType.DynamicDisappearing || DeadType == DeadType.SendCommand;
    }
}

[Serializable]
public class HODTO
{
    public float Health;
    public int ArmorEfficient;
    public DeadType DeadType;
    public float DeadDelay;
    public float ResetHPTo;
    public string ObjectId;
    public List<AnimationDTO> animationDTOs;
    public List<WhitelistWeapon> whitelistWeapons;
    public WarheadActionType warheadActionType;
    public List<MessageModule> messageModules;
    public List<DropItem> dropItems;
    public bool DoNotDestroyAfterDeath;
    public List<Commanding> commandings;
    public List<ExplodeModule> ExplodeModules;
    public List<EffectGivingModule> effectGivingModules;
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
            HODTO dTO = new HODTO
            {
                Health = health.Health,
                ArmorEfficient = health.ArmorEfficient,
                DeadType = health.DeadType,
                ObjectId = PublicFunctions.FindPath(health.transform),
                whitelistWeapons = health.whitelistWeapons,
                DeadDelay = health.DeadActionDelay,
                DoNotDestroyAfterDeath = health.DoNotRemoveAfterDeath
            };
            foreach (DeadType type in Enum.GetValues(typeof(DeadType)))
            {
                if (dTO.DeadType.HasFlag(type))
                {
                    switch (type)
                    {
                        case DeadType.Explode:
                            dTO.ExplodeModules = health.ExplodeModules;
                            break;
                        case DeadType.PlayAnimation:
                            dTO.animationDTOs = new List<AnimationDTO> { };
                            foreach (AnimationModule module in health.AnimationModules)
                            {
                                dTO.animationDTOs.Add(new AnimationDTO
                                {
                                    Animator = PublicFunctions.FindPath(module.Animator.transform),
                                    Animation = module.AnimationName,
                                    AnimationType = module.AnimationType,
                                    ForceExecute = module.ForceExecute,
                                    ChanceWeight = module.ChanceWeight
                                });
                            }
                            break;
                        case DeadType.Warhead:
                            dTO.warheadActionType = health.warheadAction;
                            break;
                        case DeadType.ResetHP:
                            dTO.ResetHPTo = health.ResetHPTo;
                            break;
                        case DeadType.DropItems:
                            dTO.dropItems = health.DropItems;
                            break;
                        case DeadType.SendMessage:
                            dTO.messageModules = health.MessageModules;
                            break;
                        case DeadType.SendCommand:
                            dTO.commandings = health.Commandings;
                            break;
                        case DeadType.GiveEffect:
                            dTO.effectGivingModules = health.effectGivingModules;
                            break;
                    }
                }
            }

            healthObjects.Add(dTO);
        }

        string serializedData = JsonConvert.SerializeObject(healthObjects, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-HealthObjects.json"), serializedData);
        Debug.Log("Successfully Imported HealthObjects.");
    }
}
