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

namespace AdvancedMERTools
{
    public class DummyDoor : MonoBehaviour
    {
        public DoorSerializable door;
        public Door RealDoor = null;

        public Animator animator;

        static readonly Config config = AdvancedMERTools.Singleton.Config;

        void Start()
        {
            MEC.Timing.CallDelayed(1f, () =>
            {
                animator = this.transform.GetChild(0).GetComponent<Animator>();
                if (RealDoor == null)
                {
                    foreach (DoorObject Door in GameObject.FindObjectsOfType<DoorObject>())
                    {
                        if (door == Door.Base)
                        {
                            RealDoor = Door.Door;
                            break;
                        }
                    }
                    if (RealDoor == null)
                    {
                        float distance = float.MaxValue;
                        foreach (Door Door in Door.List)
                        {
                            if (distance > Vector3.Distance(Door.Position, this.transform.position))
                            {
                                distance = Vector3.Distance(Door.Position, transform.position);
                                RealDoor = Door;
                            }
                        }
                        if (RealDoor == null)
                        {
                            ServerConsole.AddLog("Failed to find proper door!", ConsoleColor.Red);
                            Destroy(this.gameObject);
                        }
                    }
                }
                this.transform.parent = RealDoor.Base.transform;
                this.transform.localEulerAngles = Vector3.zero;
                this.transform.localPosition = Vector3.zero;
                if (RealDoor.Base.Rooms.Length != 0)
                {
                    AdvancedMERTools.Singleton.dummyDoors.Remove(this);
                    Destroy(this.gameObject);
                    return;
                }
                animator.Play("DoorClose");
            });
        }

        public void OnInteractDoor(bool trigger)
        {
            if (this.RealDoor == null || animator == null)
                return;
            animator.Play(trigger ? "DoorOpen" : "DoorClose");
        }

        void Update()
        {
            if (RealDoor == null) return;
            if ((RealDoor as BreakableDoor).IsDestroyed)
            {
                AdvancedMERTools.Singleton.dummyDoors.Remove(this);
                Destroy(this.gameObject, 0.5f);
            }
        }
    }

    public class DummyGate : MonoBehaviour
    {
        void Start()
        {
            pickups = (from item in this.gameObject.GetComponentsInChildren<InventorySystem.Items.Pickups.ItemPickupBase>()
                       select Exiled.API.Features.Pickups.Pickup.Get(item)).ToArray();
            if (MapEditorReborn.API.API.CurrentLoadedMap != null && AdvancedMERTools.Singleton.Config.Gates.TryGetValue(MapEditorReborn.API.API.CurrentLoadedMap.Name, out List<GateSerializable> ser))
            {
                if (ser.Count > AdvancedMERTools.Singleton.dummyGates.Count)
                {
                    GateSerializable = ser[AdvancedMERTools.Singleton.dummyGates.Count];
                    if (GateSerializable.IsOpened)
                    {
                        MEC.Timing.CallDelayed(3f, () => { IsOpened = true; });
                    }
                }
            }
            animator = this.transform.GetChild(1).GetComponent<Animator>();
            AdvancedMERTools.Singleton.dummyGates.Add(this);
            MEC.Timing.RunCoroutine(enumerator());
        }

        IEnumerator<float> enumerator()
        {
            yield return MEC.Timing.WaitUntilTrue(() => Round.IsStarted);
            MEC.Timing.CallDelayed(0.3f, Apply);
            yield break;
        }

        /*public void Apply()
        //{
            //AudioSource = AdvancedMERTools.MakeAudio("Dummy Gate #" + AdvancedMERTools.Singleton.dummyGates.Count.ToString());
            //audioPlayer = AudioPlayerBase.Get(AudioSource);
            //audioPlayer.BroadcastChannel = VoiceChat.VoiceChatChannel.Proximity;
            //MEC.Timing.CallDelayed(0.3f, () =>
            //{
            //    AudioSource.roleManager.ServerSetRole(PlayerRoles.RoleTypeId.Tutorial, PlayerRoles.RoleChangeReason.None, PlayerRoles.RoleSpawnFlags.None);
            //    audioPlayer.BroadcastChannel = VoiceChat.VoiceChatChannel.Proximity;
            //});
            //MEC.Timing.CallDelayed(0.4f, () =>
            //{
            //    FpcStandardRoleBase fpc = AudioSource.roleManager.CurrentRole as FpcStandardRoleBase;
            //    if (fpc != null)
            //    {
            //        fpc.FpcModule.Noclip.IsActive = true;
            //        AudioSource.transform.position = this.transform.position;
            //        AudioSource.transform.localScale = Vector3.one * 0.1f;
            //    }
            //});

            //ServerConsole.AddLog(this.transform.position.ToString());
            //npc = Npc.Spawn("Gate", PlayerRoles.RoleTypeId.Tutorial, position: this.transform.position);
            //PropertyInfo info = typeof(CentralAuth.PlayerAuthenticationManager).GetProperty("InstanceMode");
            //info.SetValue(npc.ReferenceHub.authManager, CentralAuth.ClientInstanceMode.DedicatedServer);
            //audioPlayer = AudioPlayerBase.Get(npc.ReferenceHub);
            //MEC.Timing.CallDelayed(0.35f, () =>
            //{
            //    info.SetValue(npc.ReferenceHub.authManager, CentralAuth.ClientInstanceMode.ReadyClient);
            //});
            //MEC.Timing.CallDelayed(0.55f, () =>
            //{
            //    npc.Scale = Vector3.one * -0.1f;
            //    npc.Position = this.transform.position;
            //    (npc.RoleManager.CurrentRole as FpcStandardRoleBase).FpcModule.Noclip.IsActive = true;

            //});
            //MEC.Timing.CallDelayed(0.45f, () =>
            //{
            //    info.SetValue(npc.ReferenceHub.authManager, CentralAuth.ClientInstanceMode.DedicatedServer);
            //});
        //}*/

