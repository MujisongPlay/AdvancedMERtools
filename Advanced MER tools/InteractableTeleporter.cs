using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using NaughtyAttributes;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;
using System;

public class InteractableTeleporter : MonoBehaviour
{
    [Tooltip("Index means the index of teleporter that this script will be applied to, in loaded map. It starts from 1.")]
    public int index;
    public TeleportInvokeType TeleportInvokeType;
    public IPActionType ActionType;
    [ShowIf("ActionType", IPActionType.PlayAnimation)]
    [ReorderableList]
    public List<AnimationModule> AnimationModules;
    [ShowIf("ActionType", IPActionType.Explode)]
    [ReorderableList]
    public List<ExplodeModule> ExplodeModules;
    [ShowIf("ActionType", IPActionType.Warhead)]
    public WarheadActionType warheadAction;
    [ShowIf("ActionType", IPActionType.SendMessage)]
    [ReorderableList]
    public List<MessageModule> MessageModules;
    //[ShowIf("ActionType", IPActionType.DropItems)]
    //[ReorderableList]
    //public List<DropItem> DropItems;
    [ShowIf("ActionType", IPActionType.SendCommand)]
    [ReorderableList]
    [Label("There's many formats you can see when you put on curser to 'command context'")]
    public List<Commanding> Commandings;
    [ShowIf("ActionType", IPActionType.GiveEffect)]
    [ReorderableList]
    public List<EffectGivingModule> effectGivingModules;

    private void OnValidate()
    {
        bool flag = false;
        if (ActionType.HasFlag(IPActionType.UpgradeItem))
        {
            flag = true;
            ActionType -= IPActionType.UpgradeItem;
        }
        //if (ActionType.HasFlag(IPActionType.PlayAnimation))
        //{
        //    flag = true;
        //    ActionType -= IPActionType.PlayAnimation;
        //}
        if (ActionType.HasFlag(IPActionType.DropItems))
        {
            flag = true;
            ActionType -= IPActionType.DropItems;
        }
        if (flag)
            Debug.Log("You cannot use that option in InteractableTeleporters!");
    }
}

[Serializable]
public class ITDTO
{
    public TeleportInvokeType InvokeType;
    public IPActionType ActionType;
    public string ObjectId;
    public List<AnimationDTO> animationDTOs;
    public WarheadActionType warheadActionType;
    public List<MessageModule> MessageModules;
    //public List<DropItem> dropItems;
    public List<Commanding> commandings;
    public List<ExplodeModule> ExplodeModules;
    public List<EffectGivingModule> effectGivingModules;
}

public class InteractableTeleporterCompiler
{
    private static readonly Config Config = SchematicManager.Config;

    [MenuItem("SchematicManager/Compile Interactable teleporter", priority = -11)]
    public static void OnCompile()
    {
        string parentDirectoryPath = Directory.Exists(Config.ExportPath)
            ? Config.ExportPath
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "MapEditorReborn_CompiledSchematics");
        if (!Directory.Exists(parentDirectoryPath))
        {
            Debug.LogError("Could not find schematic output!");
            return;
        }

        List<ITDTO> interactables = new List<ITDTO> { };

        foreach (InteractableTeleporter ip in GameObject.FindObjectsOfType<InteractableTeleporter>())
        {
            ITDTO DTO = new ITDTO
            {
                ObjectId = ip.index.ToString(),
                ActionType = ip.ActionType,
                InvokeType = ip.TeleportInvokeType
            };
            foreach (IPActionType type in Enum.GetValues(typeof(IPActionType)))
            {
                if (ip.ActionType.HasFlag(type))
                {
                    switch (type)
                    {
                        case IPActionType.Explode:
                            DTO.ExplodeModules = ip.ExplodeModules;
                            break;
                        //case IPActionType.DropItems:
                        //    DTO.dropItems = ip.DropItems;
                        //    break;
                        case IPActionType.PlayAnimation:
                            DTO.animationDTOs = new List<AnimationDTO> { };
                            foreach (AnimationModule module in ip.AnimationModules)
                            {
                                DTO.animationDTOs.Add(new AnimationDTO
                                {
                                    Animator = PublicFunctions.FindPath(module.Animator.transform),
                                    Animation = module.AnimationName,
                                    AnimationType = module.AnimationType,
                                    ForceExecute = module.ForceExecute,
                                    ChanceWeight = module.ChanceWeight
                                });
                            }
                            break;
                        case IPActionType.Warhead:
                            DTO.warheadActionType = ip.warheadAction;
                            break;
                        case IPActionType.SendMessage:
                            DTO.MessageModules = ip.MessageModules;
                            break;
                        case IPActionType.SendCommand:
                            DTO.commandings = ip.Commandings;
                            break;
                        case IPActionType.GiveEffect:
                            DTO.effectGivingModules = ip.effectGivingModules;
                            break;
                        default:
                            break;
                    }
                }
            }

            interactables.Add(DTO);
        }

        string serializedData = JsonConvert.SerializeObject(interactables, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        File.WriteAllText(Path.Combine(parentDirectoryPath, "{map name here}-ITeleporters.json"), serializedData);
        Debug.Log("Successfully Imported Interactable Teleporters.");
    }

    /*public static void Compile(Schematic schematic)
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

        List<ITDTO> interactables = new List<ITDTO> { };

        foreach (InteractableTeleporter ip in schematic.transform.GetComponentsInChildren<InteractableTeleporter>())
        {
            if (!ip.transform.TryGetComponent<TeleportComponent>(out _))
            {
                continue;
            }
            ITDTO DTO = new ITDTO
            {
                ObjectId = PublicFunctions.FindPath(ip.transform),
                ActionType = ip.ActionType,
                InvokeType = ip.TeleportInvokeType
            };
            foreach (IPActionType type in Enum.GetValues(typeof(IPActionType)))
            {
                if (ip.ActionType.HasFlag(type))
                {
                    switch (type)
                    {
                        case IPActionType.Explode:
                            DTO.ExplodeModules = ip.ExplodeModules;
                            break;
                        case IPActionType.DropItems:
                            DTO.dropItems = ip.DropItems;
                            break;
                        case IPActionType.PlayAnimation:
                            DTO.animationDTOs = new List<AnimationDTO> { };
                            foreach (AnimationModule module in ip.AnimationModules)
                            {
                                DTO.animationDTOs.Add(new AnimationDTO
                                {
                                    Animator = PublicFunctions.FindPath(module.Animator.transform),
                                    Animation = module.AnimationName,
                                    AnimationType = module.AnimationType,
                                    ForceExecute = module.ForceExecute,
                                    ChanceWeight = module.ChanceWeight
                                });
                            }
                            break;
                        case IPActionType.Warhead:
                            DTO.warheadActionType = ip.warheadAction;
                            break;
                        case IPActionType.SendMessage:
                            DTO.MessageModules = ip.MessageModules;
                            break;
                        case IPActionType.SendCommand:
                            DTO.commandings = ip.Commandings;
                            break;
                        case IPActionType.GiveEffect:
                            DTO.effectGivingModules = ip.effectGivingModules;
                            break;
                        default:
                            break;
                    }
                }
            }

            interactables.Add(DTO);
        }

        string serializedData = JsonConvert.SerializeObject(interactables, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-ITeleporters.json"), serializedData);
        Debug.Log("Successfully Imported Interactable Teleporters.");
    }*/
}
