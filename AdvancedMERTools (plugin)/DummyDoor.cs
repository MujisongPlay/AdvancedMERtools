using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapEditorReborn.API.Features;
using UnityEngine;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Serializable;
using MapEditorReborn.API.Features.Objects;
using Exiled.API.Features.Doors;
using Exiled.Events.Handlers;
using Exiled.Events.EventArgs.Player;

namespace AdvancedMERTools
{
    public class DummyDoor : MonoBehaviour
    {
        public DoorSerializable door;
        public Door RealDoor = null;

        public Animator animator;

        static readonly Config config = AdvancedMERTools.Singleton.Config;

        void Start()
        {
            MEC.Timing.CallDelayed(1f, () =>
            {
                animator = this.transform.GetChild(0).GetComponent<Animator>();
                if (RealDoor == null)
                {
                    foreach (DoorObject Door in GameObject.FindObjectsOfType<DoorObject>())
                    {
                        if (door == Door.Base)
                        {
                            RealDoor = Door.Door;
                            break;
                        }
                    }
                    if (RealDoor == null)
                    {
                        float distance = float.MaxValue;
                        foreach (Door Door in Door.List)
                        {
                            if (distance > Vector3.Distance(Door.Position, this.transform.position))
                            {
                                distance = Vector3.Distance(Door.Position, transform.position);
                                RealDoor = Door;
                            }
                        }
                        if (RealDoor == null)
                        {
                            ServerConsole.AddLog("Failed to find proper door!", ConsoleColor.Red);
                            Destroy(this.gameObject);
                        }
                    }
                }
                this.transform.parent = RealDoor.Base.transform;
                this.transform.localEulerAngles = Vector3.zero;
                this.transform.localPosition = Vector3.zero;
                if (RealDoor.Base.Rooms.Length != 0)
                {
                    AdvancedMERTools.Singleton.dummyDoors.Remove(this);
                    Destroy(this.gameObject);
                    return;
                }
            });
        }

        public void OnInteractDoor(InteractingDoorEventArgs ev)
        {
            if (this.RealDoor == null || animator == null)
                return;
            if (ev.Door == RealDoor && ev.IsAllowed)
            {
                animator.Play(ev.Door.IsOpen ? "DoorClose" : "DoorOpen");
            }
        }

        void Update()
        {
            if (RealDoor == null) return;
            if ((RealDoor as BreakableDoor).IsDestroyed)
            {
                AdvancedMERTools.Singleton.dummyDoors.Remove(this);
                Destroy(this.gameObject, 0.5f);
            }
        }
    }
}
