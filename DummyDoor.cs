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
using Exiled.Events.Handlers;
using Exiled.Events.EventArgs.Player;

namespace MERDummyDoorInstaller
{
    public class DummyDoor : MonoBehaviour
    {
        public DoorSerializable door;
        public Door RealDoor;

        public Animator animator;

        void Start()
        {
            MEC.Timing.CallDelayed(1f, () =>
            {
                animator = this.transform.GetChild(0).GetComponent<Animator>();
                foreach (DoorObject Door in GameObject.FindObjectsOfType<DoorObject>())
                {
                    if (door == Door.Base)
                    {
                        RealDoor = Door.Door;
                        break;
                    }
                }
                Exiled.Events.Handlers.Player.InteractingDoor += OnInteractDoor;
            });
        }

        public void OnInteractDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Door == RealDoor || ev.IsAllowed)
            {
                animator.Play(ev.Door.IsOpen ? "DoorClose" : "DoorOpen");
            }
        }

        void Update()
        {
            if (RealDoor == null) return;
            if (RealDoor.IsBroken)
            {
                Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractDoor;
                Destroy(this.gameObject);
            }
        }
    }
}
