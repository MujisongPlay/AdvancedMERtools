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
using Utf8Json.Formatters;

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
                //SchematicObject @object = ObjectSpawner.SpawnSchematic(str, basicDoor.transform.position, basicDoor.transform.rotation, isStatic: false);
                SchematicObject @object = ObjectSpawner.SpawnSchematic(str, basicDoor.transform.position, basicDoor.transform.rotation, basicDoor.transform.localScale, null);
                DummyDoor dummy = @object.gameObject.AddComponent<DummyDoor>();
                AdvancedMERTools.Singleton.dummyDoors.Add(dummy);
                dummy.RealDoor = Door.Get(basicDoor);
            }
        }
    }

    [HarmonyPatch(nameof(DoorVariant), nameof(DoorVariant.NetworkTargetState), MethodType.Setter)]
    public class DoorVariantPatcher
    {
        static void Prefix(DoorVariant __instance, bool value)
        {
            DummyDoor d = AdvancedMERTools.Singleton.dummyDoors.Find(x => x.RealDoor == Door.Get(__instance));
            if (d != null)
                d.OnInteractDoor(value);
        }
    }

    [HarmonyPatch(nameof(DoorVariant), nameof(DoorVariant.NetworkActiveLocks), MethodType.Setter)]
    public class DoorVariantLockPatcher
    {
        static void Prefix(DoorVariant __instance, ushort value)
        {
            CustomDoor d = AdvancedMERTools.Singleton.customDoors.Find(x => x.door == Door.Get(__instance));
            if (d != null)
                d.OnLockChange(value);
        }
    }

    //[HarmonyPatch(typeof(MapUtils), nameof(MapUtils.LoadMap), new Type[] { typeof(MapSchematic) })]
    //public class MapLoadingPatcher
    //{
    //    static void Postfix(MapSchematic map)
    //    {
    //        if (map == null || !map.IsValid)
    //            return;
    //        string path = Path.Combine(MapEditorReborn.MapEditorReborn.MapsDir, map.Name + "-ITeleporters.json");
    //        if (File.Exists(path))
    //        {
    //            List<ITDTO> iTDTOs = JsonSerializer.Deserialize<List<ITDTO>>(File.ReadAllText(path));
    //            TeleportObject[] teleports = API.SpawnedObjects.Where(x => x is TeleportObject).Cast<TeleportObject>().ToArray();
    //            foreach (ITDTO to in iTDTOs)
    //            {
    //                int n = int.Parse(to.ObjectId);
    //                if (n > 0 && n <= teleports.Length)
    //                {
    //                    InteractableTeleporter interactable = teleports[n - 1].gameObject.AddComponent<InteractableTeleporter>();
    //                    interactable.Base = to;
    //                }
    //            }
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(ServerListFormatter), nameof(ServerListFormatter.Serialize))]
    //public class PlayerCountPatcher
    //{
    //    static void Prefix(ref JsonWriter writer, ServerListItem value, IJsonFormatterResolver formatterResolver)
    //    {
    //        if (AdvancedMERTools.Singleton.Config.UseExperimentalFeature)
    //        {
    //            if (int.TryParse(value.players, out int result))
    //                value = new ServerListItem(value.serverId, value.ip, value.port, (result + 1 - ReferenceHub.AllHubs.Count(x => x.Mode == CentralAuth.ClientInstanceMode.DedicatedServer)).ToString(), value.info, value.pastebin, value.version, value.friendlyFire, value.modded, value.whitelist, value.officialCode);
    //            else
    //                ServerConsole.AddLog("Send folloing message to me. - Mujishung: " + value.players);
    //        }
    //    }
    //}
}
