using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Exiled.API.Enums;
using System.Linq;
using Exiled.API.Features;
using InventorySystem.Items.Pickups;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using PlayerRoles;

namespace AdvancedMERTools
{

    [Serializable]
    public class If : ActionsFunctioner
    {
        public ScriptValue Statement;

        public override void OnValidate()
        {
            Statement.OnValidate();
            Actions.ForEach(x => x.OnValidate());
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            if (!ConditionCheck(args, Statement))
                return new FunctionReturn { result = FunctionResult.FunctionCheck, value = false };
            return ExecuteActions(args, FunctionResult.FunctionCheck);
        }
    }

    [Serializable]
    public class ElseIf : ActionsFunctioner
    {
        public ScriptValue Statement;

        public override void OnValidate()
        {
            Statement.OnValidate();
            Actions.ForEach(x => x.OnValidate());
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            if (!ConditionCheck(args, Statement))
                return new FunctionReturn { result = FunctionResult.FunctionCheck, value = false };
            return ExecuteActions(args, FunctionResult.FunctionCheck);
        }
    }

    [Serializable]
    public class Else : ActionsFunctioner
    {
        public override void OnValidate()
        {
            Actions.ForEach(x => x.OnValidate());
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            return ExecuteActions(args);
        }
    }

    [Serializable]
    public class While : ActionsFunctioner
    {
        public ScriptValue Condition;

        public override void OnValidate()
        {
            Condition.OnValidate();
            Actions.ForEach(x => x.OnValidate());
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            while (true)
            {
                if (!ConditionCheck(args, Condition))
                    return new FunctionReturn();
                FunctionReturn result = ExecuteActions(args);
                switch (result.result)
                {
                    case FunctionResult.Break:
                        return new FunctionReturn();
                    case FunctionResult.Return:
                        return result;
                }
            }
        }
    }

    [Serializable]
    public class For : ActionsFunctioner
    {
        public ScriptValue RepeatCount;

