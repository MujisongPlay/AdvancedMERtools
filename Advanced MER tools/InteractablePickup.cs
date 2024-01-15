using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using System.IO;
using UnityEditor;
using Newtonsoft.Json;

public class InteractablePickup : MonoBehaviour
{
    public InvokeType InvokeType;
    public IPActionType ActionType;
    public bool CancelActionWhenActive;
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
    [ShowIf("ActionType", IPActionType.DropItems)]
    [ReorderableList]
    public List<DropItem> DropItems;
    [ShowIf("ActionType", IPActionType.SendCommand)]
    [ReorderableList]
    [Label("There's many formats you can see when you put on curser to 'command context'")]
    public List<Commanding> Commandings;
    [ShowIf("ActionType", IPActionType.UpgradeItem)]
    public Scp914Mode Setting;
    [ShowIf("ActionType", IPActionType.GiveEffect)]
    [ReorderableList]
    public List<EffectGivingModule> effectGivingModules;
}

[Serializable]
public class IPDTO
{
    public InvokeType InvokeType;
    public IPActionType ActionType;
    public bool CancelActionWhenActive;
    public string ObjectId;
    public List<AnimationDTO> animationDTOs;
    public WarheadActionType warheadActionType;
    public List<MessageModule> MessageModules;
    public List<DropItem> dropItems;
    public List<Commanding> commandings;
    public Scp914Mode Scp914Mode;
    public List<ExplodeModule> ExplodeModules;
    public List<EffectGivingModule> effectGivingModules;
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

        List<IPDTO> interactables = new List<IPDTO> { };

        foreach (InteractablePickup ip in schematic.transform.GetComponentsInChildren<InteractablePickup>())
        {
            if (!ip.transform.TryGetComponent<PickupComponent>(out _))
            {
                continue;
            }
            IPDTO DTO = new IPDTO
            {
                ObjectId = PublicFunctions.FindPath(ip.transform),
                ActionType = ip.ActionType,
                InvokeType = ip.InvokeType,
                CancelActionWhenActive = ip.CancelActionWhenActive
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
                        case IPActionType.UpgradeItem:
                            DTO.Scp914Mode = ip.Setting;
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

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-Pickups.json"), serializedData);
        Debug.Log("Successfully Imported Interactable Pickups.");
    }
}
