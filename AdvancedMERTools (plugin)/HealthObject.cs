using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features.Items;
using InventorySystem.Items.Armor;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using RemoteAdmin;
using CommandSystem.Commands;
using CommandSystem;
using MapEditorReborn.API.Features.Objects;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.ThrowableProjectiles;
using AdminToys;

namespace AdvancedMERTools
{
    public class HealthObject : AMERTInteractable
    {
        private void Start()
        {
            this.Base = base.Base as HODTO;
            Health = Base.Health;
            AdvancedMERTools.Singleton.healthObjects.Add(this);
        }

        public void OnShot(Exiled.Events.EventArgs.Player.ShotEventArgs ev)
        {
            if (!ev.CanHurt || ev.Player == null || !IsAlive || !(ev.Player.CurrentItem is Firearm)) return;
            if (Base.whitelistWeapons.Count != 0)
            {
                if (CustomItem.TryGet(ev.Player.CurrentItem, out CustomItem custom))
                {
                    if (Base.whitelistWeapons.Find(x => x.CustomItemId == custom.Id) == null) return;
                }
                else
                {
                    if (Base.whitelistWeapons.Find(x => x.CustomItemId == 0 && x.ItemType == ev.Player.CurrentItem.Type) == null) return;
                }
            }
            Firearm firearm = ev.Player.CurrentItem as Firearm;
            float damage = BodyArmorUtils.ProcessDamage(Base.ArmorEfficient, firearm.Base.BaseStats.DamageAtDistance(firearm.Base, ev.Distance), Mathf.RoundToInt(firearm.Base.ArmorPenetration * 100f));
            if (firearm.Type == ItemType.GunShotgun && firearm.Base is InventorySystem.Items.Firearms.Shotgun)
            {
                //damage /= (firearm.Base.ActionModule as PumpAction).LastFiredAmount * 8;
                //FieldInfo info = typeof(BuckshotHitreg).GetField("_buckshotSettingsProvider", BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                //damage /= ((BuckshotHitreg.BuckshotSettings)info.GetValue(ev.Firearm.Base.HitregModule as BuckshotHitreg)).MaxHits;
                PropertyInfo info = typeof(BuckshotHitreg).GetProperty("CurBuckshotSettings", BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                damage /= (float)((BuckshotHitreg.BuckshotSettings)info.GetValue(ev.Firearm.Base.HitregModule as BuckshotHitreg)).MaxHits;
            }
            Health -= damage;
            Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);
            CheckDead(ev.Player, damage);
        }

        public void OnGrenadeExplode(Exiled.Events.EventArgs.Map.ExplodingGrenadeEventArgs ev)
        {
            if (!ev.IsAllowed || !IsAlive) return;
            if (Base.whitelistWeapons.Count != 0 && Base.whitelistWeapons.Find(x => x.CustomItemId == 0 && x.ItemType == ItemType.GrenadeHE) == null) return;
            if (ev.Projectile.Type == ItemType.GrenadeHE)
            {
                float Dis = Vector3.Distance(this.transform.position, ev.Position);
                FieldInfo info = typeof(ExplosionGrenade).GetField("_playerDamageOverDistance", BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                float damage = ((AnimationCurve)info.GetValue(ev.Projectile.Base as ExplosionGrenade)).Evaluate(Dis);
                damage = BodyArmorUtils.ProcessDamage(Base.ArmorEfficient, damage, 50);
                ServerConsole.AddLog(damage.ToString());
                if (ev.Player != null) Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);
                Health -= damage;
                CheckDead(ev.Player, damage);
            }
        }

