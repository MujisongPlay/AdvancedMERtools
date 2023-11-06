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
using RemoteAdmin;
using CommandSystem.Commands;
using CommandSystem;
using MapEditorReborn.API.Features.Objects;

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
                float damage = Dis < 4 ? 300f + Mathf.Max(0f, 30f * (Mathf.Pow(Dis, 2f) - Mathf.Pow(2f, Dis))) : 2160 / Dis - 240f;
                damage = BodyArmorUtils.ProcessDamage(Base.ArmorEfficient, damage, 50);
                if (ev.Player != null) Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);
                Health -= damage;
                CheckDead(ev.Player, damage);
            }
        }

        public void CheckDead(Player player, float damage)
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
                    foreach (DeadType type in Enum.GetValues(typeof(DeadType)))
                    {
                        if (Base.DeadType.HasFlag(type))
                        {
                            switch (type)
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
                                    Utils.ExplosionUtils.ServerExplode(this.transform.position, (Base.FFon ? Server.Host : player).Footprint);
                                    Destroy();
                                    break;
                                case DeadType.ResetHP:
                                    Health = Base.ResetHPTo == 0 ? Base.Health : Base.ResetHPTo;
                                    IsAlive = true;
                                    break;
                                case DeadType.PlayAnimation:
                                    if (modules.Count == 0)
                                    {
                                        foreach (AnimationDTO dTO in Base.animationDTOs)
                                        {
                                            if (!EventManager.FindObjectWithPath(this.GetComponentInParent<SchematicObject>().transform, dTO.Animator).TryGetComponent(out Animator animator))
                                            {
                                                ServerConsole.AddLog("Cannot find appopriate animator!");
                                                continue;
                                            }
                                            modules.Add(new AnimationModule
                                            {
                                                Animator = animator,
                                                AnimationName = dTO.Animation,
                                                AnimationType = dTO.AnimationType,
                                                ChanceWeight = dTO.Chance,
                                                ForceExecute = dTO.Force
                                            });
                                        }
                                        if (modules.Count == 0)
                                        {
                                            break;
                                        }
                                    }
                                    float Chance = 0f;
                                    modules.ForEach(x => Chance += x.ChanceWeight);
                                    Chance = UnityEngine.Random.Range(0, Chance);
                                    foreach (AnimationModule module in modules)
                                    {
                                        if (module.Animator == null)
                                            continue;
                                        if (module.ForceExecute)
                                        {
                                            goto IL_01;
                                        }
                                        if (Chance <= 0)
                                            continue;
                                        Chance -= module.ChanceWeight;
                                        if (Chance <= 0)
                                        {
                                            goto IL_01;
                                        }
                                        continue;
                                    IL_01:
                                        if (module.AnimationType == AnimationType.Start)
                                        {
                                            module.Animator.Play(module.AnimationName);
                                            module.Animator.speed = 1f;
                                        }
                                        else
                                            module.Animator.speed = 0f;
                                    }
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
                                    Player[] players = Player.List.ToArray();
                                    if (Base.SendType == SendType.Killer)
                                    {
                                        players = new Player[] { player };
                                    }
                                    switch (Base.MessageType)
                                    {
                                        case MessageType.Cassie:
                                            Cassie.Message(ApplyFormat(Base.MessageContent, player, damage), false, true, true);
                                            break;
                                        case MessageType.BroadCast:
                                            players.ForEach(x => x.Broadcast(3, ApplyFormat(Base.MessageContent, player, damage)));
                                            break;
                                        case MessageType.Hint:
                                            players.ForEach(x => x.ShowHint(ApplyFormat(Base.MessageContent, player, damage)));
                                            break;
                                    }
                                    break;
                                case DeadType.DropItems:
                                    if (Base.dropItems.Count == 0)
                                    {
                                        return;
                                    }
                                    Chance = 0;
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
                                case DeadType.SendCommand:
                                    if (Base.commandings.Count == 0)
                                    {
                                        return;
                                    }
                                    Chance = 0;
                                    Base.commandings.ForEach(x => Chance += x.Chance);
                                    Chance = UnityEngine.Random.Range(0f, Chance);
                                    foreach (Commanding commanding in Base.commandings)
                                    {
                                        if (commanding.ForceExecute && commanding.CommandContext != "")
                                        {
                                            ExecuteCommand(commanding, player, damage);
                                            continue;
                                        }
                                        if (Chance <= 0) continue;
                                        Chance -= commanding.Chance;
                                        if (Chance <= 0 && commanding.CommandContext != "")
                                        {
                                            ExecuteCommand(commanding, player, damage);
                                        }
                                    }
                                    break;
                            }
                        }
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

        void ExecuteCommand(Commanding commanding, Player player, float damage)
        {
            string command = ApplyFormat(commanding.CommandContext, player, damage);
            string[] array = command.Trim().Split(new char[] { ' ' }, 512, StringSplitOptions.RemoveEmptyEntries);
            ICommand command1;
            if (CommandProcessor.RemoteAdminCommandHandler.TryGetCommand(array[0], out command1) /*&& commanding.CommandType == CommandType.RemoteAdmin*/)
            {
                //if (commanding.ExecutorType == ExecutorType.Attacker)
                //    command1.Execute(array.Segment(0), player.Sender, out _);
                //else
                    command1.Execute(array.Segment(1), ServerConsole.Scs, out _);
            }
            //if (commanding.CommandType == CommandType.ClientConsole)
            //{
            //    if (commanding.ExecutorType == ExecutorType.Attacker)
            //        Server.RunCommand(command, player.Sender);
            //    else
            //        Server.RunCommand(command, ServerConsole.Scs);
            //}
        }

        string ApplyFormat(string context, Player player, float damage)
        {
            try
            {
                context = context.Replace("{attacker_i}", player.Id.ToString())
                .Replace("{attacker_name}", player.Nickname)
                .Replace("{a_role}", player.Role.Type.ToString())
                .Replace("{damage}", damage.ToString());
                Vector3 vector3 = player.Position;
                context = context.Replace("{a_pos}", string.Format("{0} {1} {2}", vector3.x, vector3.y, vector3.z));
                if (player.CurrentRoom != null)
                    context = context.Replace("{a_room}", player.CurrentRoom.RoomName.ToString())
                    .Replace("{a_zone}", player.CurrentRoom.Identifier.Zone.ToString());
                vector3 = this.transform.position;
                context = context.Replace("{s_pos}", string.Format("{0} {1} {2}", vector3.x, vector3.y, vector3.z));
                Room room = Room.Get(this.transform.position);
                if (room != null)
                    context = context.Replace("{s_room}", Room.Get(this.transform.position).Type.ToString())
                    .Replace("{s_zone}", Room.Get(this.transform.position).Identifier.Zone.ToString());
                if (player.CurrentItem == null)
                    context = context.Replace("{a_item}", "null");
                else
                    context = context.Replace("{a_item}", player.CurrentItem.Type.ToString());
            }
            catch (Exception) { }
            return context;
        }

        void Update()
        {
            if (Health <= 0 && this.Base.DeadType == DeadType.DynamicDisappearing && !AnimationEnded)
            {
                //if (Base.AnimationCurve != null)
                //{
                //    if (Base.AnimationCurve.keys.Last().time < animationkey)
                //    {
                //        Destroy();
                //    }
                //    else
                //    {
                //        this.transform.localScale = Vector3.one * Base.AnimationCurve.Evaluate(animationkey);
                //    }
                //}
                //else
                //{
                    this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, Time.deltaTime);
                    if (this.transform.localScale.magnitude <= 0.1f)
                    {
                        Destroy();
                    }
                //}
                //animationkey += Time.deltaTime;
            }
        }

        void Destroy()
        {
            AnimationEnded = true;
            if (Base.DoNotDestroyAfterDeath) return;
            AdvancedMERTools.Singleton.healthObjects.Remove(this);
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

        public HealthObjectDTO Base;

        public List<AnimationModule> modules = new List<AnimationModule> { };
    }
}
