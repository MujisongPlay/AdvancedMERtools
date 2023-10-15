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
    public ActionType ActionType;
    [BoxGroup("Animation")]
    [ShowIf("ActionType", ActionType.PlayAnimation)]
    public GameObject Animator = null;
    [BoxGroup("Animation")]
    [ShowIf("ActionType", ActionType.PlayAnimation)]
    public string AnimationName = "";
    [BoxGroup("Explode")]
    [ShowIf("ActionType", ActionType.Explode)]
    public bool ExplosionFriendlyKill = false;
    [BoxGroup("Animation")]
    [ShowIf("ActionType", ActionType.PlayAnimation)]
    public AnimationType AnimationType = AnimationType.Start;
    [BoxGroup("Warhead")]
    [ShowIf("ActionType", ActionType.Warhead)]
    public WarheadActionType warheadAction = WarheadActionType.Start;
    [BoxGroup("Message")]
    [ShowIf("ActionType", ActionType.SendMessage)]
    public MessageType MessageType = MessageType.BroadCast;
    [BoxGroup("Message")]
    [ShowIf("ActionType", ActionType.SendMessage)]
    [Tooltip("{picker_i} = picker's player id.\n{picker_name}\n{a_pos} = picker's position.\n{a_room} = picker's room\n{a_zone} = picker's zone\n{a_role} = picker's role\n{s_pos} = item's exact position.\n{s_room} = item's exact room.\n{s_zone} = item's zone.\n{a_item} = picker's current item.")]
    public string MessageContent = "";
    [BoxGroup("Message")]
    [ShowIf("MessageShowCheck")]
    public SendType SendType = SendType.Killer;
    [BoxGroup("Drop items")]
    [ShowIf("ActionType", ActionType.DropItems)]
    [ReorderableList]
    public List<DropItem> DropItems = new List<DropItem> { };
    [BoxGroup("Command")]
    [ShowIf("ActionType", ActionType.SendCommand)]
    [ReorderableList]
    [Label("There's many formats you can see when you put on curser to 'command context'")]
    public List<Commanding> Commandings = new List<Commanding> { };
    [BoxGroup("914Upgrade")]
    [ShowIf("ActionType", ActionType.UpgradeItem)]
    public Scp914Mode Setting;

    public bool MessageShowCheck()
    {
        return ActionType.HasFlag(ActionType.SendMessage) && MessageType != MessageType.Cassie;
    }
}

[Serializable]
public class IPDTO
{
    public ActionType ActionType;
    public string ObjectId;
    public string Animator;
    public string AnimationName;
    public AnimationType AnimationType;
    public WarheadActionType warheadActionType;
    public string MessageContent;
    public MessageType MessageType;
    public SendType SendType;
    public List<DropItem> dropItems;
    public List<Commanding> commandings;
    public Scp914Mode Scp914Mode;
    public bool FFon;
}

[Flags]
[Serializable]
public enum Scp914Mode
{
    Rough = 0,
    Coarse = 1,
    OneToOne = 2,
    Fine = 3,
    VeryFine = 4
}

[Flags]
[Serializable]
public enum ActionType
{
    Disappear = 1,
    Explode = 2,
    PlayAnimation = 4,
    Warhead = 8,
    SendMessage = 16,
    DropItems = 32,
    SendCommand = 64,
    UpgradeItem = 128
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
            if (!ip.transform.TryGetComponent<PickupComponent>(out _) || ip.ActionType == 0)
            {
                continue;
            }
            IPDTO DTO = new IPDTO
            {
                ObjectId = FindPath(ip.transform),
                ActionType = ip.ActionType
            };
            foreach (ActionType type in Enum.GetValues(typeof(ActionType)))
            {
                if (ip.ActionType.HasFlag(type))
                {
                    switch (type)
                    {
                        case ActionType.Explode:
                            DTO.FFon = ip.ExplosionFriendlyKill;
                            break;
                        case ActionType.DropItems:
                            DTO.dropItems = ip.DropItems;
                            break;
                        case ActionType.PlayAnimation:
                            if (ip.Animator == null)
                                break;
                            DTO.AnimationName = ip.AnimationName;
                            DTO.AnimationType = ip.AnimationType;
                            DTO.Animator = FindPath(ip.Animator.transform);
                            break;
                        case ActionType.Warhead:
                            DTO.warheadActionType = ip.warheadAction;
                            break;
                        case ActionType.SendMessage:
                            DTO.SendType = ip.SendType;
                            DTO.MessageContent = ip.MessageContent;
                            DTO.MessageType = ip.MessageType;
                            break;
                        case ActionType.SendCommand:
                            DTO.commandings = ip.Commandings;
                            break;
                        case ActionType.UpgradeItem:
                            DTO.Scp914Mode = ip.Setting;
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

    public static string FindPath(Transform transform)
    {
        string path = "";
        if (transform.TryGetComponent<Schematic>(out _))
        {
            return path;
        }
        while (true)
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i) == transform)
                {
                    path += i.ToString();
                }
            }
            transform = transform.parent;
            if (transform.TryGetComponent<Schematic>(out _)) break;
            path += " ";
        }
        return path;
    }
}
