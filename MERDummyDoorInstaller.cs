using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.Loader;
using UnityEngine;

using Maps = MapEditorReborn.Events.Handlers.Map;

namespace MERDummyDoorInstaller
{
    public class MERDummyDoorInstaller : Plugin<Config>
    {
        private EventManager manager;

        public static MERDummyDoorInstaller Singleton;

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
        }

        void UnRegister()
        {
            Maps.LoadingMap -= manager.OnLoadMap;
        }
    }

    public class EventManager
    {
        Config config = MERDummyDoorInstaller.Singleton.Config;

        public void OnLoadMap(MapEditorReborn.Events.EventArgs.LoadingMapEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            int index = config.DummyDoorInstallingMaps.IndexOf(ev.NewMap.Name);
            if (index != -1)
            {
                for (int i = 0; i < ev.NewMap.Doors.Count; i++)
                {
                    if (config.BlackListOfDummyDoor[index].Contains(i))
                    {
                        continue;
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
                    MapEditorReborn.API.Features.Objects.SchematicObject @object = MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic(str, ev.NewMap.Doors[i].Position, Quaternion.Euler(ev.NewMap.Doors[i].Rotation));
                    @object.gameObject.AddComponent<DummyDoor>().door = ev.NewMap.Doors[i];
                }
            }
        }
    }
}
