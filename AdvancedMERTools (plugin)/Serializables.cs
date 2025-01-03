using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using UnityEngine;
using MapEditorReborn.API.Features.Objects;
using Utils;
using Footprinting;
using Exiled.CustomItems.API.Features;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Mirror;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;

namespace AdvancedMERTools
{
    [Serializable]
    public class HODTO : AMERTDTO
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
    }

    [Serializable]
    public class ITDTO : AMERTDTO
    {
        public TeleportInvokeType InvokeType;
        public IPActionType ActionType;
        public List<AnimationDTO> animationDTOs;
        public WarheadActionType warheadActionType;
        public List<MessageModule> MessageModules;
        public List<Commanding> commandings;
        public List<ExplodeModule> ExplodeModules;
        public List<EffectGivingModule> effectGivingModules;
    }

    [Serializable]
    public class IPDTO : AMERTDTO
    {
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
    }

    [Serializable]
    public class CCDTO : AMERTDTO
    {
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
    }

    [Serializable]
    public class IODTO : AMERTDTO
    {
        public KeyCode KeyCode;
        public float MaxRange;
        public IPActionType ActionType;
        public List<AnimationDTO> AnimationModules;
        public WarheadActionType warheadActionType;
        public List<MessageModule> MessageModules;
        public List<DropItem> dropItems;
        public List<Commanding> commandings;
        public Scp914Mode Scp914Mode;
        public List<ExplodeModule> ExplodeModules;
        public List<EffectGivingModule> effectGivingModules;
    }

    [Serializable]
    public class GNDTO : AMERTDTO
    {
        public List<GMDTO> GMDTOs;
    }

    [Serializable]
    public class CDDTO : AMERTDTO
    {
        public DoorType DoorType;
        public string animator;
        public string OpenAnimation;
        public string CloseAnimation;
        public string LockAnimation;
        public string UnlockAnimation;
        public string BrokenAnimation;
        public Vector3 DoorInstallPos;
        public Vector3 DoorInstallRot;
        public Vector3 DoorInstallScl;
        public KeycardPermissions DoorPermissions;
        public float DoorHealth;
        public Interactables.Interobjects.DoorUtils.DoorDamageType DoorDamageType;
    }

    [Serializable]
    public class DGDTO
    {
        public float Health;
        public Interactables.Interobjects.DoorUtils.DoorDamageType DamagableDamageType;
        public KeycardPermissions KeycardPermissions;
        public string ObjectId;
    }

    [Serializable]
    public class GMDTO : RandomExecutionModule
    {
        public List<int> codes;
        public bool Enable;

        public override void Execute(params object[] args)
        {
            List<GMDTO> gs = args[0] as List<GMDTO>;
            foreach (GMDTO mDTO in gs)
            {
                MEC.Timing.CallDelayed(mDTO.ActionDelay, () => 
                {
                    mDTO.codes.ForEach(x => AdvancedMERTools.Singleton.codeClassPair[x].Active = mDTO.Enable);
                });
            }
        }
    }

    [Serializable]
    public class AMERTDTO
    {
        public bool Active;
        public string ObjectId;
        public int Code;
    }

    [Serializable]
    public class AMERTInteractable : NetworkBehaviour
    {
        void Start()
        {
            Active = Base.Active;
        }

        public AMERTDTO Base;
        public bool Active;
    }

    [Flags]
    [Serializable]
    public enum ColliderActionType
    {
        ModifyHealth = 1,
        GiveEffect = 2,
        SendMessage = 4,
        PlayAnimation = 8,
        SendCommand = 16,
        Warhead = 32,
        Explode = 64
    }

    [Flags]
    [Serializable]
    public enum CollisionType
    {
        OnEnter = 1,
        OnStay = 2,
        OnExit = 4
    }

    [Flags]
    [Serializable]
    public enum DetectType
    {
        Pickup = 1,
        Player = 2,
        Projectile = 4
    }

    namespace AMERT
    {
        [Serializable]
        public enum DoorType : int
        {
            LCZ = 0,
            HCZ = 1,
            EZ = 2
        }
    }

    [Serializable]
    public class EffectGivingModule : RandomExecutionModule
    {
        public EffectFlag EffectFlag;
        public EffectType effectType;
        public SendType GivingTo;
        public byte Inensity;
        public float Duration;

        public override void Execute(params object[] args)
        {
            List<EffectGivingModule> modules = args[0] as List<EffectGivingModule>;
            Player hub = args[1] as Player;

            foreach (EffectGivingModule module in modules)
            {
                MEC.Timing.CallDelayed(module.ActionDelay, () => 
                {
                    List<Player> list = new List<Player> { };
                    if (module.GivingTo.HasFlag(SendType.Interactor))
                        list.Add(hub);
                    if (module.GivingTo.HasFlag(SendType.AllExceptAboveOne))
                        list.AddRange(Player.List.Where(x => x != hub));
                    if (module.GivingTo.HasFlag(SendType.Alive))
                        list.AddRange(Player.List.Where(x => x.IsAlive));
                    if (module.GivingTo.HasFlag(SendType.Spectators))
                        list.AddRange(Player.List.Where(x => !x.IsAlive));
                    list = Player.List.Where(x => list.Contains(x)).ToList();

                    foreach (Player player in list)
                    {
                        if (module.EffectFlag.HasFlag(EffectFlag.Disable))
                            player.DisableEffect(module.effectType);
                        else if (module.EffectFlag.HasFlag(EffectFlag.Enable))
                        {
                            byte intensity = (byte)((module.EffectFlag.HasFlag(EffectFlag.ModifyIntensity) ? player.GetEffect(module.effectType).Intensity : 0) + module.Inensity);
                            player.EnableEffect(module.effectType, intensity, module.Duration, module.EffectFlag.HasFlag(EffectFlag.ModifyDuration));
                        }
                    }
                });
            }
        }
    }

    [Serializable]
    public class ExplodeModule : RandomExecutionModule
    {
        public bool FFon;
        public bool EffectOnly;
        public SVector3 LocalPosition;

        public override void Execute(params object[] args)
        {
            List<ExplodeModule> modules = args[0] as List<ExplodeModule>;
            Transform transform = args[1] as Transform;
            ReferenceHub hub = args[2] as ReferenceHub;
            ReferenceHub.TryGetLocalHub(out ReferenceHub local);
            foreach (ExplodeModule module in modules)
            {
                MEC.Timing.CallDelayed(module.ActionDelay, () => 
                {
                    if (module.EffectOnly)
                        ExplosionUtils.ServerSpawnEffect(transform.TransformPoint(module.LocalPosition), ItemType.GrenadeHE);
                    else
                        ExplosionUtils.ServerExplode(transform.TransformPoint(module.LocalPosition), module.FFon ? new Footprint(local) : new Footprint(hub), ExplosionType.Grenade);
                });
            }
        }
    }

    [Serializable]
    public class MessageModule : RandomExecutionModule
    {
        public SendType SendType;
        public string MessageContent;
        public MessageType MessageType;
        public float Duration;

        /// <summary>
        /// messageModules, FuncDictionary, Interactor, transform (health)  
        /// '', '', '', pickup  
        /// '', '', '', teleporterT  
        /// '', '', '', Transform (CC)
        /// </summary>
        /// <param name="args"></param>
        public override void Execute(params object[] args)
        {
            List<MessageModule> messages = args[0] as List<MessageModule>;
            Dictionary<string, Func<object[], string>> pairs = args[1] as Dictionary<string, Func<object[], string>>;
            Player interactor = args[2] as Player;
            foreach (MessageModule module in messages)
            {
                MEC.Timing.CallDelayed(module.ActionDelay, () => 
                {
                    string Content = module.MessageContent;
                    foreach (string v in pairs.Keys)
                    {
                        try
                        {
                            Content = Content.Replace(v, pairs[v]((object[])args.Skip(2).ToArray()));
                        }
                        catch (Exception _) { }
                    }
                    try
                    {
                        Content = ServerConsole.singleton.NameFormatter.ProcessExpression(Content);
                    }
                    catch (Exception e) { }
                    if (module.MessageType == MessageType.Cassie)
                    {
                        Cassie.Message(Content);
                    }
                    else
                    {
                        List<Player> targets = new List<Player> { };
                        if (module.SendType.HasFlag(SendType.AllExceptAboveOne))
                            targets.AddRange(Player.List.Where(x => x != interactor));
                        if (module.SendType.HasFlag(SendType.Spectators))
                            targets.AddRange(Player.List.Where(x => !x.IsAlive));
                        if (module.SendType.HasFlag(SendType.Alive))
                            targets.AddRange(Player.List.Where(x => x.IsAlive));
                        if (module.SendType.HasFlag(SendType.Interactor))
                            targets.Add(interactor);

                        targets = Player.List.Where(x => targets.Contains(x)).ToList();

                        foreach (Player p in targets)
                        {
                            if (module.MessageType == MessageType.BroadCast)
                            {
                                p.Broadcast((ushort)Math.Round(module.Duration), Content);
                            }
                            else
                            {
                                p.ShowHint(Content, module.Duration);
                            }
                        }
                    }
                });
            }
        }
    }

    [Flags]
    [Serializable]
    public enum EffectFlag
    {
        Disable = 1,
        Enable = 2,
        ModifyDuration = 4,
        ForceDuration = 8,
        ModifyIntensity = 16,
        ForceIntensity = 32
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
    public enum TeleportInvokeType
    {
        Enter = 1,
        Exit = 2,
        Collide = 4
    }

    [Serializable]
    public class AnimationDTO : RandomExecutionModule
    {
        [HideInInspector]
        public string AnimatorAdress;
        public string AnimationName;
        public AnimationType AnimationType;
        public ParameterType ParameterType;
        public string ParameterName;
        [Header("If parameter type is bool or trigger, input 0 for false, and input 1 for true.")]
        public string ParameterValue;
    }

    [Serializable]
    public class AnimationModule : RandomExecutionModule
    {
        public Animator Animator;
        public string AnimationName;
        public AnimationType AnimationType;
        public ParameterType ParameterType;
        public string ParameterName;
        public string ParameterValue;

        public override void Execute(params object[] args)
        {
            List<AnimationModule> modules = args[0] as List<AnimationModule>;
            foreach (AnimationModule module in modules)
            {
                MEC.Timing.CallDelayed(module.ActionDelay, () =>
                {
                    if (module.Animator != null)
                    {
                        if (module.AnimationType == AnimationType.Start)
                        {
                            module.Animator.Play(module.AnimationName);
                            module.Animator.speed = 1f;
                        }
                        else if (module.AnimationType == AnimationType.Stop)
                            module.Animator.speed = 0f;
                        else
                        {
                            switch (module.ParameterType)
                            {
                                case ParameterType.Bool:
                                    module.Animator.SetBool(module.ParameterName, module.ParameterValue == "1");
                                    break;
                                case ParameterType.Float:
                                    module.Animator.SetFloat(module.ParameterName, float.Parse(module.ParameterValue));
                                    break;
                                case ParameterType.Integer:
                                    module.Animator.SetInteger(module.ParameterName, int.Parse(module.ParameterValue));
                                    break;
                                case ParameterType.Trigger:
                                    module.Animator.SetTrigger(module.ParameterName);
                                    break;
                            }
                        }
                    }
                });
            }
        }

        public static List<AnimationModule> GetModules(List<AnimationDTO> list, GameObject gameObject)
        {
            List<AnimationModule> modules = new List<AnimationModule> { };
            foreach (AnimationDTO dTO in list)
            {
                if (!EventManager.FindObjectWithPath(gameObject.GetComponentInParent<SchematicObject>().transform, dTO.AnimatorAdress).TryGetComponent(out Animator animator))
                {
                    ServerConsole.AddLog("Cannot find appopriate animator!");
                    continue;
                }
                modules.Add(new AnimationModule
                {
                    Animator = animator,
                    AnimationName = dTO.AnimationName,
                    AnimationType = dTO.AnimationType,
                    ChanceWeight = dTO.ChanceWeight,
                    ForceExecute = dTO.ForceExecute,
                    ActionDelay = dTO.ActionDelay,
                    ParameterName = dTO.ParameterName,
                    ParameterType = dTO.ParameterType,
                    ParameterValue = dTO.ParameterValue
                });
            }
            return modules;
        }
    }

    [Flags]
    [Serializable]
    public enum DeadType
    {
        Disappear = 1,
        GetRigidbody = 2,
        DynamicDisappearing = 4,
        Explode = 8,
        ResetHP = 16,
        PlayAnimation = 32,
        Warhead = 64,
        SendMessage = 128,
        DropItems = 256,
        SendCommand = 512,
        GiveEffect = 1024
    }

    [Flags]
    [Serializable]
    public enum IPActionType
    {
        Disappear = 1,
        Explode = 2,
        PlayAnimation = 4,
        Warhead = 8,
        SendMessage = 16,
        DropItems = 32,
        SendCommand = 64,
        UpgradeItem = 128,
        GiveEffect = 256
    }

    [Flags]
    [Serializable]
    public enum InvokeType
    {
        Searching = 1,
        Picked = 2
    }

    [Serializable]
    public class RandomExecutionModule
    {
        public float ChanceWeight;
        public bool ForceExecute;
        public float ActionDelay;

        public static RandomExecutionModule GetSingleton<T>() where T : RandomExecutionModule, new()
        {
            if (!AdvancedMERTools.Singleton.TypeSingletonPair.TryGetValue(typeof(T), out RandomExecutionModule type))
            {
                AdvancedMERTools.Singleton.TypeSingletonPair.Add(typeof(T), type = new T());
            }
            return type;
        }

        public static List<T> SelectList<T>(List<T> list) where T : RandomExecutionModule, new()
        {
            float Chance = list.Sum(x => x.ChanceWeight);
            Chance = UnityEngine.Random.Range(0f, Chance);
            List<T> output = new List<T> { };
            foreach (T element in list)
            {
                if (element.ForceExecute)
                    output.Add(element);
                else
                {
                    if (Chance <= 0)
                        continue;
                    Chance -= element.ChanceWeight;
                    if (Chance <= 0)
                    {
                        output.Add(element);
                    }
                }
            }
            return output;
        }

        public virtual void Execute(params object[] args) { }
    }

    [Serializable]
    public enum AnimationType
    {
        Start,
        Stop,
        ModifyParameter
    }

    [Serializable]
    public enum ParameterType
    {
        Integer,
        Float,
        Bool,
        Trigger
    }

    [Flags]
    [Serializable]
    public enum WarheadActionType
    {
        Start = 1,
        Stop = 2,
        Lock = 4,
        UnLock = 8,
        Disable = 16,
        Enable = 32
    }

    [Serializable]
    public enum MessageType
    {
        Cassie,
        BroadCast,
        Hint
    }

    [Flags]
    [Serializable]
    public enum SendType
    {
        Interactor = 1,
        AllExceptAboveOne = 2,
        Alive = 4,
        Spectators = 8
    }

    [Serializable]
    public class DropItem : RandomExecutionModule
    {
        public ItemType ItemType;
        public uint CustomItemId;
        public int Count;
        public SVector3 DropLocalPosition;

        public override void Execute(params object[] args)
        {
            //ServerConsole.AddLog("!!");
            List<DropItem> items = args[0] as List<DropItem>;
            Transform transform = args[1] as Transform;

            //ServerConsole.AddLog(items.Count.ToString());
            foreach (DropItem item in items)
            {
                //ServerConsole.AddLog((transform == null).ToString());
                MEC.Timing.CallDelayed(item.ActionDelay, () => 
                {
                    //ServerConsole.AddLog("!!!");
                    Vector3 vector3 = transform.TransformPoint(item.DropLocalPosition);
                    //ServerConsole.AddLog(vector3.ToString());
                    if (item.CustomItemId != 0 && CustomItem.TryGet(item.CustomItemId, out CustomItem custom))
                    {
                        for (int i = 0; i < item.Count; i++)
                        {
                            custom.Spawn(vector3);
                        }
                    }
                    else
                    {
                        if (!InventoryItemLoader.AvailableItems.TryGetValue(item.ItemType, out ItemBase itemBase) || itemBase.PickupDropModel == null)
                            return;
                        for (int i = 0; i < item.Count; i++)
                        {
                            ItemPickupBase itemPickupBase = UnityEngine.Object.Instantiate<ItemPickupBase>(itemBase.PickupDropModel, vector3, transform.rotation);
                            itemPickupBase.NetworkInfo = new PickupSyncInfo(item.ItemType, itemBase.Weight, 0, false);
                            NetworkServer.Spawn(itemPickupBase.gameObject);
                        }
                    }
                });
            }
        }
    }

    [Serializable]
    public class WhitelistWeapon
    {
        public ItemType ItemType;
        public uint CustomItemId;
    }

    [Serializable]
    public class SVector3
    {
        public float x;
        public float y;
        public float z;

        public static implicit operator Vector3(SVector3 sVector) => new Vector3(sVector.x, sVector.y, sVector.z);
    }

    [Serializable]
    public class Commanding : RandomExecutionModule
    {
        public string CommandContext;

        public override void Execute(params object[] args)
        {
            List<Commanding> modules = args[0] as List<Commanding>;
            Dictionary<string, Func<object[], string>> pairs = args[1] as Dictionary<string, Func<object[], string>>;

            foreach (Commanding module in modules)
            {
                MEC.Timing.CallDelayed(module.ActionDelay, () => 
                {
                    string Content = module.CommandContext;
                    foreach (string v in pairs.Keys)
                    {
                        try
                        {
                            Content = Content.Replace(v, pairs[v]((object[])args.Skip(2).ToArray()));
                        }
                        catch (Exception e) { }
                    }
                    Content = ServerConsole.singleton.NameFormatter.ProcessExpression(Content);
                    AdvancedMERTools.ExecuteCommand(Content);
                });
            }
        }
    }

    [Serializable]
    public class GateSerializable
    {
        public Interactables.Interobjects.DoorUtils.KeycardPermissions keycardPermissions { get; set; }
        public bool RequireAllPermission { get; set; }
        public bool IsLocked { get; set; }
        public bool IsOpened { get; set; }
    }

    //[Serializable]
    //public enum CommandType
    //{
    //    RemoteAdmin,
    //    ClientConsole
    //}

    //[Serializable]
    //public enum ExecutorType
    //{
    //    Attacker,
    //    LocalAdmin
    //}

    //[Serializable]
    //public class DoorInstallingGuideDTO
    //{
    //    public float Health;
    //    public DoorDamageType DamagableDamageType;
    //    public KeycardPermissions KeycardPermissions;
    //    public string ObjectId;
    //}
}
