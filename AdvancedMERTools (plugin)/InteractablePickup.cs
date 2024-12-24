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

namespace AdvancedMERTools
{
    public class InteractablePickup : AMERTInteractable
    {
        void Start()
        {
            this.Base = base.Base as IPDTO;
            Pickup = Pickup.Get(this.gameObject);
            if (Pickup != null)
            {
                AdvancedMERTools.Singleton.InteractablePickups.Add(this);
                AdvancedMERTools.Singleton.codeClassPair.Add(Base.Code, this);
            }
            else
            {
                Destroy(this);
            }
        }

        public void RunProcess(Player player, Pickup pickup, out bool Remove)
        {
            Remove = false;
            if (pickup != this.Pickup)
            {
                return;
            }
            foreach (IPActionType type in Enum.GetValues(typeof(IPActionType)))
            {
                if (Base.ActionType.HasFlag(type))
                {
                    switch (type)
                    {
                        case IPActionType.Disappear:
                            Remove = true;
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
                            MessageModule.GetSingleton<MessageModule>().Execute(MessageModule.SelectList<MessageModule>(Base.MessageModules), Formatter, player, pickup);
                            break;
                        case IPActionType.DropItems:
                            DropItem.GetSingleton<DropItem>().Execute(DropItem.SelectList<DropItem>(Base.dropItems), this.transform);
                            break;
                        case IPActionType.SendCommand:
                            Commanding.GetSingleton<Commanding>().Execute(Commanding.SelectList<Commanding>(Base.commandings), Formatter, player, pickup);
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
                    }
                }
            }
        }

        void OnDestroy()
        {
            AdvancedMERTools.Singleton.InteractablePickups.Remove(this);
        }

        static readonly Dictionary<string, Func<object[], string>> Formatter = new Dictionary<string, Func<object[], string>>
        {
            { "{p_i}", vs => (vs[0] as Player).Id.ToString() },
            { "{p_name}", vs => (vs[0] as Player).Nickname.ToString() },
            { "{p_pos}", vs => { Vector3 pos = (vs[0] as Player).Transform.position; return string.Format("{0} {1} {2}", pos.x, pos.y, pos.z); } },
            { "{p_room}", vs => (vs[0] as Player).CurrentRoom.RoomName.ToString() },
            { "{p_zone}", vs => (vs[0] as Player).Zone.ToString() },
            { "{p_role}", vs => (vs[0] as Player).Role.Type.ToString() },
            { "{p_item}", vs => (vs[0] as Player).CurrentItem.Type.ToString() },
            { "{o_pos}", vs => { Vector3 pos = (vs[1] as Pickup).Transform.position; return string.Format("{0} {1} {2}", pos.x, pos.y, pos.z); } },
            { "{o_room}", vs => (vs[1] as Pickup).Room.RoomName.ToString() },
            { "{o_zone}", vs => (vs[1] as Pickup).Room.Zone.ToString() }
        };

        public Pickup Pickup;

        public new IPDTO Base;

        public List<AnimationModule> modules = new List<AnimationModule> { };
    }
}
