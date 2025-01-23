using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Items;
using MapEditorReborn.Events.EventArgs;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features;
using Exiled.CustomItems;
using CommandSystem;
using Utf8Json.Formatters;
using RemoteAdmin;
using Exiled.CustomItems.API.Features;
using InventorySystem.Items.ThrowableProjectiles;
using MapGeneration;
using Mirror;

namespace AdvancedMERTools
{
    public class CustomCollider : AMERTInteractable
    {
        protected virtual void Start()
        {
            this.Base = base.Base as CCDTO;
            Register();
        }

        protected void Register()
        {
            AdvancedMERTools.Singleton.CustomColliders.Add(this);
            CustomCollider[] customColliders = gameObject.GetComponents<CustomCollider>();
            if (customColliders.Length > 1 && customColliders[0] != this)
            {
                MEC.Timing.CallDelayed(0.1f, () =>
                {
                    meshCollider = customColliders[0].meshCollider;
                });
                return;
            }
            Vector3[] vs = new Vector3[] { transform.position, transform.eulerAngles };
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;

            MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();
            meshCollider = gameObject.AddComponent<MeshCollider>();
            CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];

            for (int i = 0; i < meshFilters.Length; i++)
            {
                combineInstances[i].mesh = meshFilters[i].sharedMesh;
                combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combineInstances);
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;
            meshCollider.isTrigger = true;
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;

            //if (Base.Invisible)
            //{
            transform.GetComponentsInChildren<AdminToys.PrimitiveObjectToy>().ForEach(x =>
            {
                x.gameObject.SetActive(false);
                NetworkServer.Destroy(x.gameObject);
            });
            //}
            //meshCollider.contactOffset = Base.ContactOffSet;

            transform.position = vs[0];
            transform.eulerAngles = vs[1];
        }

        void OnTriggerEnter(Collider collider)
        {
            if (Base.CollisionType.HasFlag(CollisionType.OnEnter))
                RunProcess(collider);
        }

        void OnTriggerExit(Collider collider)
        {
            if (Base.CollisionType.HasFlag(CollisionType.OnExit))
                RunProcess(collider);
        }

        void OnTriggerStay(Collider collider)
        {
            if (Base.CollisionType.HasFlag(CollisionType.OnStay))
                RunProcess(collider);
        }

        public virtual void RunProcess(Collider collider)
        {
            if (!Active)
                return;
            bool flag = false;
            Player target = null;
            Pickup pickup = Pickup.Get(collider.gameObject);
            if (Base.DetectType.HasFlag(DetectType.Pickup) && pickup != null)
            {
                target = pickup.PreviousOwner;
                flag = true;
            }
            if (Base.DetectType.HasFlag(DetectType.Player) && Player.TryGet(collider, out target))
            {
                flag = target.Role.Base.ActiveTime > 0.25f;
            }
            ThrownProjectile projectile = collider.GetComponentInParent<ThrownProjectile>();
            if (Base.DetectType.HasFlag(DetectType.Projectile) && projectile != null)
            {
                target = Player.Get(projectile.PreviousOwner);
                flag = true;
            }
            if (!flag)
                return;
            ModuleGeneralArguments args = new ModuleGeneralArguments { player = target, TargetCalculated = false, transform = this.transform, schematic = OSchematic, interpolations = Formatter, interpolationsList = new object[] { target, gameObject } };
            var colliderActionExecutors = new Dictionary<ColliderActionType, Action>
{
    { ColliderActionType.ModifyHealth, () =>
        {
            if (target != null)
            {
                if (Base.ModifyHealthAmount > 0)
                    target.Heal(Base.ModifyHealthAmount);
                else
                    target.Hurt(-1 * Base.ModifyHealthAmount);
            }
        }
    },
    { ColliderActionType.Explode, () => ExplodeModule.Execute(Base.ExplodeModules, args) },
    { ColliderActionType.PlayAnimation, () => AnimationDTO.Execute(Base.AnimationModules, args) },
    { ColliderActionType.Warhead, () => AlphaWarhead(Base.warheadActionType) },
    { ColliderActionType.SendMessage, () => MessageModule.Execute(Base.MessageModules, args) },
    { ColliderActionType.SendCommand, () => Commanding.Execute(Base.commandings, args) },
    { ColliderActionType.GiveEffect, () => EffectGivingModule.Execute(Base.effectGivingModules, args) },
    { ColliderActionType.PlayAudio, () => AudioModule.Execute(Base.AudioModules, args) },
    { ColliderActionType.CallGroovieNoise, () => CGNModule.Execute(Base.GroovieNoiseToCall, args) },
    { ColliderActionType.CallFunction, () => CFEModule.Execute(Base.FunctionToCall, args) }
};
            foreach (ColliderActionType type in Enum.GetValues(typeof(ColliderActionType)))
            {
                if (Base.ColliderActionType.HasFlag(type) && colliderActionExecutors.TryGetValue(type, out var execute))
                {
                    execute();
                }
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
            { "{o_pos}", vs => { Vector3 pos = (vs[1] as GameObject).transform.position; return string.Format("{0} {1} {2}", pos.x, pos.y, pos.z); } },
            { "{o_room}", vs => RoomIdUtils.RoomAtPosition((vs[1] as GameObject).transform.position).Name.ToString() },
            { "{o_zone}", vs => RoomIdUtils.RoomAtPosition((vs[1] as GameObject).transform.position).Zone.ToString() }
        };

        void OnDestroy()
        {
            AdvancedMERTools.Singleton.CustomColliders.Remove(this);
        }

        public MeshCollider meshCollider;

        public new CCDTO Base;

        public Transform originalT;
    }

