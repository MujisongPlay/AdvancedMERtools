using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapEditorReborn.API.Features;
using UnityEngine;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Serializable;
using MapEditorReborn.API.Features.Objects;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using System.IO;
using Utf8Json;
using System.Reflection.Emit;
using System.Reflection;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using Mirror;
using PlayerRoles;
using RelativePositioning;
using Exiled.API.Enums;

namespace AdvancedMERTools
{
    public class CustomDoor : AMERTInteractable
    {
        void Start()
        {
            Base = base.Base as CDDTO;
            AdvancedMERTools.Singleton.customDoors.Add(this);
            animator = EventManager.FindObjectWithPath(transform, Base.animator).GetComponent<Animator>();
            door = ObjectSpawner.SpawnDoor(new DoorSerializable
            {
                DoorHealth = Base.DoorHealth,
                DoorType = new DoorType[] { DoorType.LightContainmentDoor, DoorType.HeavyContainmentDoor, DoorType.EntranceDoor }[(int)Base.DoorType],
                IgnoredDamageSources = (Interactables.Interobjects.DoorUtils.DoorDamageType)(31 ^ (byte)Base.DoorDamageType),
                KeycardPermissions = (Interactables.Interobjects.DoorUtils.KeycardPermissions)(int)Base.DoorPermissions,
                RoomType = RoomType.Surface,
                Position = transform.position + transform.rotation * Base.DoorInstallPos,
                Rotation = Quaternion.LookRotation(transform.TransformDirection(Base.DoorInstallRot), Vector3.up).eulerAngles,
                Scale = Base.DoorInstallScl
            }).Door;
            door.Transform.parent = this.transform;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteract;
            Exiled.Events.Handlers.Player.DamagingDoor += OnDestroy;
        }

        void OnDestroy()
        {
            AdvancedMERTools.Singleton.customDoors.Remove(this);
        }

        void OnInteract(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (ev.Door != door || !ev.IsAllowed)
                return;
            animator.Play(door.IsOpen ? Base.CloseAnimation : Base.OpenAnimation);
        }

        void OnDestroy(Exiled.Events.EventArgs.Player.DamagingDoorEventArgs ev)
        {
            if (ev.Door != door || !ev.IsAllowed)
                return;
            if (ev.Damage >= (door.Base as Interactables.Interobjects.BreakableDoor).RemainingHealth)
                animator.Play(Base.BrokenAnimation);
        }

        public void OnLockChange(ushort value)
        {
            if (value == 0)
                animator.Play(Base.UnlockAnimation);
            else
                animator.Play(Base.LockAnimation);
        }

        public Animator animator;

        public Door door;

        public new CDDTO Base;
    }
}