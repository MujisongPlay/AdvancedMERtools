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
    public class InteractablePickup : MonoBehaviour
    {
        void Start()
        {
            if (gameObject.TryGetComponent<ItemSpawnPointObject>(out ItemSpawnPointObject item))
            {
                AdvancedMERTools.Singleton.InteractablePickups.Add(this);
                itemSpawn = item;
            }
            else
            {
                ServerConsole.AddLog("Could not find itemspawnpointObject! abort!");
                Destroy(this);
            }
        }

        public void Destroy()
        {
            AdvancedMERTools.Singleton.InteractablePickups.Remove(this);
            Destroy(this);
        }

        public void OnInteracted(Exiled.Events.EventArgs.Player.PickingUpItemEventArgs ev)
        {
            Pickups.Clear();
            Pickups.AddRange(itemSpawn.AttachedPickups);
            if (Pickups.Count == 0 || !Pickups.Contains(ev.Pickup))
            {
                return;
            }
            foreach (ActionType type in Enum.GetValues(typeof(ActionType)))
            {
                if (Base.ActionType.HasFlag(type))
                {
                    switch (type)
                    {
                        case ActionType.Disappear:
                            Pickups.Remove(ev.Pickup);
                            ev.Pickup.Destroy();
                            break;
                        case ActionType.Explode:
                            Utils.ExplosionUtils.ServerExplode(ev.Pickup.Position, (Base.FFon ? Server.Host : ev.Player).Footprint);
                            break;
                        case ActionType.PlayAnimation:
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
                        case ActionType.Warhead:
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
                        case ActionType.SendMessage:
                            Player[] players = Player.List.ToArray();
                            if (Base.SendType == SendType.Killer)
                            {
                                players = new Player[] { ev.Player };
                            }
                            switch (Base.MessageType)
                            {
                                case MessageType.Cassie:
                                    Cassie.Message(ApplyFormat(Base.MessageContent, ev.Player, ev.Pickup), false, true, true);
                                    break;
                                case MessageType.BroadCast:
                                    players.ForEach(x => x.Broadcast(3, ApplyFormat(Base.MessageContent, ev.Player, ev.Pickup)));
                                    break;
                                case MessageType.Hint:
                                    players.ForEach(x => x.ShowHint(ApplyFormat(Base.MessageContent, ev.Player, ev.Pickup)));
                                    break;
                            }
                            break;
                        case ActionType.DropItems:
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
                        case ActionType.SendCommand:
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
                                    ExecuteCommand(commanding, ev.Player, ev.Pickup);
                                    continue;
                                }
                                if (Chance <= 0) continue;
                                Chance -= commanding.Chance;
                                if (Chance <= 0 && commanding.CommandContext != "")
                                {
                                    ExecuteCommand(commanding, ev.Player, ev.Pickup);
                                }
                            }
                            break;
                        case ActionType.UpgradeItem:
                            if (ev.Player.GameObject.TryGetComponent<Collider>(out Collider col))
                            {
                                List<int> vs = new List<int> { };
                                for (int j = 0; j < 5; j++)
                                {
                                    if (Base.Scp914Mode.HasFlag((Scp914Mode)j))
                                    {
                                        vs.Add(j);
                                    }
                                }
                                Scp914.Scp914Upgrader.Upgrade(new Collider[] { col }, Vector3.zero, Scp914.Scp914Mode.Held, (Scp914.Scp914KnobSetting)vs.RandomItem());
                            }
                            break;
                    }
                }
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

        void ExecuteCommand(Commanding commanding, Player player, Pickup pickup)
        {
            string command = ApplyFormat(commanding.CommandContext, player, pickup);
            string[] array = command.Trim().Split(new char[] { ' ' }, 512, StringSplitOptions.RemoveEmptyEntries);
            bool flag = CommandProcessor.RemoteAdminCommandHandler.TryGetCommand(array[0], out ICommand command1) && commanding.CommandType == CommandType.RemoteAdmin
                || GameCore.Console.singleton.ConsoleCommandHandler.TryGetCommand(array[0], out command1) && commanding.CommandType == CommandType.ClientConsole;
            if (flag)
            {
                if (commanding.ExecutorType == ExecutorType.Attacker)
                    command1.Execute(array.Segment(0), player.Sender, out _);
                else
                    command1.Execute(array.Segment(0), ServerConsole.Scs, out _);
            }
        }

        string ApplyFormat(string context, Player player, Pickup pickup)
        {
            context.Replace("{picker_i}", player.Id.ToString());
            context.Replace("{picker_name}", player.Nickname);
            Vector3 vector3 = player.Position;
            context.Replace("{a_pos}", string.Format("{0} {1} {2}", vector3.x, vector3.y, vector3.z));
            context.Replace("{a_room}", player.CurrentRoom.RoomName.ToString());
            context.Replace("{a_zone}", player.CurrentRoom.Identifier.Zone.ToString());
            context.Replace("{a_role}", player.Role.Type.ToString());
            vector3 = pickup.Transform.position;
            context.Replace("{s_pos}", string.Format("{0} {1} {2}", vector3.x, vector3.y, vector3.z));
            context.Replace("{s_room}", Room.Get(this.transform.position).Type.ToString());
            context.Replace("{s_zone}", Room.Get(this.transform.position).Identifier.Zone.ToString());
            context.Replace("{a_item}", player.CurrentItem.Type.ToString());
            return context;
        }

        public ItemSpawnPointObject itemSpawn;

        public List<Pickup> Pickups;

        public IPDTO Base;

        public Animator animator;
    }
}
