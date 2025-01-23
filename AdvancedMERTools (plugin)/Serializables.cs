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
        Explode = 64,
        PlayAudio = 128,
        CallGroovieNoise = 256,
        CallFunction = 512,
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

    [Flags]
    [Serializable]
    public enum EffectFlagE
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

    //[Flags]
    //[Serializable]
    //public enum TeleportInvokeType
    //{
    //    Enter = 1,
    //    Exit = 2,
    //    Collide = 4
    //}

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
        GiveEffect = 1024,
        PlayAudio = 2048,
        CallGroovieNoise = 4096,
        CallFunction = 8192
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
        GiveEffect = 256,
        PlayAudio = 512,
        CallGroovieNoise = 1024,
        CallFunction = 2048,
    }

    [Flags]
    [Serializable]
    public enum InvokeType
    {
        Searching = 1,
        Picked = 2
    }

    [Serializable]
    public enum AnimationTypeE
    {
        Start,
        Stop,
        ModifyParameter
    }

    [Serializable]
    public enum ParameterTypeE
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
    public enum MessageTypeE
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
