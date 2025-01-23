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
        protected virtual void Start()
        {
            this.Base = base.Base as IPDTO;
            Pickup = Pickup.Get(this.gameObject);
            if (Pickup != null)
            {
                AdvancedMERTools.Singleton.InteractablePickups.Add(this);
            }
            else
            {
                Destroy(this);
            }
        }

        public virtual void RunProcess(Player player, Pickup pickup, out bool Remove)
        {
            bool r = false;
            Remove = false;
            if (pickup != this.Pickup || !Active)
            {
                return;
            }
            ModuleGeneralArguments args = new ModuleGeneralArguments { interpolations = Formatter, interpolationsList = new object[] { player }, player = player, schematic = OSchematic, transform = this.transform, TargetCalculated = false };
            var ipActionExecutors = new Dictionary<IPActionType, Action>
{
    { IPActionType.Disappear, () => r = true },
    { IPActionType.Explode, () => ExplodeModule.Execute(Base.ExplodeModules, args) },
    { IPActionType.PlayAnimation, () => AnimationDTO.Execute(Base.AnimationModules, args) },
    { IPActionType.Warhead, () => AlphaWarhead(Base.warheadActionType) },
    { IPActionType.SendMessage, () => MessageModule.Execute(Base.MessageModules, args) },
    { IPActionType.DropItems, () => DropItem.Execute(Base.dropItems, args) },
    { IPActionType.SendCommand, () => Commanding.Execute(Base.commandings, args) },
    { IPActionType.UpgradeItem, () =>
        {
            if (player.GameObject.TryGetComponent<Collider>(out Collider col))
            {
                var vs = Enumerable.Range(0, 5)
                    .Where(j => Base.Scp914Mode.HasFlag((Scp914Mode)j))
                    .ToList();
                Scp914.Scp914Upgrader.Upgrade(
                    new Collider[] { col },
                    Scp914.Scp914Mode.Held,
                    (Scp914.Scp914KnobSetting)vs.RandomItem()
                );
            }
        }
    },
                { IPActionType.GiveEffect, () => EffectGivingModule.Execute(Base.effectGivingModules, args) },
                { IPActionType.PlayAudio, () => AudioModule.Execute(Base.AudioModules, args) },
                { IPActionType.CallGroovieNoise, () => CGNModule.Execute(Base.GroovieNoiseToCall, args) },
                { IPActionType.CallFunction, () => CFEModule.Execute(Base.FunctionToCall, args) }
};
            foreach (IPActionType type in Enum.GetValues(typeof(IPActionType)))
            {
                if (Base.ActionType.HasFlag(type) && ipActionExecutors.TryGetValue(type, out var execute))
                {
                    execute();
                }
            }
            Remove = r;
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
    }

    public class FInteractablePickup : InteractablePickup
    {
        protected override void Start()
        {
            this.Base = ((AMERTInteractable)this).Base as FIPDTO;
            Pickup = Pickup.Get(this.gameObject);
            if (Pickup != null)
            {
                AdvancedMERTools.Singleton.InteractablePickups.Add(this);
            }
            else
            {
                Destroy(this);
            }
        }

        public override void RunProcess(Player player, Pickup pickup, out bool Remove)
        {
            bool r = false;
            Remove = false;
            if (pickup != this.Pickup || !Active)
            {
                return;
            }
            FunctionArgument args = new FunctionArgument(this, player);
            var ipActionExecutors = new Dictionary<IPActionType, Action>
{
    { IPActionType.Disappear, () => r = true },
    { IPActionType.Explode, () => FExplodeModule.Execute(Base.ExplodeModules, args) },
    { IPActionType.PlayAnimation, () => FAnimationDTO.Execute(Base.AnimationModules, args) },
    { IPActionType.Warhead, () => AlphaWarhead(Base.warheadActionType.GetValue<WarheadActionType>(args, 0)) },
    { IPActionType.SendMessage, () => FMessageModule.Execute(Base.MessageModules, args) },
    { IPActionType.DropItems, () => FDropItem.Execute(Base.dropItems, args) },
    { IPActionType.SendCommand, () => FCommanding.Execute(Base.commandings, args) },
    { IPActionType.UpgradeItem, () =>
        {
            if (player.GameObject.TryGetComponent<Collider>(out Collider col))
            {
                Scp914Mode mode = Base.Scp914Mode.GetValue<Scp914Mode>(args, 0);
                var vs = Enumerable.Range(0, 5)
                    .Where(j => mode.HasFlag((Scp914Mode)j))
                    .ToList();
                Scp914.Scp914Upgrader.Upgrade(
                    new Collider[] { col },
                    Scp914.Scp914Mode.Held,
                    (Scp914.Scp914KnobSetting)vs.RandomItem()
                );
            }
        }
    },
    { IPActionType.GiveEffect, () => FEffectGivingModule.Execute(Base.effectGivingModules, args) },
    { IPActionType.PlayAudio, () => FAudioModule.Execute(Base.AudioModules, args) },
    { IPActionType.CallGroovieNoise, () => FCGNModule.Execute(Base.GroovieNoiseToCall, args) },
    { IPActionType.CallFunction, () => FCFEModule.Execute(Base.FunctionToCall, args) }
};
            foreach (IPActionType type in Enum.GetValues(typeof(IPActionType)))
            {
                if (Base.ActionType.HasFlag(type) && ipActionExecutors.TryGetValue(type, out var execute))
                {
                    execute();
                }
            }
            Remove = r;
        }

        void OnDestroy()
        {
            AdvancedMERTools.Singleton.InteractablePickups.Remove(this);
        }

        public new FIPDTO Base;
    }
}
