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
using MapEditorReborn.API.Features;
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
using System.Reflection;
using PlayerRoles.FirstPersonControl;
using CommandSystem;
using CommandSystem.Commands;
using RemoteAdmin;

using Maps = MapEditorReborn.Events.Handlers.Map;

namespace AdvancedMERTools
{
    [HarmonyPatch(typeof(DoorObject), nameof(DoorObject.Init))]
    public class DoorSpawnPatcher
    {
        static void Prefix(DoorObject __instance)
        {
            if (AdvancedMERTools.Singleton.Config.AutoRun && __instance.gameObject.TryGetComponent(out Interactables.Interobjects.BasicDoor basicDoor) && basicDoor.Rooms.Length == 0)
            {
                string str = "DoorLCZ";
                if (__instance.gameObject.name.Contains("HCZ")) str = "DoorHCZ";
                if (__instance.gameObject.name.Contains("EZ")) str = "DoorEZ";
                SchematicObject @object = ObjectSpawner.SpawnSchematic(str, basicDoor.transform.position, basicDoor.transform.rotation, isStatic: false);
                DummyDoor dummy = @object.gameObject.AddComponent<DummyDoor>();
                AdvancedMERTools.Singleton.dummyDoors.Add(dummy);
                dummy.RealDoor = Door.Get(basicDoor);
            }
        }
    }

    [HarmonyPatch(typeof(MapUtils), nameof(MapUtils.LoadMap), new Type[] { typeof(MapSchematic) })]
    public class MapLoadingPatcher
    {
        static void Postfix(MapSchematic map)
        {
            string path = Path.Combine(MapEditorReborn.MapEditorReborn.MapsDir, map.Name + "-ITeleporters.json");
            if (File.Exists(path))
            {
                List<ITDTO> iTDTOs = JsonSerializer.Deserialize<List<ITDTO>>(File.ReadAllText(path));
                TeleportObject[] teleports = API.SpawnedObjects.Where(x => x is TeleportObject).Cast<TeleportObject>().ToArray();
                foreach (ITDTO to in iTDTOs)
                {
                    int n = int.Parse(to.ObjectId);
                    if (n > 0 && n <= teleports.Length)
                    {
                        InteractableTeleporter interactable = teleports[n - 1].gameObject.AddComponent<InteractableTeleporter>();
                        interactable.Base = to;
                    }
                }
            }
        }
    }
}