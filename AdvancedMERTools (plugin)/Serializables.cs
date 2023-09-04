using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public enum AnimationType
    {
        Start,
        Stop
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
    }

    //[Serializable]
    //public class DoorInstallingGuideDTO
    //{
    //    public float Health;
    //    public DoorDamageType DamagableDamageType;
    //    public KeycardPermissions KeycardPermissions;
    //    public string ObjectId;
    //}
}
