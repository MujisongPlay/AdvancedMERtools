using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.Loader;
using UnityEngine;
using Exiled.API.Features.Pickups;
using CustomCulling;
using Interactables.Interobjects;
using Interactables;
using Interactables.Interobjects.DoorUtils;
using System.IO;
using Utf8Json;
using MapEditorReborn.API.Features.Objects;
using Exiled.Events.Features;
using Exiled.Events;
using PlayerRoles;
using MapEditorReborn.API;
using MapEditorReborn.API.Enums;
using MapEditorReborn.API.Features.Serializable;
using Exiled.API.Features.Doors;
using HarmonyLib;
using Mirror;
using System.Reflection.Emit;
using PlayerRoles.FirstPersonControl;
using CommandSystem;
using CommandSystem.Commands;
using RemoteAdmin;

using Maps = MapEditorReborn.Events.Handlers.Map;

namespace AdvancedMERTools
{
    public class AdvancedMERTools : Plugin<Config>
    {
        public override PluginPriority Priority => PluginPriority.Last;

        private EventManager manager;

        public static AdvancedMERTools Singleton;

        public List<HealthObject> healthObjects = new List<HealthObject> { };

        public List<InteractablePickup> InteractablePickups = new List<InteractablePickup> { };

        public List<InteractableTeleporter> InteractableTPs = new List<InteractableTeleporter> { };

        public List<CustomCollider> CustomColliders = new List<CustomCollider> { };

        public List<DummyDoor> dummyDoors = new List<DummyDoor> { };

        public List<DummyGate> dummyGates = new List<DummyGate> { };

        public Dictionary<Type, RandomExecutionModule> TypeSingletonPair = new Dictionary<Type, RandomExecutionModule> { };

        public override void OnEnabled()
        {
            Singleton = this;
            manager = new EventManager();
            Harmony harmony = new Harmony("AMERT");
            harmony.PatchAll();

            Register();
        }

        public override void OnDisabled()
        {
            UnRegister();
            manager = null;
            Singleton = null;
        }

        void Register()
        {
            Maps.LoadingMap += manager.OnLoadMap;
            MapEditorReborn.Events.Handlers.Schematic.SchematicSpawned += manager.OnSchematicLoad;
            Exiled.Events.Handlers.Map.Generated += manager.OnGen;
            //Exiled.Events.Handlers.Server.RoundStarted += manager.OnRound;
            //Exiled.Events.Handlers.Map.Decontaminating += manager.OnDecont;
            Exiled.Events.Handlers.Map.ExplodingGrenade += manager.OnGrenade;
            //Exiled.Events.Handlers.Warhead.Detonated += manager.OnAlpha;
            Exiled.Events.Handlers.Player.Shot += manager.OnShot;
            Exiled.Events.Handlers.Player.Spawned += manager.ApplyCustomSpawnPoint;
            Exiled.Events.Handlers.Player.SearchingPickup += manager.OnItemSearching;
            Exiled.Events.Handlers.Player.PickingUpItem += manager.OnItemPicked;
            Exiled.Events.Handlers.Player.InteractingDoor += manager.OnInteracted;
            Exiled.Events.Handlers.Player.Joined += manager.OnJoined;
            MapEditorReborn.Events.Handlers.Teleport.Teleporting += manager.OnTeleport;
        }

        void UnRegister()
        {
            Maps.LoadingMap -= manager.OnLoadMap;
            MapEditorReborn.Events.Handlers.Schematic.SchematicSpawned -= manager.OnSchematicLoad;
            Exiled.Events.Handlers.Map.Generated -= manager.OnGen;
            //Exiled.Events.Handlers.Server.RoundStarted -= manager.OnRound;
            //Exiled.Events.Handlers.Map.Decontaminating -= manager.OnDecont;
            Exiled.Events.Handlers.Map.ExplodingGrenade -= manager.OnGrenade;
            //Exiled.Events.Handlers.Warhead.Detonated -= manager.OnAlpha;
            Exiled.Events.Handlers.Player.Shot -= manager.OnShot;
            Exiled.Events.Handlers.Player.Spawned -= manager.ApplyCustomSpawnPoint;
            Exiled.Events.Handlers.Player.SearchingPickup -= manager.OnItemSearching;
            Exiled.Events.Handlers.Player.PickingUpItem -= manager.OnItemPicked;
            Exiled.Events.Handlers.Player.InteractingDoor -= manager.OnInteracted;
            Exiled.Events.Handlers.Player.Joined -= manager.OnJoined;
            MapEditorReborn.Events.Handlers.Teleport.Teleporting -= manager.OnTeleport;
        }

