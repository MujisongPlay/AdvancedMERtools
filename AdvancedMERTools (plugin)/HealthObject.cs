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
        private void Start()
        {
            this.Base = base.Base as HODTO;
            Health = Base.Health;
            //ServerConsole.AddLog("!");
            //transform.GetComponentsInChildren<Transform>().ForEach(x => x.gameObject.layer = LayerMask.GetMask("Glass"));
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
            AdvancedMERTools.Singleton.codeClassPair.Add(Base.Code, this);
        }

        static Dictionary<ExplosionType, ItemType> ExplosionDic = new Dictionary<ExplosionType, ItemType>
        {
            { ExplosionType.Grenade, ItemType.GrenadeHE },
            { ExplosionType.Disruptor, ItemType.ParticleDisruptor },
            { ExplosionType.Jailbird, ItemType.Jailbird },
            { ExplosionType.Cola, ItemType.SCP207 },
            { ExplosionType.PinkCandy, ItemType.SCP330 },
            { ExplosionType.SCP018, ItemType.SCP018 }
        };

        public bool Damage(float damage, DamageHandlerBase handler, Vector3 pos)
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

        public void OnGrenadeExplode(Exiled.Events.EventArgs.Map.ExplodingGrenadeEventArgs ev)
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

        public void CheckDead(Player player, float damage)
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
                EventHandler.OnHealthObjectDead(new HealthObjectDeadEventArgs(clone, player));
                MEC.Timing.CallDelayed(Base.DeadActionDelay, () =>
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
                                        modules = AnimationModule.GetModules(Base.AnimationModules, this.gameObject);
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
                                        if (Base.warheadAction.HasFlag(warhead))
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
                                    MessageModule.GetSingleton<MessageModule>().Execute(MessageModule.SelectList<MessageModule>(Base.MessageModules), Formatter, player, this.transform, damage);
                                    break;
                                case DeadType.DropItems:
                                    DropItem.GetSingleton<DropItem>().Execute(DropItem.SelectList<DropItem>(Base.DropItems), this.transform);
                                    break;
                                case DeadType.SendCommand:
                                    Commanding.GetSingleton<Commanding>().Execute(Commanding.SelectList<Commanding>(Base.Commandings), Formatter, player, this.transform, damage);
                                    break;
                                case DeadType.GiveEffect:
                                    EffectGivingModule.GetSingleton<EffectGivingModule>().Execute(EffectGivingModule.SelectList<EffectGivingModule>(Base.effectGivingModules), player);
                                    break;
                                case DeadType.PlayAudio:
                                    AudioModule.GetSingleton<AudioModule>().Execute(AudioModule.SelectList<AudioModule>(Base.AudioModules), this.transform);
                                    break;
                                case DeadType.CallGroovieNoise:
                                    CGNModule.GetSingleton<CGNModule>().Execute(CGNModule.SelectList<CGNModule>(Base.GroovieNoiseToCall));
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
            AdvancedMERTools.Singleton.codeClassPair.Remove(Base.Code);
            AdvancedMERTools.Singleton.healthObjects.Remove(this);
        }

        bool AnimationEnded = false;

        public float Health;

        public bool IsAlive = true;

        public new HODTO Base;

        public List<AnimationModule> modules = new List<AnimationModule> { };

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
}