        public void CheckDead(Player player, float damage)
        {
            if (Health <= 0)
            {
                HODTO clone = new HODTO
                {
                    Health = this.Base.Health,
                    ArmorEfficient = this.Base.ArmorEfficient,
                    DeadType = this.Base.DeadType,
                    ObjectId = this.Base.ObjectId
                };
                IsAlive = false;
                EventHandler.OnHealthObjectDead(new HealthObjectDeadEventArgs(clone, player));
                MEC.Timing.CallDelayed(Base.DeadDelay, () =>
                {
                    foreach (DeadType type in Enum.GetValues(typeof(DeadType)))
                    {
                        if (Base.DeadType.HasFlag(type))
                        {
                            switch (type)
                            {
                                case DeadType.Disappear:
                                    Destroy(this.gameObject, 0.1f);
                                    break;
                                case DeadType.GetRigidbody:
                                    MakeNonStatic(gameObject);
                                    this.gameObject.AddComponent<Rigidbody>();
                                    break;
                                case DeadType.DynamicDisappearing:
                                    MakeNonStatic(gameObject);
                                    break;
                                case DeadType.Explode:
                                    ExplodeModule.GetSingleton<ExplodeModule>().Execute(ExplodeModule.SelectList<ExplodeModule>(Base.ExplodeModules), this.transform, player.ReferenceHub);
                                    break;
                                case DeadType.ResetHP:
                                    Health = Base.ResetHPTo == 0 ? Base.Health : Base.ResetHPTo;
                                    IsAlive = true;
                                    break;
                                case DeadType.PlayAnimation:
                                    if (modules.Count == 0)
                                    {
                                        modules = AnimationModule.GetModules(Base.animationDTOs, this.gameObject);
                                        if (modules.Count == 0)
                                        {
                                            break;
                                        }
                                    }
                                    AnimationModule.GetSingleton<AnimationModule>().Execute(AnimationModule.SelectList<AnimationModule>(modules));
                                    break;
                                case DeadType.Warhead:
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
                                case DeadType.SendMessage:
                                    MessageModule.GetSingleton<MessageModule>().Execute(MessageModule.SelectList<MessageModule>(Base.messageModules), Formatter, player, this.transform, damage);
                                    break;
                                case DeadType.DropItems:
                                    DropItem.GetSingleton<DropItem>().Execute(DropItem.SelectList<DropItem>(Base.dropItems), this.transform);
                                    break;
                                case DeadType.SendCommand:
                                    CommandModule.GetSingleton<CommandModule>().Execute(CommandModule.SelectList<CommandModule>(Base.commandings), Formatter, player, this.transform, damage);
                                    break;
                                case DeadType.GiveEffect:
                                    EffectGivingModule.GetSingleton<EffectGivingModule>().Execute(EffectGivingModule.SelectList<EffectGivingModule>(Base.effectGivingModules), player);
                                    break;
                            }
                        }
                    }
                });
            }
        }

        void MakeNonStatic(GameObject game)
        {
            foreach (AdminToyBase adminToyBase in game.transform.GetComponentsInChildren<AdminToyBase>())
            {
                adminToyBase.enabled = true;
            }
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
            { "{o_pos}", vs => { Vector3 pos = (vs[1] as Transform).position; return string.Format("{0} {1} {2}", pos.x, pos.y, pos.z); } },
            { "{o_room}", vs => Room.Get((vs[1] as Transform).position).RoomName.ToString() },
            { "{o_zone}", vs => Room.Get((vs[1] as Transform).position).Zone.ToString() },
            { "{damage}", vs => (vs[2] as float?).ToString() }
        };

        void Update()
        {
            if (Health <= 0 && this.Base.DeadType.HasFlag(DeadType.DynamicDisappearing) && !AnimationEnded)
            {
                this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, Time.deltaTime);
                if (this.transform.localScale.magnitude <= 0.1f)
                {
                    Destroy();
                }
            }
        }

        void Destroy()
        {
            AnimationEnded = true;
            if (Base.DoNotDestroyAfterDeath) return;
            Destroy(this.gameObject);
        }

        void OnDestroy()
        {
            AdvancedMERTools.Singleton.healthObjects.Remove(this);
        }

        bool AnimationEnded = false;

        //float animationkey = 0;

        public float Health;

        public bool IsAlive = true;

        public new HODTO Base;

        public List<AnimationModule> modules = new List<AnimationModule> { };
    }
}
