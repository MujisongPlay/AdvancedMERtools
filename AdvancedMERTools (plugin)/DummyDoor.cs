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

        static Config config = AdvancedMERTools.Singleton.Config;

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
                }
                this.transform.parent = RealDoor.Base.transform;
                if (RealDoor.Base.Rooms.Length != 0)
                {
                    Destroy(this.gameObject);
                    return;
                }
                Exiled.Events.Handlers.Player.InteractingDoor += OnInteractDoor;
            });
        }

        public void OnInteractDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Door.Base.transform == this.transform.parent && ev.IsAllowed)
            {
                animator.Play(ev.Door.IsOpen ? "DoorClose" : "DoorOpen");
            }
        }

        void Update()
        {
            if (RealDoor == null) return;
            if ((RealDoor as BreakableDoor).IsDestroyed)
            {
                Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractDoor;
                Destroy(this.gameObject);
            }
        }
    }
}
