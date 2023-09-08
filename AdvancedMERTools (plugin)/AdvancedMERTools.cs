using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.Loader;
using UnityEngine;
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

using Maps = MapEditorReborn.Events.Handlers.Map;

namespace AdvancedMERTools
{
    public class AdvancedMERTools : Plugin<Config>
    {
        private EventManager manager;

        public static AdvancedMERTools Singleton;

        public List<HealthObject> healthObjects = new List<HealthObject> { };

        public List<InteractablePickup> InteractablePickups = new List<InteractablePickup> { };

        public override void OnEnabled()
        {
            Singleton = this;
            manager = new EventManager();

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
            Exiled.Events.Handlers.Server.RoundStarted += manager.OnRound;
            Exiled.Events.Handlers.Map.Decontaminating += manager.OnDecont;
            Exiled.Events.Handlers.Map.ExplodingGrenade += manager.OnGrenade;
            Exiled.Events.Handlers.Warhead.Detonated += manager.OnAlpha;
            Exiled.Events.Handlers.Player.Shot += manager.OnShot;
            Exiled.Events.Handlers.Player.Spawned += manager.ApplyCustomSpawnPoint;
            Exiled.Events.Handlers.Player.PickingUpItem += manager.OnItemPicked;
        }

        void UnRegister()
        {
            Maps.LoadingMap -= manager.OnLoadMap;
            MapEditorReborn.Events.Handlers.Schematic.SchematicSpawned -= manager.OnSchematicLoad;
            Exiled.Events.Handlers.Map.Generated -= manager.OnGen;
            Exiled.Events.Handlers.Server.RoundStarted -= manager.OnRound;
            Exiled.Events.Handlers.Map.Decontaminating -= manager.OnDecont;
            Exiled.Events.Handlers.Map.ExplodingGrenade -= manager.OnGrenade;
            Exiled.Events.Handlers.Warhead.Detonated -= manager.OnAlpha;
            Exiled.Events.Handlers.Player.Shot -= manager.OnShot;
            Exiled.Events.Handlers.Player.Spawned -= manager.ApplyCustomSpawnPoint;
            Exiled.Events.Handlers.Player.PickingUpItem -= manager.OnItemPicked;
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

        public void OnShot(Exiled.Events.EventArgs.Player.ShotEventArgs ev)
        {
            ev.RaycastHit.collider.transform.GetComponentsInParent<HealthObject>().ForEach(x => x.OnShot(ev));
        }

        public void OnGrenade(Exiled.Events.EventArgs.Map.ExplodingGrenadeEventArgs ev)
        {
            AdvancedMERTools.Singleton.healthObjects.ForEach(x => x.OnGrenadeExplode(ev));
        }

        public void OnItemPicked(Exiled.Events.EventArgs.Player.PickingUpItemEventArgs ev)
        {
            AdvancedMERTools.Singleton.InteractablePickups.ForEach(x => x.OnInteracted(ev));
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
                    SchematicObject @object = MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic(str, ev.NewMap.Doors[i].Position, Quaternion.Euler(ev.NewMap.Doors[i].Rotation));
                    @object.gameObject.AddComponent<DummyDoor>().door = ev.NewMap.Doors[i];
                }
            }
        }

        public void OnSchematicLoad(MapEditorReborn.Events.EventArgs.SchematicSpawnedEventArgs ev)
        {
            string path = Path.Combine(ev.Schematic.DirectoryPath, ev.Schematic.Base.SchematicName + "-HealthObjects.json");
            if (File.Exists(path))
            {
                List<HealthObjectDTO> healthObjectDTOs = JsonSerializer.Deserialize<List<HealthObjectDTO>>(File.ReadAllText(path));
                foreach (HealthObjectDTO dTO in healthObjectDTOs)
                {
                    Transform target = FindObjectWithPath(ev.Schematic.transform, dTO.ObjectId);
                    HealthObject health = target.gameObject.AddComponent<HealthObject>();
                    health.Base = dTO;
                    if (dTO.DeadType == DeadType.PlayAnimation)
                    {
                        target = FindObjectWithPath(ev.Schematic.transform, dTO.ObjectId);
                        if (target.TryGetComponent<Animator>(out Animator animator))
                        {
                            health.animator = animator;
                        }
                    }
                }
            }
            path = Path.Combine(ev.Schematic.DirectoryPath, ev.Schematic.Base.SchematicName + "-Pickups.json");
            if (File.Exists(path))
            {
                List<IPDTO> healthObjectDTOs = JsonSerializer.Deserialize<List<IPDTO>>(File.ReadAllText(path));
                foreach (IPDTO dTO in healthObjectDTOs)
                {
                    Transform target = FindObjectWithPath(ev.Schematic.transform, dTO.ObjectId);
                    InteractablePickup health = target.gameObject.AddComponent<InteractablePickup>();
                    health.Base = dTO;
                    if (dTO.ActionType == ActionType.PlayAnimation)
                    {
                        target = FindObjectWithPath(ev.Schematic.transform, dTO.ObjectId);
                        if (target.TryGetComponent<Animator>(out Animator animator))
                        {
                            health.animator = animator;
                        }
                    }
                }
            }
        }

        public static Transform FindObjectWithPath(Transform target, string path)
        {
            if (path != "")
            {
                for (int i = path.Length - 1; i > -1; i--)
                {
                    if (target.childCount == 0 || target.childCount <= int.Parse(path[i].ToString()))
                    {
                        ServerLogs.AddLog(ServerLogs.Modules.Logger, "Advanced MER tools: Could not find appropriate child!", ServerLogs.ServerLogType.RemoteAdminActivity_Misc);
                        break;
                    }
                    target = target.GetChild(int.Parse(path[i].ToString()));
                }
            }
            return target;
        }

        public void OnGen()
        {
            if (config.AutoRunOnEventList.Contains(Config.EventList.Generated))
            {
                AutoRun();
            }
        }

        public void OnRound()
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
        }

        //public void InstallDoor(DoorObject door)
        //{
        //    ServerConsole.AddLog(".");
        //}

        public void AutoRun()
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
                            @object.gameObject.AddComponent<DummyDoor>().door = MapEditorReborn.API.API.CurrentLoadedMap.Doors[i];
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
                            @object.gameObject.AddComponent<DummyDoor>().RealDoor = Door.Get(basicDoor);
                        }
                    }
                }
            });
        }

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
