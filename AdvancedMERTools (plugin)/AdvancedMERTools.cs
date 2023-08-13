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
using Exiled.Events.Extensions;
using Exiled.Events;

using Maps = MapEditorReborn.Events.Handlers.Map;

namespace AdvancedMERTools
{
    public class AdvancedMERTools : Plugin<Config>
    {
        private EventManager manager;

        public static AdvancedMERTools Singleton;

        public List<HealthObject> healthObjects = new List<HealthObject> { };

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
        }
    }

    public static class EventHandler
    {
        public static event Events.CustomEventHandler<HealthObjectDeadEventArgs> HealthObjectDead;

        internal static void OnHealthObjectDead(HealthObjectDeadEventArgs ev)
        {
            EventHandler.HealthObjectDead.InvokeSafely(ev);
        }
    }

    [Serializable]
    public class HealthObjectDTO
    {
        public float Health;
        public int ArmorEfficient;
        public DeadType DeadType;
        public string ObjectId;
    }

    [Serializable]
    public enum DeadType
    {
        Disappear,
        GetRigidbody,
        DynamicDisappearing,
        Explode,
        ResetHP
    }

    //[Serializable]
    //public class DoorInstallingGuideDTO
    //{
    //    public float Health;
    //    public DoorDamageType DamagableDamageType;
    //    public KeycardPermissions KeycardPermissions;
    //    public string ObjectId;
    //}

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
                    Transform target = ev.Schematic.transform;
                    if (dTO.ObjectId != "")
                    {
                        for (int i = dTO.ObjectId.Length - 1; i > -1; i--)
                        {
                            target = target.GetChild(int.Parse(dTO.ObjectId[i].ToString()));
                        }
                    }
                    target.gameObject.AddComponent<HealthObject>().Base = dTO;
                }
            }
            /*path = Path.Combine(ev.Schematic.DirectoryPath, ev.Schematic.Base.SchematicName + "-DoorInstalling.json");
            if (File.Exists(path))
            {
                List<HealthObjectDTO> healthObjectDTOs = JsonSerializer.Deserialize<List<HealthObjectDTO>>(File.ReadAllText(path));
                foreach (HealthObjectDTO dTO in healthObjectDTOs)
                {
                    Transform target = ev.Schematic.transform;
                    if (dTO.ObjectId != "")
                    {
                        for (int i = dTO.ObjectId.Length - 1; i > -1; i--)
                        {
                            target = target.GetChild(int.Parse(dTO.ObjectId[i].ToString()));
                        }
                    }
                    target.gameObject.AddComponent<HealthObject>().Base = dTO;
                }
            }*/
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
                        if (door.gameObject.TryGetComponent<BasicDoor>(out BasicDoor basicDoor) && basicDoor.Rooms.Length == 0)
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
    }

    public class HealthObjectDeadEventArgs : EventArgs, Exiled.Events.EventArgs.Interfaces.IExiledEvent
    {
        public HealthObjectDeadEventArgs(HealthObjectDTO healthObject, Player Attacker)
        {
            HealthObject = healthObject;
            Killer = Attacker;
        }

        public HealthObjectDTO HealthObject;
        public Player Killer;
    }
}
