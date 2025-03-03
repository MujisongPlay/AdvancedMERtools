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
using PlayerStatsSystem;
using Mirror;

namespace AdvancedMERTools
{
    public class Healther : NetworkBehaviour, IDestructible
    {
        public uint NetworkId 
        {
            get
            {
                return base.netId;
            }
        }

        public Vector3 CenterOfMass 
        {
            get
            {
                return Vector3.zero;
            }
        }

        public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos)
        {
            bool hit = false;
            parents.ForEach(x => hit |= x.Damage(damage, handler, exactHitPos));
            return hit;
        }

        public List<HealthObject> parents = new List<HealthObject> { };
    }

    public class HealthObject : AMERTInteractable, IDestructible
    {
        protected virtual void Start()
        {
            this.Base = base.Base as HODTO;
            Health = Base.Health;
            Register();
        }

        protected void Register()
        {
            this.transform.GetComponentsInChildren<PrimitiveObjectToy>().ForEach(x =>
            {
                if (x.gameObject.TryGetComponent<Healther>(out Healther healther))
                {
                    healther.parents.Add(this);
                }
                else
                {
                    healther = x.gameObject.AddComponent<Healther>();
                    healther.parents.Add(this);
                }
            });
            AdvancedMERTools.Singleton.healthObjects.Add(this);
        }

        protected static Dictionary<ExplosionType, ItemType> ExplosionDic = new Dictionary<ExplosionType, ItemType>
        {
            { ExplosionType.Grenade, ItemType.GrenadeHE },
            { ExplosionType.Disruptor, ItemType.ParticleDisruptor },
            { ExplosionType.Jailbird, ItemType.Jailbird },
            { ExplosionType.Cola, ItemType.SCP207 },
            { ExplosionType.PinkCandy, ItemType.SCP330 },
            { ExplosionType.SCP018, ItemType.SCP018 }
        };

        public virtual bool Damage(float damage, DamageHandlerBase handler, Vector3 pos)
        {
            if (!IsAlive || !Active)
                return false;
            //ServerConsole.AddLog("1");
            Player attacker = null;
            AttackerDamageHandler damageHandler = handler as AttackerDamageHandler;
            if (damageHandler != null)
            {
                //ServerConsole.AddLog("2");
                attacker = Player.Get(damageHandler.Attacker);
                FirearmDamageHandler firearm = handler as FirearmDamageHandler;
                ExplosionDamageHandler explosion = handler as ExplosionDamageHandler;
                if (firearm != null)
                {
                    //ServerConsole.AddLog("3");
                    if (Base.whitelistWeapons.Count == 0 || Base.whitelistWeapons.Any(x =>
                    {
                        if (CustomItem.TryGet(attacker.CurrentItem, out CustomItem custom))
                        {
                            return custom.Id == x.CustomItemId;
                        }
                        else
                        {
                            return attacker.CurrentItem.Type == x.ItemType;
                        }
                    }))
                    {
                        //ServerConsole.AddLog("4");
                        FieldInfo info = typeof(FirearmDamageHandler).GetField("_penetration", BindingFlags.NonPublic | BindingFlags.Instance);
                        damage = BodyArmorUtils.ProcessDamage(Base.ArmorEfficient, damage, Mathf.RoundToInt((float)info.GetValue(firearm) * 100f));
                    }
                    else
                        return false;
                }
                //ServerConsole.AddLog("5");
                if (explosion != null)
                {
                    if (Base.whitelistWeapons.Count != 0 && !Base.whitelistWeapons.Any(x =>
                    {
                        if (ExplosionDic.TryGetValue(explosion.ExplosionType, out ItemType item))
                        {
                            return item == x.ItemType;
                        }
                        return false;
                    }))
                    {
                        return false;
                    }
                }
                //ServerConsole.AddLog("6");
            }
            //ServerConsole.AddLog("7");
            CheckDead(attacker, damage);
            return true;
        }

        public virtual void OnGrenadeExplode(Exiled.Events.EventArgs.Map.ExplodingGrenadeEventArgs ev)
        {
            if (!ev.IsAllowed || !IsAlive || !Active) return;
            if (Base.whitelistWeapons.Count != 0 && Base.whitelistWeapons.Find(x => x.CustomItemId == 0 && x.ItemType == ItemType.GrenadeHE) == null) return;
            if (ev.Projectile.Type == ItemType.GrenadeHE)
            {
                float Dis = Vector3.Distance(this.transform.position, ev.Position);
                FieldInfo info = typeof(ExplosionGrenade).GetField("_playerDamageOverDistance", BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                float damage = ((AnimationCurve)info.GetValue(ev.Projectile.Base as ExplosionGrenade)).Evaluate(Dis);
                damage = BodyArmorUtils.ProcessDamage(Base.ArmorEfficient, damage, 50);
                CheckDead(ev.Player, damage);
            }
        }

        public virtual void CheckDead(Player player, float damage)
        {
            Health -= damage;
            Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, damage / 10f);
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
                ModuleGeneralArguments args = new ModuleGeneralArguments { interpolations = Formatter, interpolationsList = new object[] { player, transform }, player = player, schematic = OSchematic, transform = transform, TargetCalculated = false };
                EventHandler.OnHealthObjectDead(new HealthObjectDeadEventArgs(clone, player));
                MEC.Timing.CallDelayed(Base.DeadActionDelay, () =>
                {
                    var deadTypeExecutors = new Dictionary<DeadType, Action>
{
    { DeadType.Disappear, () => Destroy(this.gameObject, 0.1f) },
    { DeadType.GetRigidbody, () =>
        {
            MakeNonStatic(gameObject);
            this.gameObject.AddComponent<Rigidbody>();
        }
    },
    { DeadType.DynamicDisappearing, () => MakeNonStatic(gameObject) },
    { DeadType.Explode, () => ExplodeModule.Execute(Base.ExplodeModules, args) },
    { DeadType.ResetHP, () =>
        {
            Health = Base.ResetHPTo == 0 ? Base.Health : Base.ResetHPTo;
            IsAlive = true;
        }
    },
    { DeadType.PlayAnimation, () => AnimationDTO.Execute(Base.AnimationModules, args) },
    { DeadType.Warhead, () => AlphaWarhead(Base.warheadAction) },
    { DeadType.SendMessage, () => MessageModule.Execute(Base.MessageModules, args) },
    { DeadType.DropItems, () => DropItem.Execute(Base.DropItems, args) },
    { DeadType.SendCommand, () => Commanding.Execute(Base.Commandings, args) },
    { DeadType.GiveEffect, () => EffectGivingModule.Execute(Base.effectGivingModules, args) },
    { DeadType.PlayAudio, () => AudioModule.Execute(Base.AudioModules, args) },
    { DeadType.CallGroovieNoise, () => CGNModule.Execute(Base.GroovieNoiseToCall, args) },
    { DeadType.CallFunction, () => CFEModule.Execute(Base.FunctionToCall, args) }
};
                    foreach (DeadType type in Enum.GetValues(typeof(DeadType)))
                    {
                        if (Base.DeadType.HasFlag(type) && deadTypeExecutors.TryGetValue(type, out var execute))
                        {
                            execute();
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
            { "{p_pos}", vs => { Vector3 pos = (vs[0] as Player).Transform.position; return $"{pos.x} {pos.y} {pos.z}"; } },
            { "{p_room}", vs => (vs[0] as Player).CurrentRoom.RoomName.ToString() },
            { "{p_zone}", vs => (vs[0] as Player).Zone.ToString() },
            { "{p_role}", vs => (vs[0] as Player).Role.Type.ToString() },
            { "{p_item}", vs => (vs[0] as Player).CurrentItem.Type.ToString() },
            { "{o_pos}", vs => { Vector3 pos = (vs[1] as Transform).position; return $"{pos.x} {pos.y} {pos.z}"; } },
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

        protected virtual void Destroy()
        {
            AnimationEnded = true;
            if (Base.DoNotDestroyAfterDeath) return;
            Destroy(this.gameObject);
        }

        bool AnimationEnded = false;

        public float Health;

        public bool IsAlive = true;

        public new HODTO Base;

        public uint NetworkId
        {
            get
            {
                return base.netId;
            }
        }

        public Vector3 CenterOfMass
        {
            get
            {
                return Vector3.zero;
            }
        }
    }

    public class FHealthObject : HealthObject
    {
        protected override void Start()
        {
            this.Base = ((AMERTInteractable)this).Base as FHODTO;
            Health = Base.Health.GetValue(new FunctionArgument(this), 100f);
            Register();
        }

        public override bool Damage(float damage, DamageHandlerBase handler, Vector3 pos)
        {
            if (!IsAlive || !Active)
                return false;
            //ServerConsole.AddLog("1");
            Player attacker = null;
            AttackerDamageHandler damageHandler = handler as AttackerDamageHandler;
            if (damageHandler != null)
            {
                //ServerConsole.AddLog("2");
                attacker = Player.Get(damageHandler.Attacker);
                FunctionArgument args = new FunctionArgument(this, attacker);
                FirearmDamageHandler firearm = handler as FirearmDamageHandler;
                ExplosionDamageHandler explosion = handler as ExplosionDamageHandler;
                if (firearm != null)
                {
                    //ServerConsole.AddLog("3");
                    if (Base.whitelistWeapons.Count == 0 || Base.whitelistWeapons.Any(x =>
                    {
                        if (CustomItem.TryGet(attacker.CurrentItem, out CustomItem custom))
                        {
                            return custom.Id == x.CustomItemId.GetValue(args, 0);
                        }
                        else
                        {
                            return attacker.CurrentItem.Type == x.ItemType.GetValue(args, ItemType.None);
                        }
                    }))
                    {
                        //ServerConsole.AddLog("4");
                        FieldInfo info = typeof(FirearmDamageHandler).GetField("_penetration", BindingFlags.NonPublic | BindingFlags.Instance);
                        damage = BodyArmorUtils.ProcessDamage(Base.ArmorEfficient.GetValue(args, 0), damage, Mathf.RoundToInt((float)info.GetValue(firearm) * 100f));
                    }
                    else
                        return false;
                }
                //ServerConsole.AddLog("5");
                if (explosion != null)
                {
                    if (Base.whitelistWeapons.Count != 0 && !Base.whitelistWeapons.Any(x =>
                    {
                        if (ExplosionDic.TryGetValue(explosion.ExplosionType, out ItemType item))
                        {
                            return item == x.ItemType.GetValue(args, ItemType.None);
                        }
                        return false;
                    }))
                    {
                        return false;
                    }
                }
                //ServerConsole.AddLog("6");
            }
            //ServerConsole.AddLog("7");
            CheckDead(attacker, damage);
            return true;
        }

        public override void OnGrenadeExplode(Exiled.Events.EventArgs.Map.ExplodingGrenadeEventArgs ev)
        {
            if (!ev.IsAllowed || !IsAlive || !Active) return;
            FunctionArgument args = new FunctionArgument(this, ev.Player);
            if (Base.whitelistWeapons.Count != 0 && Base.whitelistWeapons.Find(x => x.CustomItemId.GetValue(args, 0) == 0 && x.ItemType.GetValue(args, ItemType.None) == ItemType.GrenadeHE) == null) return;
            if (ev.Projectile.Type == ItemType.GrenadeHE)
            {
                float Dis = Vector3.Distance(this.transform.position, ev.Position);
                FieldInfo info = typeof(ExplosionGrenade).GetField("_playerDamageOverDistance", BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                float damage = ((AnimationCurve)info.GetValue(ev.Projectile.Base as ExplosionGrenade)).Evaluate(Dis);
                damage = BodyArmorUtils.ProcessDamage(Base.ArmorEfficient.GetValue(args, 0), damage, 50);
                CheckDead(ev.Player, damage);
            }
        }

        public override void CheckDead(Player player, float damage)
        {
            Health -= damage;
            Hitmarker.SendHitmarkerDirectly(player.ReferenceHub, damage / 10f);
            if (Health <= 0)
            {
                IsAlive = false;
                FunctionArgument args = new FunctionArgument(this, player);
                MEC.Timing.CallDelayed(Base.DeadActionDelay.GetValue(args, 0f), () =>
                {
                    var deadTypeExecutors = new Dictionary<DeadType, Action>
{
    { DeadType.Disappear, () => Destroy(this.gameObject, 0.1f) },
    { DeadType.GetRigidbody, () =>
        {
            MakeNonStatic(gameObject);
            this.gameObject.AddComponent<Rigidbody>();
        }
    },
    { DeadType.DynamicDisappearing, () => MakeNonStatic(gameObject) },
    { DeadType.Explode, () => FExplodeModule.Execute(Base.ExplodeModules, args) },
    { DeadType.ResetHP, () =>
        {
            float RH = Base.ResetHPTo.GetValue(args, 0f);
            Health = RH == 0 ? Base.Health.GetValue(args, 100f) : RH;
            IsAlive = true;
        }
    },
    { DeadType.PlayAnimation, () => FAnimationDTO.Execute(Base.AnimationModules, args) },
    { DeadType.Warhead, () => AlphaWarhead(Base.warheadActionType.GetValue<WarheadActionType>(args, 0)) },
    { DeadType.SendMessage, () => FMessageModule.Execute(Base.MessageModules, args) },
    { DeadType.DropItems, () => FDropItem.Execute(Base.dropItems, args) },
    { DeadType.SendCommand, () => FCommanding.Execute(Base.commandings, args) },
    { DeadType.GiveEffect, () => FEffectGivingModule.Execute(Base.effectGivingModules, args) },
    { DeadType.PlayAudio, () => FAudioModule.Execute(Base.AudioModules, args) },
    { DeadType.CallGroovieNoise, () => FCGNModule.Execute(Base.GroovieNoiseToCall, args) },
    { DeadType.CallFunction, () => FCFEModule.Execute(Base.FunctionToCall, args) },
};
                    foreach (DeadType type in Enum.GetValues(typeof(DeadType)))
                    {
                        if (Base.DeadType.HasFlag(type) && deadTypeExecutors.TryGetValue(type, out var execute))
                        {
                            execute();
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

        protected override void Destroy()
        {
            AnimationEnded = true;
            if (Base.DoNotDestroyAfterDeath.GetValue(new FunctionArgument(this), false)) return;
            Destroy(this.gameObject);
        }

        bool AnimationEnded = false;

        public new FHODTO Base;
    }
}