        public static ReferenceHub MakeAudio(out int id)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(NetworkManager.singleton.playerPrefab);
            ReferenceHub hub = gameObject.GetComponent<ReferenceHub>();
            try
            {
                hub.roleManager.InitializeNewRole(RoleTypeId.None, RoleChangeReason.None, RoleSpawnFlags.All, null);
            }
            catch { }
            id = new RecyclablePlayerId(true).Value;
            FakeConnection fakeConnection = new FakeConnection(id);
            NetworkServer.AddPlayerForConnection(fakeConnection, gameObject);
            MEC.Timing.CallDelayed(0.25f, () =>
            {
                try
                {
                    hub.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.All);
                }
                catch { }
            });
            return hub;
        }

        public static void ExecuteCommand(string context)
        {
            string[] array = context.Trim().Split(new char[] { ' ' }, 512, StringSplitOptions.RemoveEmptyEntries);
            ICommand command1;
            if (CommandProcessor.RemoteAdminCommandHandler.TryGetCommand(array[0], out command1))
            {
                command1.Execute(array.Segment(1), ServerConsole.Scs, out _);
            }
        }
    }

    public class FakeConnection : NetworkConnectionToClient
    {
        public FakeConnection(int connectionId) : base(connectionId)
        {

        }

        public override string address
        {
            get
            {
                return "localhost";
            }
        }

        public override void Send(ArraySegment<byte> segment, int channelId = 0)
        {
            base.Send(segment, channelId);
        }
        public override void Disconnect()
        {
            base.Disconnect();
        }
    }

    public static class EventHandler
    {
        public static Event<HealthObjectDeadEventArgs> HealthObjectDead { get; set; } = new Event<HealthObjectDeadEventArgs>();

        internal static void OnHealthObjectDead(HealthObjectDeadEventArgs ev)
        {
            EventHandler.HealthObjectDead.InvokeSafely(ev);
        }
    }

    public class EventManager
    {
        Config config = AdvancedMERTools.Singleton.Config;

        public void OnJoined(Exiled.Events.EventArgs.Player.JoinedEventArgs ev)
        {
            //identities.ForEach(x => x.gameObject.SetActive(false));
        }

        public void OnShot(Exiled.Events.EventArgs.Player.ShotEventArgs ev)
        {
            ev.RaycastHit.collider.transform.GetComponentsInParent<HealthObject>().ForEach(x => x.OnShot(ev));
        }

        public void OnInteracted(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            AdvancedMERTools.Singleton.dummyDoors.ForEach(x => x.OnInteractDoor(ev));
        }

        public void OnGrenade(Exiled.Events.EventArgs.Map.ExplodingGrenadeEventArgs ev)
        {
            //foreach (HealthObject health in AdvancedMERTools.Singleton.healthObjects)
            //{
            //    try
            //    {
            //        health.OnGrenadeExplode(ev);
            //    }
            //    catch (NullReferenceException _)
            //    {
            //        continue;
            //    }
            //}
            AdvancedMERTools.Singleton.healthObjects.ForEach(x => x.OnGrenadeExplode(ev));
        }

        public void OnItemSearching(Exiled.Events.EventArgs.Player.SearchingPickupEventArgs ev)
        {
            List<InteractablePickup> list = AdvancedMERTools.Singleton.InteractablePickups.FindAll(x => x.Pickup == ev.Pickup);
            List<Pickup> removeList = new List<Pickup> { };
            foreach (InteractablePickup interactable in list)
            {
                if (interactable.Base.InvokeType.HasFlag(InvokeType.Searching))
                {
                    interactable.RunProcess(ev.Player, ev.Pickup, out bool Remove);
                    if (interactable.Base.CancelActionWhenActive)
                    {
                        ev.IsAllowed = false;
                    }
                    if (Remove && !removeList.Contains(interactable.Pickup))
                    {
                        removeList.Add(interactable.Pickup);
                    }
                }
            }
            removeList.ForEach(x => x.Destroy());
            AdvancedMERTools.Singleton.dummyGates.ForEach(x => x.OnPickingUp(ev));
        }

        public void OnItemPicked(Exiled.Events.EventArgs.Player.PickingUpItemEventArgs ev)
        {
            List<InteractablePickup> list = AdvancedMERTools.Singleton.InteractablePickups.FindAll(x => x.Pickup == ev.Pickup);
            List<Pickup> removeList = new List<Pickup> { };
            foreach (InteractablePickup interactable in list)
            {
                if (interactable.Base.InvokeType.HasFlag(InvokeType.Picked))
                {
                    interactable.RunProcess(ev.Player, ev.Pickup, out bool Remove);
                    if (Remove && !removeList.Contains(interactable.Pickup))
                    {
                        removeList.Add(interactable.Pickup);
                    }
                }
            }
            removeList.ForEach(x => x.Destroy());
        }

        //public void OnInteracted(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        //{
        //    if (ev.IsAllowed && )
        //    {

        //    }
        //}

        public void OnLoadMap(MapEditorReborn.Events.EventArgs.LoadingMapEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            int index = config.DummyDoorInstallingMaps.IndexOf(ev.NewMap.Name);
            if (index != -1 && ev.NewMap.Doors.Count != 0)
            {
                bool flag = config.BlackListOfDummyDoor.TryGet<List<int>>(index, out List<int> element);
                for (int i = 0; i < ev.NewMap.Doors.Count; i++)
                {
                    if (flag)
                    {
                        if (element.Contains(i))
                        {
                            continue;
                        }
                    }
                    string str = "DoorLCZ";
                    switch (ev.NewMap.Doors[i].DoorType)
                    {
                        case DoorType.HeavyContainmentDoor:
                            str = "DoorHCZ";
                            break;
                        case DoorType.EntranceDoor:
                            str = "DoorEZ";
                            break;
                    }
                    SchematicObject @object = MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic(str, ev.NewMap.Doors[i].Position, Quaternion.Euler(ev.NewMap.Doors[i].Rotation), isStatic: false);
                    DummyDoor door = @object.gameObject.AddComponent<DummyDoor>();
                    door.door = ev.NewMap.Doors[i];
                    AdvancedMERTools.Singleton.dummyDoors.Add(door);
                }
            }
        }

        public void OnTeleport(MapEditorReborn.Events.EventArgs.TeleportingEventArgs ev)
        {
            List<InteractableTeleporter> ITO = AdvancedMERTools.Singleton.InteractableTPs.FindAll(x => (x.TO == ev.EntranceTeleport && x.Base.InvokeType.HasFlag(TeleportInvokeType.Enter))
            || (x.TO == ev.ExitTeleport && x.Base.InvokeType.HasFlag(TeleportInvokeType.Exit)));
            if (ITO.Count != 0 && ev.Player != null)
            {
                ITO.ForEach(x => x.RunProcess(ev.Player));
            }
        }

        public void OnSchematicLoad(MapEditorReborn.Events.EventArgs.SchematicSpawnedEventArgs ev)
        {
            if (ev.Name.Equals("Gate", StringComparison.InvariantCultureIgnoreCase))
            {
                ev.Schematic.gameObject.AddComponent<DummyGate>();
            }
            DataLoad<HODTO, HealthObject>("HealthObjects", ev);
            DataLoad<IPDTO, InteractablePickup>("Pickups", ev);
            DataLoad<CCDTO, CustomCollider>("Colliders", ev);
        }

        public void DataLoad<Tdto, Tclass>(string name, MapEditorReborn.Events.EventArgs.SchematicSpawnedEventArgs ev) where Tdto : AMERTDTO where Tclass : AMERTInteractable, new()
        {
            string path = Path.Combine(ev.Schematic.DirectoryPath, ev.Schematic.Base.SchematicName + $"-{name}.json");
            if (File.Exists(path))
            {
                List<Tdto> ts = JsonSerializer.Deserialize<List<Tdto>>(File.ReadAllText(path));
                foreach (Tdto dto in ts)
                {
                    Transform target = FindObjectWithPath(ev.Schematic.transform, dto.ObjectId);
                    Tclass tclass = target.gameObject.AddComponent<Tclass>();
                    tclass.Base = dto;
                }
            }
        }

        public static Transform FindObjectWithPath(Transform target, string pathO)
        {
            if (pathO != "")
            {
                string[] path = pathO.Split(' ');
                for (int i = path.Length - 1; i > -1; i--)
                {
                    if (target.childCount == 0 || target.childCount <= int.Parse(path[i].ToString()))
                    {
                        ServerLogs.AddLog(ServerLogs.Modules.Logger, "Advanced MER tools: Could not find appropriate child!", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
                        break;
                    }
                    target = target.GetChild(int.Parse(path[i]));
                }
            }
            return target;
        }

        public void OnGen()
        {
            AdvancedMERTools.Singleton.healthObjects.Clear();
            AdvancedMERTools.Singleton.InteractablePickups.Clear();
            AdvancedMERTools.Singleton.dummyDoors.Clear();
            AdvancedMERTools.Singleton.dummyGates.Clear();
            AdvancedMERTools.Singleton.InteractableTPs.Clear();
            AdvancedMERTools.Singleton.CustomColliders.Clear();
            //if (config.AutoRunOnEventList.Contains(Config.EventList.Generated))
            //{
            //    AutoRun();
            //}
            //foreach (NetworkIdentity identity in GameObject.FindObjectsOfType<NetworkIdentity>())
            //{
            //    if (identity.name.Equals("All"))
            //    {
            //        //ServerConsole.AddLog(identity.transform.parent.gameObject.name);
            //        //identities.Add(identity);
            //        NetworkIdentity.Destroy(identity.transform.parent.gameObject);
            //    }
            //}
        }

        List<NetworkIdentity> identities = new List<NetworkIdentity> { };

        /*public void OnRound()
        {
            if (config.AutoRunOnEventList.Contains(Config.EventList.Round))
            {
                AutoRun();
            }
        }

        public void OnDecont(Exiled.Events.EventArgs.Map.DecontaminatingEventArgs ev)
        {
            if (ev.IsAllowed && config.AutoRunOnEventList.Contains(Config.EventList.Decont))
            {
                AutoRun();
            }
        }

        public void OnAlpha()
        {
            if (config.AutoRunOnEventList.Contains(Config.EventList.Warhead))
            {
                AutoRun();
            }
        }*/

        //public void InstallDoor(DoorObject door)
        //{
        //    ServerConsole.AddLog(".");
        //}

        /*public void AutoRun()
        {
            if (!config.AutoRun) return;
            MEC.Timing.CallDelayed(config.AutoRunDelay, () =>
            {
                if (!config.AutoRunWithEveryDoor)
                {
                    int index = config.DummyDoorInstallingMaps.IndexOf(MapEditorReborn.API.API.CurrentLoadedMap.Name);
                    if (index != -1)
                    {
                        for (int i = 0; i < MapEditorReborn.API.API.CurrentLoadedMap.Doors.Count; i++)
                        {
                            if (config.BlackListOfDummyDoor[index].Contains(i))
                            {
                                continue;
                            }
                            string str = "DoorLCZ";
                            switch (MapEditorReborn.API.API.CurrentLoadedMap.Doors[i].DoorType)
                            {
                                case DoorType.HeavyContainmentDoor:
                                    str = "DoorHCZ";
                                    break;
                                case DoorType.EntranceDoor:
                                    str = "DoorEZ";
                                    break;
                            }
                            SchematicObject @object = MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic(str, MapEditorReborn.API.API.CurrentLoadedMap.Doors[i].Position, Quaternion.Euler(MapEditorReborn.API.API.CurrentLoadedMap.Doors[i].Rotation));
                            DummyDoor door = @object.gameObject.AddComponent<DummyDoor>();
                            door.door = MapEditorReborn.API.API.CurrentLoadedMap.Doors[i];
                            AdvancedMERTools.Singleton.dummyDoors.Add(door);
                        }
                    }
                }
                else
                {
                    foreach (DoorLinkingRooms door in GameObject.FindObjectsOfType<DoorLinkingRooms>())
                    {
                        if (door.gameObject.TryGetComponent(out Interactables.Interobjects.BasicDoor basicDoor) && basicDoor.Rooms.Length == 0)
                        {
                            string str = "DoorLCZ";
                            if (door.gameObject.name.Contains("HCZ")) str = "DoorHCZ";
                            if (door.gameObject.name.Contains("EZ")) str = "DoorEZ";
                            SchematicObject @object = MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic(str, basicDoor.transform.position, basicDoor.transform.rotation);
                            DummyDoor dummy = @object.gameObject.AddComponent<DummyDoor>();
                            AdvancedMERTools.Singleton.dummyDoors.Add(dummy);
                            dummy.RealDoor = Door.Get(basicDoor);
                        }
                    }
                }
            });
        }*/

        public void ApplyCustomSpawnPoint(Exiled.Events.EventArgs.Player.SpawnedEventArgs ev)
        {
            if (API.CurrentLoadedMap == null)
            {
                return;
            }
            if (!config.CustomSpawnPointEnable)
            {
                return;
            }
            List<PlayerSpawnPointSerializable> list = API.CurrentLoadedMap.PlayerSpawnPoints.FindAll(x => keyValuePairs.TryGetValue(ev.Player.RoleManager.CurrentRole.RoleTypeId, out SpawnableTeam team) ? x.SpawnableTeam == team : false);
            if (list.Count != 0)
            {
                PlayerSpawnPointSerializable serializable = list.RandomItem();
                ev.Player.Teleport(API.GetRelativePosition(serializable.Position, API.GetRandomRoom(serializable.RoomType)));
            }
        }

        public static Dictionary<RoleTypeId, SpawnableTeam> keyValuePairs = new Dictionary<RoleTypeId, SpawnableTeam>
        {
            { RoleTypeId.ChaosConscript, SpawnableTeam.Chaos },
            { RoleTypeId.ChaosMarauder, SpawnableTeam.Chaos },
            { RoleTypeId.ChaosRepressor, SpawnableTeam.Chaos },
            { RoleTypeId.ChaosRifleman, SpawnableTeam.Chaos },
            { RoleTypeId.ClassD, SpawnableTeam.ClassD },
            { RoleTypeId.FacilityGuard, SpawnableTeam.FacilityGuard },
            { RoleTypeId.NtfCaptain, SpawnableTeam.MTF },
            { RoleTypeId.NtfPrivate, SpawnableTeam.MTF },
            { RoleTypeId.NtfSergeant, SpawnableTeam.MTF },
            { RoleTypeId.NtfSpecialist, SpawnableTeam.MTF },
            { RoleTypeId.Scientist, SpawnableTeam.Scientist },
            { RoleTypeId.Scp049, SpawnableTeam.Scp049 },
            { RoleTypeId.Scp0492, SpawnableTeam.Scp0492 },
            { RoleTypeId.Scp096, SpawnableTeam.Scp096 },
            { RoleTypeId.Scp106, SpawnableTeam.Scp106 },
            { RoleTypeId.Scp173, SpawnableTeam.Scp173 },
            { RoleTypeId.Scp939, SpawnableTeam.Scp939 },
            { RoleTypeId.Tutorial, SpawnableTeam.Tutorial }
        };
    }
}