        public void Apply()
        {
            //ReferenceHub hub = AdvancedMERTools.MakeAudio(out int id);
            //MEC.Timing.CallDelayed(0.35f, () =>
            //{
            //    hub.authManager.UserId = "ID_Dedicated";
            //    //Central Core.
            //    hub.authManager.NetworkSyncedUserId = "ID_Dedicated";
            //    hub.nicknameSync.DisplayName = $"{id}-Gate";
            //    hub.characterClassManager.GodMode = true;
            //    hub.transform.localScale = Vector3.one * -0.01f;
            //    foreach (Player player in Player.List)
            //    {
            //        Server.SendSpawnMessage.Invoke(null, new object[]
            //        {
            //            hub.netIdentity,
            //            player.Connection
            //        });
            //    }
            //    FirstPersonMovementModule module = (hub.roleManager.CurrentRole as FpcStandardRoleBase).FpcModule;
            //    //Preventer.
            //    module.Position = this.transform.position - Vector3.up * 0.1f;
            //    module.Motor.ReceivedPosition = new RelativePosition(this.transform.position - Vector3.up * 0.1f);
            //    module.Noclip.IsActive = true;
            //});
            //MEC.Timing.CallDelayed(0.45f, () =>
            //{
            //    PropertyInfo info = typeof(CentralAuth.PlayerAuthenticationManager).GetProperty("InstanceMode");
            //    info.SetValue(hub.authManager, CentralAuth.ClientInstanceMode.DedicatedServer);
            //    //hub.authManager.UserId = null;
            //});
            //audioPlayer = AudioPlayerBase.Get(hub);
        }

        void Update()
        {
            if (Cooldown >= 0)
                Cooldown -= Time.deltaTime;
        }

        public void OnPickingUp(SearchingPickupEventArgs ev)
        {
            if (pickups.Contains(ev.Pickup))
            {
                ev.IsAllowed = false;
                if (GateSerializable != null)
                {
                    if (!GateSerializable.IsLocked && CheckPermission(ev.Player, GateSerializable.keycardPermissions))
                    {
                        goto IL_01;
                    }
                }
                else
                {
                    goto IL_01;
                }
            }
            return;
        IL_01:;
            if (Cooldown <= 0)
            {
                IsOpened = !IsOpened;
            }
        }

        public bool CheckPermission(Exiled.API.Features.Player player, Interactables.Interobjects.DoorUtils.KeycardPermissions keycard)
        {
            if (keycard == Interactables.Interobjects.DoorUtils.KeycardPermissions.None)
            {
                return true;
            }
            if (player != null)
            {
                if (player.IsBypassModeEnabled)
                {
                    return true;
                }
                if (player.IsScp)
                {
                    return keycard.HasFlag(Interactables.Interobjects.DoorUtils.KeycardPermissions.ScpOverride);
                }
                if (player.CurrentItem == null)
                {
                    return false;
                }
                InventorySystem.Items.Keycards.KeycardItem keycardItem = player.CurrentItem.Base as InventorySystem.Items.Keycards.KeycardItem;
                if (keycardItem != null)
                {
                    if (GateSerializable != null && GateSerializable.RequireAllPermission)
                    {
                        return (keycardItem.Permissions & keycard) == keycard;
                    }
                    return (keycardItem.Permissions & keycard) > Interactables.Interobjects.DoorUtils.KeycardPermissions.None;
                }
            }
            return false;
        }

        public bool IsOpened
        {
            get
            {
                return _IsOpened;
            }
            set
            {
                if (animator != null)
                {
                    //audioPlayer.CurrentPlay = Path.Combine(Path.Combine(Paths.Configs, "Music"), value ? "GateOpen.ogg" : "GateClose.ogg");
                    //audioPlayer.Loop = false;
                    //audioPlayer.Play(-1);
                    animator.Play(value ? "GateOpen" : "GateClose");
                }
                Cooldown = 3f;
                _IsOpened = value;
            }
        }

        GateSerializable GateSerializable;
        public float Cooldown = 0f;
        bool _IsOpened = false;
        public Animator animator;
        public Exiled.API.Features.Pickups.Pickup[] pickups;
        //AudioPlayerBase audioPlayer;
    }
}
