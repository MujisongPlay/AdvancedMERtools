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
using System.IO;

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
        public List<AudioModule> AudioModules;
        public List<CGNModule> GroovieNoiseToCall;
        public List<CFEModule> FunctionToCall;
    }

    [Serializable]
    public class FHODTO : AMERTDTO
    {
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

    //[Serializable]
    //public class ITDTO : AMERTDTO
    //{
    //    public TeleportInvokeType InvokeType;
    //    public IPActionType ActionType;
    //    public List<AnimationDTO> animationDTOs;
    //    public WarheadActionType warheadActionType;
    //    public List<MessageModule> MessageModules;
    //    public List<Commanding> commandings;
    //    public List<ExplodeModule> ExplodeModules;
    //    public List<EffectGivingModule> effectGivingModules;
    //    public List<AudioModule> AudioModules;
    //    public List<CGNModule> GroovieNoiseToCall;
    //}

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
        public List<AudioModule> AudioModules;
        public List<CGNModule> GroovieNoiseToCall;
        public List<CFEModule> FunctionToCall;
    }

    [Serializable]
    public class FIPDTO : AMERTDTO
    {
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
        public List<AudioModule> AudioModules;
        public List<CGNModule> GroovieNoiseToCall;
        public List<CFEModule> FunctionToCall;
    }

    [Serializable]
    public class FCCDTO : AMERTDTO
    {
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

    [Serializable]
    public class IODTO : AMERTDTO
    {
        public int InputKeyCode;
        public float InteractionMaxRange;
        public IPActionType ActionType;
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
    public class FIODTO : AMERTDTO
    {
        public int InputKeyCode;
        public ScriptValue InteractionMaxRange;
        public IPActionType ActionType;
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

    [Serializable]
    public class CGNModule : RandomExecutionModule
    {
        public int GroovieNoiseId;
        public string GroovieNoiseGroup;

        public override void Execute(ModuleGeneralArguments args)
        {
            MEC.Timing.CallDelayed(ActionDelay, () =>
            {
                //ServerConsole.AddLog("!!");
                if (AdvancedMERTools.Singleton.codeClassPair[args.schematic].TryGetValue(GroovieNoiseId, out AMERTInteractable v))
                    v.Active = true;
                if (AdvancedMERTools.Singleton.AMERTGroup[args.schematic].TryGetValue(GroovieNoiseGroup, out List<AMERTInteractable> vs))
                    vs.ForEach(x => x.Active = true);
            });
        }
    }

    [Serializable]
    public class FCGNModule : FRandomExecutionModule
    {
        public ScriptValue GroovieNoiseId;
        public ScriptValue GroovieNoiseGroup;

        public override void Execute(FunctionArgument args)
        {
            MEC.Timing.CallDelayed(ActionDelay.GetValue(args, 0f), () => 
            {
                if (AdvancedMERTools.Singleton.codeClassPair[args.schematic].TryGetValue(GroovieNoiseId.GetValue(args, 0), out AMERTInteractable v))
                    v.Active = true;
                if (AdvancedMERTools.Singleton.AMERTGroup[args.schematic].TryGetValue(GroovieNoiseGroup.GetValue(args, ""), out List<AMERTInteractable> vs))
                    vs.ForEach(x => x.Active = true);
            });
        }
    }

    [Serializable]
    public class GNDTO : AMERTDTO
    {
        public List<GMDTO> Settings;
    }

    [Serializable]
    public class FGNDTO : AMERTDTO
    {
        public List<FGMDTO> Settings;
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
        public List<int> Targets;
        public List<string> TargetGroups;
        public bool Enable;

        public override void Execute(ModuleGeneralArguments args)
        {
            MEC.Timing.CallDelayed(ActionDelay, () =>
            {
                Targets.ForEach(x =>
                {
                    if (AdvancedMERTools.Singleton.codeClassPair[args.schematic].TryGetValue(x, out AMERTInteractable v))
                        v.Active = Enable;
                });
                TargetGroups.ForEach(x => 
                {
                    if (AdvancedMERTools.Singleton.AMERTGroup[args.schematic].TryGetValue(x, out List<AMERTInteractable> vs))
                        vs.ForEach(y => y.Active = Enable);
                });
            });
        }
    }

    [Serializable]
    public class FGMDTO : FRandomExecutionModule
    {
        public ScriptValue Targets;
        public ScriptValue TargetGroups;
        public ScriptValue Enable;

        public override void Execute(FunctionArgument args)
        {
            MEC.Timing.CallDelayed(ActionDelay.GetValue(args, 0f), () =>
            {
                Targets.GetValue(args, new int[] { }).ForEach(x =>
                {
                    if (AdvancedMERTools.Singleton.codeClassPair[args.schematic].TryGetValue(x, out AMERTInteractable v))
                        v.Active = Enable.GetValue(args, v.Active);
                });
                TargetGroups.GetValue(args, new string[] { }).ForEach(x =>
                {
                    if (AdvancedMERTools.Singleton.AMERTGroup[args.schematic].TryGetValue(x, out List<AMERTInteractable> vs))
                        vs.ForEach(y => y.Active = Enable.GetValue(args, y.Active));
                });
            });
        }
    }

    [Serializable]
    public class AMERTDTO
    {
        public bool Active;
        public string ObjectId;
        public int Code;
        public string ScriptGroup;
        public bool UseScriptValue;
    }

    [Serializable]
    public class AMERTInteractable : NetworkBehaviour
    {
        void OnDestroy()
        {
            AdvancedMERTools.Singleton.codeClassPair[OSchematic].Remove(Base.Code);
            AdvancedMERTools.Singleton.AMERTGroup[OSchematic][Base.ScriptGroup].Remove(this);
        }

        public static void AlphaWarhead(WarheadActionType type)
        {
            foreach (WarheadActionType warhead in Enum.GetValues(typeof(WarheadActionType)))
            {
                if (type.HasFlag(warhead))
                {
                    switch (warhead)
                    {
                        case WarheadActionType.Start:
                            Warhead.Start();
                            break;
                        case WarheadActionType.Stop:
                            Warhead.Stop();
                            break;
                        case WarheadActionType.Lock:
                            Warhead.IsLocked = true;
                            break;
                        case WarheadActionType.UnLock:
                            Warhead.IsLocked = false;
                            break;
                        case WarheadActionType.Disable:
                            Warhead.LeverStatus = false;
                            break;
                        case WarheadActionType.Enable:
                            Warhead.LeverStatus = true;
                            break;
                    }
                }
            }
        }

        public SchematicObject OSchematic;
        public AMERTDTO Base;
        public bool Active;
    }

    [Serializable]
    public class EffectGivingModule : RandomExecutionModule
    {
        public EffectFlagE EffectFlag;
        public EffectType effectType;
        public SendType GivingTo;
        public byte Inensity;
        public float Duration;

        public override void Execute(ModuleGeneralArguments args)
        {
            MEC.Timing.CallDelayed(ActionDelay, () =>
            {
                if (!args.TargetCalculated)
                    args.targets = GetTargets(GivingTo, args);
                foreach (Player player in args.targets)
                {
                    if (EffectFlag.HasFlag(EffectFlagE.Disable))
                        player.DisableEffect(effectType);
                    else if (EffectFlag.HasFlag(EffectFlagE.Enable))
                    {
                        byte intensity = (byte)((EffectFlag.HasFlag(EffectFlagE.ModifyIntensity) ? player.GetEffect(effectType).Intensity : 0) + Inensity);
                        player.EnableEffect(effectType, intensity, Duration, EffectFlag.HasFlag(EffectFlagE.ModifyDuration));
                    }
                }
            });
        }
    }

    [Serializable]
    public class FEffectGivingModule : FRandomExecutionModule
    {
        public ScriptValue EffectFlag;
        public ScriptValue effectType;
        public ScriptValue GivingTo;
        public ScriptValue Inensity;
        public ScriptValue Duration;

        public override void Execute(FunctionArgument args)
        {
            MEC.Timing.CallDelayed(ActionDelay.GetValue(args, 0f), () =>
            {
                EffectFlagE flag = EffectFlag.GetValue<EffectFlagE>(args, 0);
                EffectType type = effectType.GetValue(args, EffectType.None);
                foreach (Player player in GivingTo.GetValue(args, new Player[] { }))
                {
                    if (flag.HasFlag(EffectFlagE.Disable))
                        player.DisableEffect(type);
                    else if (flag.HasFlag(EffectFlagE.Enable))
                    {
                        byte intensity = (byte)((flag.HasFlag(EffectFlagE.ModifyIntensity) ? player.GetEffect(type).Intensity : 0) + Inensity.GetValue(args, 0));
                        player.EnableEffect(type, intensity, Duration.GetValue(args, 0f), flag.HasFlag(EffectFlagE.ModifyDuration));
                    }
                }
            });
        }
    }

    [Serializable]
    public class ExplodeModule : RandomExecutionModule
    {
        public bool FFon;
        public bool EffectOnly;
        public SVector3 LocalPosition;

        public override void Execute(ModuleGeneralArguments args)
        {
            ReferenceHub.TryGetLocalHub(out ReferenceHub local);
            MEC.Timing.CallDelayed(ActionDelay, () =>
            {
                if (EffectOnly)
                    ExplosionUtils.ServerSpawnEffect(args.transform.TransformPoint(LocalPosition), ItemType.GrenadeHE);
                else
                    ExplosionUtils.ServerExplode(args.transform.TransformPoint(LocalPosition), FFon ? new Footprint(local) : new Footprint(args.player.ReferenceHub), ExplosionType.Grenade);
            });
        }
    }

    [Serializable]
    public class FExplodeModule : FRandomExecutionModule
    {
        public ScriptValue FFon;
        public ScriptValue EffectOnly;
        public ScriptValue LocalPosition;

        public override void Execute(FunctionArgument args)
        {
            ReferenceHub.TryGetLocalHub(out ReferenceHub local);
            MEC.Timing.CallDelayed(ActionDelay.GetValue(args, 0f), () =>
            {
                if (EffectOnly.GetValue(args, true))
                    ExplosionUtils.ServerSpawnEffect(args.transform.TransformPoint(LocalPosition.GetValue(args, Vector3.zero)), ItemType.GrenadeHE);
                else
                    ExplosionUtils.ServerExplode(args.transform.TransformPoint(LocalPosition.GetValue(args, Vector3.zero)), FFon.GetValue(args, false) ? new Footprint(local) : new Footprint(args.player.ReferenceHub), ExplosionType.Grenade);
            });
        }
    }

    [Serializable]
    public class AudioModule : RandomExecutionModule
    {
        public string AudioName;
        [Header("0: Loop")]
        public int PlayCount;
        public bool IsSpatial;
        public float MaxDistance;
        public float MinDistance;
        public float Volume;
        public SVector3 LocalPlayPosition;
        public AudioPlayer AP;
        bool loaded;

        public override void Execute(ModuleGeneralArguments args)
        {
            MEC.Timing.CallDelayed(ActionDelay, () =>
            {
                if (!loaded)
                {
                    if (!Directory.Exists(AdvancedMERTools.Singleton.Config.AudioFolderPath))
                    {
                        ServerConsole.AddLog("Cannot find Audio Folder Directory!", ConsoleColor.Red);
                        return;
                    }
                    if (!AudioClipStorage.AudioClips.ContainsKey(AudioName))
                        AudioClipStorage.LoadClip(Path.Combine(AdvancedMERTools.Singleton.Config.AudioFolderPath, AudioName), AudioName);
                    loaded = true;
                }
                if (AP == null)
                {
                    AP = AudioPlayer.Create($"AudioHandler-{args.transform.GetHashCode()}-{GetHashCode()}");
                    Speaker speaker = AP.AddSpeaker("Primary", args.transform.TransformPoint(LocalPlayPosition), Volume, IsSpatial, MinDistance, MaxDistance);
                    AP.transform.parent = speaker.transform.parent = args.transform;
                    AP.transform.localPosition = speaker.transform.localPosition = LocalPlayPosition;
                    //ServerConsole.AddLog(speaker.transform.position.ToPreciseString());
                }
                if (PlayCount == 0)
                    AP.AddClip(AudioName, Volume, true, false);
                for (int i = 0; i < PlayCount; i++)
                    AP.AddClip(AudioName, Volume, false, false);
            });
        }
    }

    [Serializable]
    public class FAudioModule : FRandomExecutionModule
    {
        public ScriptValue AudioName;
        [Header("0: Loop")]
        public ScriptValue PlayCount;
        public ScriptValue IsSpatial;
        public ScriptValue MaxDistance;
        public ScriptValue MinDistance;
        public ScriptValue Volume;
        public ScriptValue LocalPlayPosition;
        public AudioPlayer AP;

        public override void Execute(FunctionArgument args)
        {
            MEC.Timing.CallDelayed(ActionDelay.GetValue(args, 0f), () =>
            {
                string audioName = AudioName.GetValue<string>(args, null);
                if (audioName == null)
                    return;
                if (!Directory.Exists(AdvancedMERTools.Singleton.Config.AudioFolderPath))
                {
                    ServerConsole.AddLog("Cannot find Audio Folder Directory!", ConsoleColor.Red);
                    return;
                }
                if (!AudioClipStorage.AudioClips.ContainsKey(audioName))
                    AudioClipStorage.LoadClip(Path.Combine(AdvancedMERTools.Singleton.Config.AudioFolderPath, audioName), audioName);
                Vector3 vector = LocalPlayPosition.GetValue(args, Vector3.zero);
                float vol = Volume.GetValue(args, 1f);
                if (AP == null)
                {
                    AP = AudioPlayer.Create($"AudioHandler-{args.transform.GetHashCode()}-{GetHashCode()}");
                    Speaker speaker = AP.AddSpeaker($"Primary-{audioName}", args.transform.TransformPoint(vector), vol, IsSpatial.GetValue(args, true), MinDistance.GetValue(args, 5f), MaxDistance.GetValue(args, 5f));
                    //ServerConsole.AddLog(speaker.transform.position.ToPreciseString());
                }
                AP.SpeakersByName[$"Primary-{audioName}"].transform.parent = args.transform;
                AP.SpeakersByName[$"Primary-{audioName}"].transform.localPosition = vector;
                int PC = PlayCount.GetValue(args, 1);
                if (PC == 0)
                    AP.AddClip(audioName, vol, true, false);
                for (int i = 0; i < PC; i++)
                    AP.AddClip(audioName, vol, false, false);
            });
        }
    }

    [Serializable]
    public class MessageModule : RandomExecutionModule
    {
        public SendType SendType;
        public string MessageContent;
        public MessageTypeE MessageType;
        public float Duration;

        public override void Execute(ModuleGeneralArguments args)
        {
            MEC.Timing.CallDelayed(ActionDelay, () =>
            {
                string Content = MessageContent;
                foreach (string v in args.interpolations.Keys)
                {
                    try
                    {
                        Content = Content.Replace(v, args.interpolations[v](args.interpolationsList));
                    }
                    catch (Exception _) { }
                }
                try
                {
                    Content = ServerConsole.singleton.NameFormatter.ProcessExpression(Content);
                }
                catch (Exception e) { }
                if (!args.TargetCalculated)
                    args.targets = GetTargets(SendType, args);
                if (MessageType == MessageTypeE.Cassie)
                {
                    Cassie.Message(Content);
                }
                else
                {
                    foreach (Player p in args.targets)
                    {
                        if (MessageType == MessageTypeE.BroadCast)
                        {
                            p.Broadcast((ushort)Math.Round(Duration), Content);
                        }
                        else
                        {
                            p.ShowHint(Content, Duration);
                        }
                    }
                }
            });
        }
    }

    [Serializable]
    public class FMessageModule : FRandomExecutionModule
    {
        public ScriptValue SendType;
        public ScriptValue MessageContent;
        public ScriptValue MessageType;
        public ScriptValue Duration;

        public override void Execute(FunctionArgument args)
        {
            MEC.Timing.CallDelayed(ActionDelay.GetValue(args, 0f), () =>
            {
                string Content = MessageContent.GetValue(args, "");
                MessageTypeE type = MessageType.GetValue(args, MessageTypeE.BroadCast);
                if (type == MessageTypeE.Cassie)
                {
                    Cassie.Message(Content);
                }
                else
                {
                    foreach (Player p in SendType.GetValue(args, new Player[] { }))
                    {
                        if (type == MessageTypeE.BroadCast)
                        {
                            p.Broadcast((ushort)Math.Round(Duration.GetValue(args, 0f)), Content);
                        }
                        else
                        {
                            p.ShowHint(Content, Duration.GetValue(args, 0f));
                        }
                    }
                }
            });
        }
    }

    [Serializable]
    public class AnimationDTO : RandomExecutionModule
    {
        public Animator Animator;
        [HideInInspector]
        public string AnimatorAdress;
        public string AnimationName;
        public AnimationTypeE AnimationType;
        public ParameterTypeE ParameterType;
        public string ParameterName;
        [Header("If parameter type is bool or trigger, input 0 for false, and input 1 for true.")]
        public string ParameterValue;

        public override void Execute(ModuleGeneralArguments args)
        {
            MEC.Timing.CallDelayed(ActionDelay, () =>
            {
                if (Animator == null)
                {
                    if (!EventManager.FindObjectWithPath(args.schematic.GetComponentInParent<SchematicObject>().transform, AnimatorAdress).TryGetComponent(out Animator animator))
                    {
                        ServerConsole.AddLog("Cannot find appopriate animator!");
                        return;
                    }
                    Animator = animator;
                }
                if (AnimationType == AnimationTypeE.Start)
                {
                    Animator.Play(AnimationName);
                    Animator.speed = 1f;
                }
                else if (AnimationType == AnimationTypeE.Stop)
                    Animator.speed = 0f;
                else
                {
                    switch (ParameterType)
                    {
                        case ParameterTypeE.Bool:
                            Animator.SetBool(ParameterName, ParameterValue == "1");
                            break;
                        case ParameterTypeE.Float:
                            Animator.SetFloat(ParameterName, float.Parse(ParameterValue));
                            break;
                        case ParameterTypeE.Integer:
                            Animator.SetInteger(ParameterName, int.Parse(ParameterValue));
                            break;
                        case ParameterTypeE.Trigger:
                            Animator.SetTrigger(ParameterName);
                            break;
                    }
                }
            });
        }
    }

    [Serializable]
    public class FAnimationDTO : FRandomExecutionModule
    {
        public Animator Animator;
        [HideInInspector]
        public string AnimatorAdress;
        public ScriptValue AnimationName;
        public ScriptValue AnimationType;
        public ScriptValue ParameterType;
        public ScriptValue ParameterName;
        [Header("If parameter type is bool or trigger, input 0 for false, and input 1 for true.")]
        public ScriptValue ParameterValue;

        public override void Execute(FunctionArgument args)
        {
            MEC.Timing.CallDelayed(ActionDelay.GetValue(args, 0f), () =>
            {
                if (Animator == null)
                {
                    if (!EventManager.FindObjectWithPath(args.schematic.GetComponentInParent<SchematicObject>().transform, AnimatorAdress).TryGetComponent(out Animator animator))
                    {
                        ServerConsole.AddLog("Cannot find appopriate animator!");
                        return;
                    }
                    Animator = animator;
                }
                AnimationTypeE type = AnimationType.GetValue(args, AnimationTypeE.Start);
                if (type == AnimationTypeE.Start)
                {
                    Animator.Play(AnimationName.GetValue(args, ""));
                    Animator.speed = 1f;
                }
                else if (type == AnimationTypeE.Stop)
                    Animator.speed = 0f;
                else
                {
                    string pm = ParameterName.GetValue<string>(args, null);
                    if (pm == null)
                        return;
                    switch (ParameterType.GetValue(args, ParameterTypeE.Integer))
                    {
                        case ParameterTypeE.Bool:
                            Animator.SetBool(pm, ParameterValue.GetValue(args, "1") == "1");
                            break;
                        case ParameterTypeE.Float:
                            Animator.SetFloat(pm, ParameterValue.GetValue(args, 0f));
                            break;
                        case ParameterTypeE.Integer:
                            Animator.SetInteger(pm, ParameterValue.GetValue(args, 0));
                            break;
                        case ParameterTypeE.Trigger:
                            Animator.SetTrigger(pm);
                            break;
                    }
                }
            });
        }
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

        public static void Execute<T>(List<T> list, ModuleGeneralArguments args) where T : RandomExecutionModule, new()
        {
            SelectList(list).ForEach(x => x.Execute(args));
        }

        public static Player[] GetTargets(SendType type, ModuleGeneralArguments args)
        {
            List<Player> targets = new List<Player> { };
            if (type.HasFlag(SendType.AllExceptAboveOne))
                targets.AddRange(Player.List.Where(x => x != args.player));
            if (type.HasFlag(SendType.Spectators))
                targets.AddRange(Player.List.Where(x => !x.IsAlive));
            if (type.HasFlag(SendType.Alive))
                targets.AddRange(Player.List.Where(x => x.IsAlive));
            if (type.HasFlag(SendType.Interactor))
                targets.Add(args.player);
            return targets.Distinct().ToArray();
        }

        public virtual void Execute(ModuleGeneralArguments args) { }
    }

    [Serializable]
    public class FRandomExecutionModule
    {
        public ScriptValue ChanceWeight;
        public ScriptValue ForceExecute;
        public ScriptValue ActionDelay;
        float calcedWeight;

        public static List<T> SelectList<T>(List<T> list, FunctionArgument args) where T : FRandomExecutionModule, new()
        {
            float Chance = list.Sum(x => x.calcedWeight = x.ChanceWeight.GetValue(args, 0f));
            Chance = UnityEngine.Random.Range(0f, Chance);
            List<T> output = new List<T> { };
            foreach (T element in list)
            {
                if (element.ForceExecute.GetValue(args, false))
                    output.Add(element);
                else
                {
                    if (Chance <= 0)
                        continue;
                    Chance -= element.calcedWeight;
                    if (Chance <= 0)
                    {
                        output.Add(element);
                    }
                }
            }
            return output;
        }

        public static void Execute<T>(List<T> list, FunctionArgument args) where T : FRandomExecutionModule, new()
        {
            SelectList(list, args).ForEach(x => x.Execute(args));
        }

        public virtual void Execute(FunctionArgument args) { }
    }

    [Serializable]
    public class DropItem : RandomExecutionModule
    {
        public ItemType ItemType;
        public uint CustomItemId;
        public int Count;
        public SVector3 DropLocalPosition;

        public override void Execute(ModuleGeneralArguments args)
        {
            MEC.Timing.CallDelayed(ActionDelay, () =>
            {
                //ServerConsole.AddLog("!!!");
                Vector3 vector3 = args.transform.TransformPoint(DropLocalPosition);
                //ServerConsole.AddLog(vector3.ToString());
                if (CustomItemId != 0 && CustomItem.TryGet(CustomItemId, out CustomItem custom))
                {
                    for (int i = 0; i < Count; i++)
                    {
                        custom.Spawn(vector3);
                    }
                }
                else
                {
                    if (!InventoryItemLoader.AvailableItems.TryGetValue(ItemType, out ItemBase itemBase) || itemBase.PickupDropModel == null)
                        return;
                    for (int i = 0; i < Count; i++)
                    {
                        ItemPickupBase itemPickupBase = UnityEngine.Object.Instantiate<ItemPickupBase>(itemBase.PickupDropModel, vector3, args.transform.rotation);
                        itemPickupBase.NetworkInfo = new PickupSyncInfo(ItemType, itemBase.Weight, 0, false);
                        NetworkServer.Spawn(itemPickupBase.gameObject);
                    }
                }
            });
        }
    }

    [Serializable]
    public class FDropItem : FRandomExecutionModule
    {
        public ScriptValue ItemType;
        public ScriptValue CustomItemId;
        public ScriptValue Count;
        public ScriptValue DropLocalPosition;

        public override void Execute(FunctionArgument args)
        {
            MEC.Timing.CallDelayed(ActionDelay.GetValue(args, 0f), () =>
            {
                //ServerConsole.AddLog("!!!");
                Vector3 vector3 = args.transform.TransformPoint(DropLocalPosition.GetValue(args, Vector3.zero));
                //ServerConsole.AddLog(vector3.ToString());
                int u = CustomItemId.GetValue(args, 0);
                int c = Count.GetValue(args, 1);
                if (u != 0 && CustomItem.TryGet((uint)u, out CustomItem custom))
                {
                    for (int i = 0; i < c; i++)
                    {
                        custom.Spawn(vector3);
                    }
                }
                else
                {
                    ItemType value = ItemType.GetValue(args, global::ItemType.KeycardJanitor);
                    if (!InventoryItemLoader.AvailableItems.TryGetValue(value, out ItemBase itemBase) || itemBase.PickupDropModel == null)
                        return;
                    for (int i = 0; i < c; i++)
                    {
                        ItemPickupBase itemPickupBase = UnityEngine.Object.Instantiate<ItemPickupBase>(itemBase.PickupDropModel, vector3, args.transform.rotation);
                        itemPickupBase.NetworkInfo = new PickupSyncInfo(value, itemBase.Weight, 0, false);
                        NetworkServer.Spawn(itemPickupBase.gameObject);
                    }
                }
            });
        }
    }

    [Serializable]
    public class WhitelistWeapon
    {
        public ItemType ItemType;
        public uint CustomItemId;
    }

    [Serializable]
    public class FWhitelistWeapon : Value
    {
        public override void OnValidate()
        {
            ItemType.OnValidate();
            CustomItemId.OnValidate();
        }

        public ScriptValue ItemType;
        public ScriptValue CustomItemId;
    }

    [Serializable]
    public class SVector3
    {
        public float x;
        public float y;
        public float z;

        public static implicit operator Vector3(SVector3 sVector) => new Vector3(sVector.x, sVector.y, sVector.z);
    }

    public class ModuleGeneralArguments
    {
        public SchematicObject schematic;
        public Player player;
        public Player[] targets;
        public bool TargetCalculated;
        public Transform transform;
        public Dictionary<string, Func<object[], string>> interpolations;
        public object[] interpolationsList;
    }

    [Serializable]
    public class CFEModule : RandomExecutionModule
    {
        public string FunctionName;

        public override void Execute(ModuleGeneralArguments args)
        {
            MEC.Timing.CallDelayed(ActionDelay, () =>
            {
                if (AdvancedMERTools.Singleton.FunctionExecutors[args.schematic].TryGetValue(FunctionName, out FunctionExecutor function))
                    function.data.Execute(new FunctionArgument { player = args.player });
            });
        }
    }

    [Serializable]
    public class FCFEModule : FRandomExecutionModule
    {
        public ScriptValue FunctionName;
        public List<ScriptValue> FunctionArguments;

        public override void Execute(FunctionArgument args)
        {
            MEC.Timing.CallDelayed(ActionDelay.GetValue(args, 0f), () =>
            {
                //ServerConsole.AddLog("!!!");
                if (AdvancedMERTools.Singleton.FunctionExecutors[args.schematic].TryGetValue(FunctionName.GetValue(args, ""), out FunctionExecutor function))
                    function.data.Execute(new FunctionArgument { Arguments = FunctionArguments.Select(x => x.GetValue(args)).ToList(), player = args.player, schematic = args.schematic });
            });
        }
    }

    [Serializable]
    public class Commanding : RandomExecutionModule
    {
        public string CommandContext;

        public override void Execute(ModuleGeneralArguments args)
        {
            MEC.Timing.CallDelayed(ActionDelay, () =>
            {
                string Content = CommandContext;
                foreach (string v in args.interpolations.Keys)
                {
                    try
                    {
                        Content = Content.Replace(v, args.interpolations[v](args.interpolationsList));
                    }
                    catch (Exception) { }
                }
                Content = ServerConsole.singleton.NameFormatter.ProcessExpression(Content);
                AdvancedMERTools.ExecuteCommand(Content);
            });
        }
    }

    [Serializable]
    public class FCommanding : FRandomExecutionModule
    {
        public ScriptValue CommandContext;

        public override void Execute(FunctionArgument args)
        {
            MEC.Timing.CallDelayed(ActionDelay.GetValue(args, 0f), () =>
            {
                AdvancedMERTools.ExecuteCommand(CommandContext.GetValue(args, ""));
            });
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
}
