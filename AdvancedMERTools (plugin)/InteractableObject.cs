using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Items;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features;
using Exiled.CustomItems;
using CommandSystem;
using Utf8Json.Formatters;
using RemoteAdmin;
using Exiled.CustomItems.API.Features;
using UserSettings.ServerSpecific;

namespace AdvancedMERTools
{
    public class InteractableObject : AMERTInteractable
    {
        void Start()
        {
            this.Base = base.Base as IODTO;
            AdvancedMERTools.Singleton.interactableObjects.Add(this);
            if (AdvancedMERTools.Singleton.IOkeys.ContainsKey(Base.InputKeyCode))
            {
                AdvancedMERTools.Singleton.IOkeys[Base.InputKeyCode].Add(this);
            }
            else
            {
                ServerSpecificSettingsSync.DefinedSettings = ServerSpecificSettingsSync.DefinedSettings.Append(new SSKeybindSetting(null, "AMERT - Interactable Object", (KeyCode)Base.InputKeyCode, true, "")).ToArray();
                ServerSpecificSettingsSync.SendToAll();
                AdvancedMERTools.Singleton.IOkeys.Add(Base.InputKeyCode, new List<InteractableObject> { this });
            }
        }

        public void RunProcess(Player player)
        {
            if (!Active)
                return;
            foreach (IPActionType type in Enum.GetValues(typeof(IPActionType)))
            {
                if (Base.ActionType.HasFlag(type))
                {
                    switch (type)
                    {
                        case IPActionType.Disappear:
                            Destroy(this.gameObject, 0.1f);
                            break;
                        case IPActionType.Explode:
                            ExplodeModule.GetSingleton<ExplodeModule>().Execute(ExplodeModule.SelectList<ExplodeModule>(Base.ExplodeModules), this.transform, player.ReferenceHub);
                            break;
                        case IPActionType.PlayAnimation:
                            if (modules.Count == 0)
                            {
                                modules = AnimationModule.GetModules(Base.AnimationModules, this.gameObject);
                                if (modules.Count == 0)
                                {
                                    ServerConsole.AddLog("For some reason, it was failed to load animation file.");
                                    break;
                                }
                            }
                            AnimationModule.GetSingleton<AnimationModule>().Execute(AnimationModule.SelectList<AnimationModule>(modules));
                            break;
                        case IPActionType.Warhead:
                            foreach (WarheadActionType warhead in Enum.GetValues(typeof(WarheadActionType)))
                            {
                                if (Base.warheadActionType.HasFlag(warhead))
                                {
                                    switch (warhead)
                                    {
                                        case WarheadActionType.Start:
                                            Warhead.Start();
                                            break;
                                        case WarheadActionType.Stop:
                                            Warhead.Stop();
                                            break;
                                        case WarheadActionType.Lock:
                                            Warhead.IsLocked = true;
                                            break;
                                        case WarheadActionType.UnLock:
                                            Warhead.IsLocked = false;
                                            break;
                                        case WarheadActionType.Disable:
                                            Warhead.LeverStatus = false;
                                            break;
                                        case WarheadActionType.Enable:
                                            Warhead.LeverStatus = true;
                                            break;
                                    }
                                }
                            }
                            break;
                        case IPActionType.SendMessage:
                            MessageModule.GetSingleton<MessageModule>().Execute(MessageModule.SelectList<MessageModule>(Base.MessageModules), Formatter, player);
                            break;
                        case IPActionType.DropItems:
                            DropItem.GetSingleton<DropItem>().Execute(DropItem.SelectList<DropItem>(Base.dropItems), this.transform);
                            break;
                        case IPActionType.SendCommand:
                            Commanding.GetSingleton<Commanding>().Execute(Commanding.SelectList<Commanding>(Base.commandings), Formatter, player);
                            break;
                        case IPActionType.UpgradeItem:
                            if (player.GameObject.TryGetComponent<Collider>(out Collider col))
                            {
                                List<int> vs = new List<int> { };
                                for (int j = 0; j < 5; j++)
                                {
                                    if (Base.Scp914Mode.HasFlag((Scp914Mode)j))
                                    {
                                        vs.Add(j);
                                    }
                                }
                                Scp914.Scp914Upgrader.Upgrade(new Collider[] { col }, Scp914.Scp914Mode.Held, (Scp914.Scp914KnobSetting)vs.RandomItem());
                            }
                            break;
                        case IPActionType.GiveEffect:
                            EffectGivingModule.GetSingleton<EffectGivingModule>().Execute(EffectGivingModule.SelectList<EffectGivingModule>(Base.effectGivingModules), player);
                            break;
                        case IPActionType.PlayAudio:
                            AudioModule.GetSingleton<AudioModule>().Execute(AudioModule.SelectList<AudioModule>(Base.AudioModules), this.transform);
                            break;
                        case IPActionType.CallGroovieNoise:
                            CGNModule.GetSingleton<CGNModule>().Execute(CGNModule.SelectList<CGNModule>(Base.GroovieNoiseToCall), OSchematic);
                            break;
                    }
                }
            }
        }

        void OnDestroy()
        {
            AdvancedMERTools.Singleton.interactableObjects.Remove(this);
        }

        static readonly Dictionary<string, Func<object[], string>> Formatter = new Dictionary<string, Func<object[], string>>
        {
            { "{p_i}", vs => (vs[0] as Player).Id.ToString() },
            { "{p_name}", vs => (vs[0] as Player).Nickname.ToString() },
            { "{p_pos}", vs => { Vector3 pos = (vs[0] as Player).Transform.position; return string.Format("{0} {1} {2}", pos.x, pos.y, pos.z); } },
            { "{p_room}", vs => (vs[0] as Player).CurrentRoom.RoomName.ToString() },
            { "{p_zone}", vs => (vs[0] as Player).Zone.ToString() },
            { "{p_role}", vs => (vs[0] as Player).Role.Type.ToString() },
            { "{p_item}", vs => (vs[0] as Player).CurrentItem.Type.ToString() }
        };

        public new IODTO Base;

        public List<AnimationModule> modules = new List<AnimationModule> { };
    }
}
