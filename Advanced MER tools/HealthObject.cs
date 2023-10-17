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
    public float Health = 0;
    public int ArmorEfficient = 0;
    public DeadType DeadType = DeadType.Disappear;
    public float DeadActionDelay = 0f;
    [BoxGroup("Explode")]
    [ShowIf("DeadType", DeadType.Explode)]
    public bool ExplosionFriendlyKill = false;
    [ShowIf("KillCheck")]
    public bool DoNotRemoveAfterDeath = false;
    [BoxGroup("Animation")] [ShowIf("DeadType", DeadType.PlayAnimation)]
    public GameObject Animator = null;
    [BoxGroup("Animation")] [ShowIf("DeadType", DeadType.PlayAnimation)]
    public string AnimationName = "";
    [BoxGroup("Animation")]
    [ShowIf("DeadType", DeadType.PlayAnimation)]
    public AnimationType AnimationType = AnimationType.Start;
    [BoxGroup("Warhead")]
    [ShowIf("DeadType", DeadType.Warhead)]
    public WarheadActionType warheadAction = WarheadActionType.Start;
    [BoxGroup("ResetHP")]
    [ShowIf("DeadType", DeadType.ResetHP)]
    public float ResetHPTo = 0f;
    [ReorderableList]
    public List<WhitelistWeapon> whitelistWeapons = new List<WhitelistWeapon> { };
    [BoxGroup("Message")]
    [ShowIf("DeadType", DeadType.SendMessage)]
    public MessageType MessageType = MessageType.BroadCast;
    [BoxGroup("Message")]
    [ShowIf("DeadType", DeadType.SendMessage)]
    [Tooltip("{attacker_i} = attacker's player id.\n{attacker_name}\n{a_pos} = attacker's position.\n{a_room} = attacker's room\n{a_zone} = attacker's zone\n{a_role} = attacker's role\n{s_pos} = schematic's exact position.\n{s_room} = schematic's exact room.\n{s_zone} = schematic's zone.\n{a_item} = attacker's current item.\n{damage}")]
    public string MessageContent = "";
    [BoxGroup("Message")]
    [ShowIf("MessageShowCheck")]
    public SendType SendType = SendType.Killer;
    [BoxGroup("Drop items")]
    [ShowIf("DeadType", DeadType.DropItems)]
    [ReorderableList]
    public List<DropItem> DropItems = new List<DropItem> { };
    [BoxGroup("Command")]
    [ShowIf("DeadType", DeadType.SendCommand)]
    [ReorderableList]
    [Label("There's many formats you can see when you put on curser to 'command context'")]
    public List<Commanding> Commandings = new List<Commanding> { };

    public bool KillCheck()
    {
        return DeadType == DeadType.Disappear || DeadType == DeadType.Explode || DeadType == DeadType.DynamicDisappearing || DeadType == DeadType.SendCommand;
    }

    public bool MessageShowCheck()
    {
        return DeadType == DeadType.SendMessage && MessageType != MessageType.Cassie;
    }
}

[Serializable]
public class WhitelistWeapon
{
    public ItemType ItemType;
    public uint CustomItemId;
}

[Serializable]
public class HealthObjectDTO
{
    public float Health;
    public int ArmorEfficient;
    public DeadType DeadType;
    public float DeadDelay;
    public float ResetHPTo;
    public string ObjectId;
    public string Animator;
    public string AnimationName;
    public AnimationType AnimationType;
    public List<WhitelistWeapon> whitelistWeapons;
    public WarheadActionType warheadActionType;
    public string MessageContent;
    public MessageType MessageType;
    public SendType SendType;
    public List<DropItem> dropItems;
    public bool DoNotDestroyAfterDeath;
    public List<Commanding> commandings;
    public bool FFon;
}

[Serializable]
public enum DeadType
{
    Disappear,
    GetRigidbody,
    DynamicDisappearing,
    Explode,
    ResetHP,
    PlayAnimation,
    Warhead,
    SendMessage,
    DropItems,
    SendCommand
}

[Serializable]
public enum WarheadActionType
{
    Start,
    Stop,
    Lock,
    UnLock,
    Disable,
    Enable
}

[Serializable]
public enum AnimationType
{
    Start,
    Stop
}

[Serializable]
public enum MessageType
{
    Cassie,
    BroadCast,
    Hint
}

[Serializable]
public enum SendType
{
    Killer,
    All
}

[Serializable]
public class DropItem
{
    public ItemType ItemType;
    public uint CustomItemId;
    public int Count;
    public float Chance;
    public bool ForceSpawn;
}

[Serializable]
public class Commanding
{
    [Tooltip("{attacker_i} = attacker's player id.\n{attacker_name}\n{a_pos} = attacker's position.\n{a_room} = attacker's room\n{a_zone} = attacker's zone\n{a_role} = attacker's role\n{s_pos} = schematic's exact position.\n{s_room} = schematic's exact room.\n{s_zone} = schematic's zone.\n{a_item} = attacker's current item.\n{damage}")]
    public string CommandContext;
    public float Chance;
    public bool ForceExecute;
    public CommandType CommandType;
    public ExecutorType ExecutorType;
}

[Serializable]
public enum CommandType
{
    RemoteAdmin,
    ClientConsole
}

[Serializable]
public enum ExecutorType
{
    Attacker,
    LocalAdmin
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

        List<HealthObjectDTO> healthObjects = new List<HealthObjectDTO> { };

        foreach (HealthObject health in schematic.transform.GetComponentsInChildren<HealthObject>())
        {
            HealthObjectDTO dTO = new HealthObjectDTO
            {
                Health = health.Health,
                ArmorEfficient = health.ArmorEfficient,
                DeadType = health.DeadType,
                ObjectId = FindPath(health.transform),
                whitelistWeapons = health.whitelistWeapons,
                DeadDelay = health.DeadActionDelay,
                DoNotDestroyAfterDeath = health.DoNotRemoveAfterDeath
            };
            switch (health.DeadType)
            {
                case DeadType.Explode:
                    dTO.FFon = health.ExplosionFriendlyKill;
                    break;
                case DeadType.PlayAnimation:
                    dTO.Animator = FindPath(health.Animator.transform);
                    dTO.AnimationName = health.AnimationName;
                    dTO.AnimationType = health.AnimationType;
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
                    dTO.MessageType = health.MessageType;
                    dTO.MessageContent = health.MessageContent;
                    dTO.SendType = health.SendType;
                    break;
                case DeadType.SendCommand:
                    dTO.commandings = health.Commandings;
                    break;
            }
            healthObjects.Add(dTO);
        }

        string serializedData = JsonConvert.SerializeObject(healthObjects, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-HealthObjects.json"), serializedData);
        Debug.Log("Successfully Imported HealthObjects.");
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