        public override void OnValidate()
        {
            RepeatCount.OnValidate();
            Actions.ForEach(x => x.OnValidate());
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            int n = RepeatCount.GetValue(args, 1);
            for (int i = 0; i < n; i++)
            {
                FunctionReturn result = ExecuteActions(args);
                switch (result.result)
                {
                    case FunctionResult.Break:
                        return new FunctionReturn();
                    case FunctionResult.Return:
                        return result;
                }
            }
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class ForEach : ActionsFunctioner
    {
        public ScriptValue Array;
        public string ControlVariable;

        public override void OnValidate()
        {
            Array.OnValidate();
            Actions.ForEach(x => x.OnValidate());
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            object obj = Array.GetValue(args);
            if (obj == null || !(obj is object[]))
                return new FunctionReturn();
            object[] arr = (object[])obj;
            for (int i = 0; i < arr.Length; i++)
            {
                args.FunctionVariables[ControlVariable] = arr[i];
                FunctionReturn result = ExecuteActions(args);
                switch (result.result)
                {
                    case FunctionResult.Break:
                        return new FunctionReturn();
                    case FunctionResult.Return:
                        return result;
                }
            }
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class SetVariable : Function
    {
        public ScriptValue VariableName;
        public ScriptValue ValueToAssign;
        [Header("0: Function, 1: Script, 2: Schematic, 3: Game")]
        public ScriptValue AccessLevel;

        public override void OnValidate()
        {
            VariableName.OnValidate();
            ValueToAssign.OnValidate();
            AccessLevel.OnValidate();
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            object obj = VariableName.GetValue(args);
            object obj2 = AccessLevel.GetValue(args);
            object v = ValueToAssign.GetValue(args);
            if (obj != null && obj is string && obj2 != null && (obj2 is int || obj2 is float))
            {
                string str = Convert.ToString(obj);
                int val = Math.Min(3, Math.Max(0, Mathf.RoundToInt(Convert.ToSingle(obj2))));
                switch (val)
                {
                    case 0:
                        args.FunctionVariables[str] = v;
                        break;
                    case 1:
                        args.Function.ScriptVariables[str] = v;
                        break;
                    case 2:
                        AdvancedMERTools.Singleton.SchematicVariables[args.Function.OSchematic][str] = v;
                        break;
                    case 3:
                        AdvancedMERTools.Singleton.RoundVariable[str] = v;
                        break;
                }
            }
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class Return : Function
    {
        public ScriptValue ReturnValue;

        public override void OnValidate()
        {
            ReturnValue.OnValidate();
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            return new FunctionReturn { value = ReturnValue.GetValue(args), result = FunctionResult.Return };
        }
    }

    [Serializable]
    public class Wait : Function
    {
        public ScriptValue WaitSecond;

        public override void OnValidate()
        {
            WaitSecond.OnValidate();
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            return new FunctionReturn { result = FunctionResult.Wait, value = WaitSecond.GetValue(args, 0f) };
        }
    }

    [Serializable]
    public class CallFunction : Function
    {
        public List<FCFEModule> FunctionModules;

        public override void OnValidate()
        {
            //FunctionModules.ForEach(x => x.OnValidate());
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            FCFEModule.Execute(FunctionModules, args);
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class CallGroovyNoise : Function
    {
        [Header("Caution: IDs won't be updated automatically!!")]
        public List<FCGNModule> Modules;

        public override void OnValidate()
        {
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            FCGNModule.Execute(Modules, args);
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class PlayAnimation : Function
    {
        public List<FAnimationDTO> AnimationModules;

        public override void OnValidate()
        {
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            FCGNModule.Execute(AnimationModules, args);
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class SendMessage : Function
    {
        public List<FMessageModule> MessageModules;

        public override void OnValidate()
        {
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            FCGNModule.Execute(MessageModules, args);
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class SendCommand : Function
    {
        public List<FCommanding> CommandModules;

        public override void OnValidate()
        {
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            FCGNModule.Execute(CommandModules, args);
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class DropItems : Function
    {
        public List<FDropItem> DropItemsModules;

        public override void OnValidate()
        {
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            FCGNModule.Execute(DropItemsModules, args);
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class Explode : Function
    {
        public List<FExplodeModule> ExplodeModules;

        public override void OnValidate()
        {
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            FCGNModule.Execute(ExplodeModules, args);
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class GiveEffect : Function
    {
        public List<FEffectGivingModule> EffectModules;

        public override void OnValidate()
        {
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            FCGNModule.Execute(EffectModules, args);
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class PlayAudio : Function
    {
        public List<FAudioModule> AudioModules;

        public override void OnValidate()
        {
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            FCGNModule.Execute(AudioModules, args);
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class FWarhead : Function
    {
        public ScriptValue ActionType;

        public override void OnValidate()
        {
            ActionType.OnValidate();
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            AMERTInteractable.AlphaWarhead(ActionType.GetValue(args, WarheadActionType.Start));
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class ChangePlayerValue : Function
    {
        public ScriptValue player;
        public PlayerUnaryOp.PlayerUnaryOpType ValueType;
        public ScriptValue Value;

        public override void OnValidate()
        {
            player.OnValidate();
            Value.OnValidate();
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            Player p = this.player.GetValue<Player>(args, null);
            if (p == null)
                return new FunctionReturn();
            object obj = Value.GetValue(args);
            int Number = obj is int || obj is float ? (int)obj : 0;
            float Real = obj is float || obj is int ? (float)obj : 0;
            bool Bool = obj is bool ? (bool)obj : false;
            string str = obj is string ? (string)obj : "";
            Player player = obj is Player ? (Player)obj : null;
            Pickup pickup = obj is Pickup ? (Pickup)obj : null;
            Item item = obj is Item ? (Item)obj : null;
            Vector3 vector = obj is Vector3 ? (Vector3)obj : Vector3.zero;
            ItemType it = obj is ItemType ? (ItemType)obj : ItemType.None;
            RoleTypeId roleType = obj is RoleTypeId ? (RoleTypeId)obj : RoleTypeId.ClassD;
            switch (ValueType)
            {
                case PlayerUnaryOp.PlayerUnaryOpType.AHP:
                    p.ArtificialHealth = Real;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.Cuffer:
                    p.Cuffer = player;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.CurrentItem:
                    p.CurrentItem = item;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.CustomInfo:
                    p.CustomInfo = str;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.CustomName:
                    p.CustomName = str;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.DisplayNickname:
                    p.DisplayNickname = str;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.GroupName:
                    p.GroupName = str;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.HP:
                    p.Health = Real;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.HumeShield:
                    p.HumeShield = Real;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.MaxAHP:
                    p.MaxArtificialHealth = Real;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.MaxHP:
                    p.MaxHealth = Real;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.MaxHumeShield:
                    p.MaxHumeShield = Real;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.Position:
                    p.Position = vector;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.Role:
                    p.Role.Set(roleType);
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.Scale:
                    p.Scale = vector;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.Stamina:
                    p.Stamina = Real;
                    break;
                case PlayerUnaryOp.PlayerUnaryOpType.UniqueRole:
                    p.UniqueRole = str;
                    break;
            }
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class PlayerAction : Function
    {
        [Serializable]
        public enum PlayerActionType
        {
            GiveItem,
            DropItem,
            RemoveItem
        }

        public ScriptValue Player;
        public PlayerActionType ActionType;
        public ScriptValue Argument;

        public override void OnValidate()
        {
            Player.OnValidate();
            Argument.OnValidate();
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            Player p = this.Player.GetValue<Player>(args, null);
            if (p == null)
                return new FunctionReturn();
            object obj = Argument.GetValue(args);
            int Number = obj is int || obj is float ? (int)obj : 0;
            float Real = obj is float || obj is int ? (float)obj : 0;
            bool Bool = obj is bool ? (bool)obj : false;
            string str = obj is string ? (string)obj : "";
            Player player = obj is Player ? (Player)obj : null;
            Pickup pickup = obj is Pickup ? (Pickup)obj : null;
            Item item = obj is Item ? (Item)obj : null;
            Vector3 vector = obj is Vector3 ? (Vector3)obj : Vector3.zero;
            ItemType it = obj is ItemType ? (ItemType)obj : ItemType.None;
            RoleTypeId roleType = obj is RoleTypeId ? (RoleTypeId)obj : RoleTypeId.ClassD;
            switch (ActionType)
            {
                case PlayerActionType.DropItem:
                    p.DropItem(item);
                    break;
                case PlayerActionType.GiveItem:
                    if (item != null)
                        item.Give(p);
                    else if (pickup != null)
                        p.AddItem(pickup);
                    else if (it != ItemType.None)
                        p.AddItem(it);
                    break;
                case PlayerActionType.RemoveItem:
                    p.RemoveItem(item, true);
                    break;
            }
            return new FunctionReturn();
        }
    }

    [Serializable]
    public class ChangeEntityValue : Function
    {
        public ScriptValue Entity;
        public EntityUnaryOp.EntityUnaryOpType ValueType;
        public ScriptValue Value;

        public override void OnValidate()
        {
            Entity.OnValidate();
            Value.OnValidate();
        }

        public override FunctionReturn Execute(FunctionArgument args)
        {
            GameObject game = Entity.GetValue<GameObject>(args, null);
            if (game == null)
                return new FunctionReturn();
            object obj = Value.GetValue(args);
            int Number = obj is int || obj is float ? (int)obj : 0;
            float Real = obj is float || obj is int ? (float)obj : 0;
            bool Bool = obj is bool ? (bool)obj : false;
            string str = obj is string ? (string)obj : "";
            Player player = obj is Player ? (Player)obj : null;
            Pickup pickup = obj is Pickup ? (Pickup)obj : null;
            GameObject go = obj is GameObject ? (GameObject)obj : null;
            Item item = obj is Item ? (Item)obj : null;
            Vector3 vector = obj is Vector3 ? (Vector3)obj : Vector3.zero;
            ItemType it = obj is ItemType ? (ItemType)obj : ItemType.None;
            RoleTypeId roleType = obj is RoleTypeId ? (RoleTypeId)obj : RoleTypeId.ClassD;
            switch (ValueType)
            {
                case EntityUnaryOp.EntityUnaryOpType.IsActive:
                    game.SetActive(Bool);
                    break;
                case EntityUnaryOp.EntityUnaryOpType.Name:
                    game.name = str;
                    break;
                case EntityUnaryOp.EntityUnaryOpType.Parent:
                    if (go == null)
                        break;
                    game.transform.parent = go.transform;
                    break;
                case EntityUnaryOp.EntityUnaryOpType.Position:
                    game.transform.position = vector;
                    break;
                case EntityUnaryOp.EntityUnaryOpType.Rotation:
                    game.transform.localEulerAngles = vector;
                    break;
                case EntityUnaryOp.EntityUnaryOpType.Scale:
                    game.transform.localScale = vector;
                    break;
            }
            return new FunctionReturn();
        }
    }
}
