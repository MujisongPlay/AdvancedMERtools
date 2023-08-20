using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features.Items;
using InventorySystem.Items.Armor;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;

namespace AdvancedMERTools
{
    public class HealthObject : MonoBehaviour
    {
        private void Start()
        {
            Health = Base.Health;
            AdvancedMERTools.Singleton.healthObjects.Add(this);
        }

        public void OnShot(Exiled.Events.EventArgs.Player.ShotEventArgs ev)
        {
            if (!ev.CanHurt || ev.Player == null || !IsAlive) return;
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
            Health -= damage;
            Hitmarker.SendHitmarker(ev.Player.ReferenceHub, 1f);
            CheckDead(ev.Player);
        }

        public void OnGrenadeExplode(Exiled.Events.EventArgs.Map.ExplodingGrenadeEventArgs ev)
        {
            if (!ev.IsAllowed || !IsAlive) return;
            if (Base.whitelistWeapons.Count != 0 && Base.whitelistWeapons.Find(x => x.CustomItemId == 0 && x.ItemType == ItemType.GrenadeHE) == null) return;
            if (ev.Projectile.Type == ItemType.GrenadeHE)
            {
                float Dis = Vector3.Distance(this.transform.position, ev.Position);
                float damage = Dis < 4 ? 300f + Mathf.Max(0f, 30f * (Mathf.Pow(Dis, 2f) - Mathf.Pow(2f, Dis))) : 2160 / Dis - 240f;
                damage = BodyArmorUtils.ProcessDamage(Base.ArmorEfficient, damage, 50);
                if (ev.Player != null) Hitmarker.SendHitmarker(ev.Player.ReferenceHub, 1f);
                Health -= damage;
                CheckDead(ev.Player);
            }
        }

        public void CheckDead(Player player)
        {
            if (Health <= 0)
            {
                HealthObjectDTO clone = new HealthObjectDTO
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
                    switch (Base.DeadType)
                    {
                        case DeadType.Disappear:
                            Destroy();
                            break;
                        case DeadType.GetRigidbody:
                            this.gameObject.AddComponent<Rigidbody>();
                            break;
                        case DeadType.DynamicDisappearing:
                            break;
                        case DeadType.Explode:
                            Utils.ExplosionUtils.ServerExplode(this.transform.position, player.Footprint);
                            Destroy();
                            break;
                        case DeadType.ResetHP:
                            Health = Base.ResetHPTo == 0 ? Base.Health : Base.ResetHPTo;
                            IsAlive = true;
                            break;
                        case DeadType.PlayAnimation:
                            if (animator != null)
                            {
                                if (Base.AnimationType == AnimationType.Start)
                                {
                                    animator.speed = 1f;
                                    animator.Play(Base.AnimationName);
                                }
                                else
                                    animator.speed = 0f;
                            }
                            break;
                        case DeadType.Warhead:
                            switch (Base.warheadActionType)
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
                            break;
                        case DeadType.SendMessage:
                            Player[] players = Player.List.ToArray();
                            if (Base.SendType == SendType.Killer)
                            {
                                players = new Player[] { player };
                            }
                            switch (Base.MessageType)
                            {
                                case MessageType.Cassie:
                                    Cassie.Message(Base.MessageContent, false, true, true);
                                    break;
                                case MessageType.BroadCast:
                                    players.ForEach(x => x.Broadcast(3, Base.MessageContent));
                                    break;
                                case MessageType.Hint:
                                    players.ForEach(x => x.ShowHint(Base.MessageContent));
                                    break;
                            }
                            break;
                        case DeadType.DropItems:
                            if (Base.dropItems.Count == 0)
                            {
                                return;
                            }
                            float Chance = 0;
                            Base.dropItems.ForEach(x => Chance += x.Chance);
                            Chance = UnityEngine.Random.Range(0f, Chance);
                            foreach (DropItem item in Base.dropItems)
                            {
                                if (item.ForceSpawn)
                                {
                                    CreateItem(item);
                                    continue;
                                }
                                if (Chance <= 0) continue;
                                Chance -= item.Chance;
                                if (Chance <= 0 && item.Count > 0)
                                {
                                    CreateItem(item);
                                }
                            }
                            break;
                    }
                });
            }
        }

        void CreateItem(DropItem item)
        {
            if (item.Count == 0) return;
            for (int i = 0; i < item.Count; i++)
            {
                if (item.CustomItemId != 0)
                {
                    if (CustomItem.TryGet(item.CustomItemId, out CustomItem custom))
                    {
                        custom.Spawn(this.transform.position);
                    }
                }
                else
                {
                    Item.Create(item.ItemType).CreatePickup(this.transform.position);
                }
            }
        }

        void Update()
        {
            if (Health <= 0 && this.Base.DeadType == DeadType.DynamicDisappearing && !AnimationEnded)
            {
                if (Base.AnimationCurve != null)
                {
                    if (Base.AnimationCurve.length < animationkey)
                    {
                        Destroy();
                    }
                    else
                    {
                        this.transform.localScale = Vector3.one * Base.AnimationCurve.Evaluate(animationkey);
                    }
                }
                else
                {
                    this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, Time.deltaTime);
                    if (this.transform.localScale.magnitude <= 0.1f)
                    {
                        Destroy();
                    }
                }
                animationkey += Time.deltaTime;
            }
        }

        void Destroy()
        {
            AnimationEnded = true;
            if (Base.DoNotDestroyAfterDeath) return;
            AdvancedMERTools.Singleton.healthObjects.Remove(this);
            Destroy(this.gameObject);
        }

        bool AnimationEnded = false;

        float animationkey = 0;

        public float Health;

        public bool IsAlive = true;

        public HealthObjectDTO Base;

        public Animator animator;
    }
}
