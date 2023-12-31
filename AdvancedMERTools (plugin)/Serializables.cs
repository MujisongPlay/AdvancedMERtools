using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdvancedMERTools
{
    [Serializable]
    public class HealthObjectDTO
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
        public string MessageContent;
        public MessageType MessageType;
        public SendType SendType;
        public List<DropItem> dropItems;
        public bool DoNotDestroyAfterDeath;
        public List<Commanding> commandings;
        public bool FFon;
    }

    [Serializable]
    public class IPDTO
    {
        public ActionType ActionType;
        public string ObjectId;
        public List<AnimationDTO> animationDTOs;
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

    [Serializable]
    public class AnimationDTO
    {
        public string Animator;
        public string Animation;
        public AnimationType AnimationType;
        public float Chance;
        public bool Force;
    }

    [Serializable]
    public class AnimationModule
    {
        public Animator Animator;
        public string AnimationName;
        public AnimationType AnimationType;
        public float ChanceWeight;
        public bool ForceExecute;
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
        SendCommand = 512
    }

    [Serializable]
    public enum AnimationType
    {
        Start,
        Stop
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
    public class WhitelistWeapon
    {
        public ItemType ItemType;
        public uint CustomItemId;
    }

    [Serializable]
    public class Commanding
    {
        public string CommandContext;
        public float Chance;
        public bool ForceExecute;
        //public CommandType CommandType;
        //public ExecutorType ExecutorType;
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