    public class FCustomCollider : CustomCollider
    {
        protected override void Start()
        {
            Base = ((AMERTInteractable)this).Base as FCCDTO;
            Register();
        }

        public override void RunProcess(Collider collider)
        {
            if (!Active)
                return;
            bool flag = false;
            Player target = null;
            Pickup pickup = Pickup.Get(collider.gameObject);
            CollisionType collision = Base.DetectType.GetValue<CollisionType>(new FunctionArgument(this), 0);
            if (collision.HasFlag(DetectType.Pickup) && pickup != null)
            {
                target = pickup.PreviousOwner;
                flag = true;
            }
            if (collision.HasFlag(DetectType.Player) && Player.TryGet(collider, out target))
            {
                flag = target.Role.Base.ActiveTime > 0.25f;
            }
            ThrownProjectile projectile = collider.GetComponentInParent<ThrownProjectile>();
            if (collision.HasFlag(DetectType.Projectile) && projectile != null)
            {
                target = Player.Get(projectile.PreviousOwner);
                flag = true;
            }
            if (!flag)
                return;
            FunctionArgument args = new FunctionArgument(this, target);
            var colliderActionExecutors = new Dictionary<ColliderActionType, Action>
{
    { ColliderActionType.ModifyHealth, () =>
        {
            if (target != null)
            {
                float amount = Base.ModifyHealthAmount.GetValue(args, 0f);
                if (amount > 0)
                    target.Heal(amount);
                else
                    target.Hurt(-amount);
            }
        }
    },
    { ColliderActionType.Explode, () => FExplodeModule.Execute(Base.ExplodeModules, args) },
    { ColliderActionType.PlayAnimation, () => FAnimationDTO.Execute(Base.AnimationModules, args) },
    { ColliderActionType.Warhead, () => AlphaWarhead(Base.warheadActionType.GetValue<WarheadActionType>(args, 0)) },
    { ColliderActionType.SendMessage, () => FMessageModule.Execute(Base.MessageModules, args) },
    { ColliderActionType.SendCommand, () => FCommanding.Execute(Base.commandings, args) },
    { ColliderActionType.GiveEffect, () => FEffectGivingModule.Execute(Base.effectGivingModules, args) },
    { ColliderActionType.PlayAudio, () => FAudioModule.Execute(Base.AudioModules, args) },
    { ColliderActionType.CallGroovieNoise, () => FCGNModule.Execute(Base.GroovieNoiseToCall, args) },
    { ColliderActionType.CallFunction, () => FCFEModule.Execute(Base.FunctionToCall, args) },
};
            foreach (ColliderActionType type in Enum.GetValues(typeof(ColliderActionType)))
            {
                if (Base.ColliderActionType.HasFlag(type) && colliderActionExecutors.TryGetValue(type, out var execute))
                {
                    execute();
                }
            }
        }

        public new FCCDTO Base;
    }
}