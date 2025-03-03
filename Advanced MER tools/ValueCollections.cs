using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;

[Serializable]
public class Integer : Value
{
    public int Value;
    public override void OnValidate() { }
}

[Serializable]
public class Real : Value
{
    public float Value;
    public override void OnValidate() { }
}

[Serializable]
public class Bool : Value
{
    public bool Value;
    public override void OnValidate() { }
}

[Serializable]
public class String : Value
{
    public string Value;
    [Header("Replace {0}, {1}, ... to assgined values.")]
    public List<ScriptValue> StringInterpolations = new List<ScriptValue> { };
    public override void OnValidate() 
    {
        StringInterpolations.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class Compare : Value
{
    [Serializable]
    public enum CompareType
    {
        Equal,
        NotEqual,
        Bigger,
        BigOrEqual,
        Less,
        LessOrEqual,
        TypeEqual,
        And,
        Or,
        Xor,
        Not
    }

    public ScriptValue Value1;
    public CompareType Operator;
    public ScriptValue Value2;

    public override void OnValidate()
    {
        Value1.OnValidate();
        Value2.OnValidate();
    }
}

[Serializable]
public class IfThenElse : Value
{
    public ScriptValue Statement;
    public ScriptValue Then;
    public ScriptValue Else;
    public override void OnValidate()
    {
        Statement.OnValidate();
        Then.OnValidate();
        Else.OnValidate();
    }
}

[Serializable]
public class Array : Value
{
    public ScriptValue[] Values;
    public override void OnValidate()
    {
        for (int i = 0; Values != null && i < Values.Length; i++)
            Values[i].OnValidate();
    }
}

[Serializable]
public class Variable : Value
{
    public ScriptValue VariableName;
    public ScriptValue AccessLevel;
    public override void OnValidate()
    {
        VariableName.OnValidate();
        AccessLevel.OnValidate();
    }
}

[Serializable]
public class Argument : Value
{
    public ScriptValue ArgumentName;
    public override void OnValidate()
    {
        ArgumentName.OnValidate();
    }
}

[Serializable]
public class VFunction : Value
{
    public ScriptValue FunctionName;
    public List<ScriptValue> Arguments;
    public override void OnValidate()
    {
        FunctionName.OnValidate();
        Arguments.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class Vector : Value
{
    public ScriptValue X;
    public ScriptValue Y;
    public ScriptValue Z;
    public override void OnValidate()
    {
        X.OnValidate();
        Y.OnValidate();
        Z.OnValidate();
    }
}

[Serializable]
public class NumUnaryOp : Value
{
    [Serializable]
    public enum NumUnaryOpType
    {
        Inverse,
        Reciprocal,
        Absolute,
        Factorial,
        Ceiling,
        Floor,
        Round,
        CommonLog,
        NaturalLog,
        BinaryLog,
        Exponential,
        Sine,
        Cosine,
        Tangent,
        Arcsine,
        Arccosine,
        Arctangent,
        Sigmoid,
        IsPrimeNumber
    }

    public NumUnaryOpType Operator;
    public ScriptValue Value;

    public override void OnValidate()
    {
        Value.OnValidate();
    }
}

[Serializable]
public class NumBinomialOp : Value
{
    [Serializable]
    public enum NumBiOpType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        RaiseToPower,
        Log,
        Max,
        Min,
        Permutation,
        Combination,
        Random,
        GCD,
        LCM,
    }

    public NumBiOpType Operator;
    public ScriptValue Value1;
    public ScriptValue Value2;

    public override void OnValidate()
    {
        Value1.OnValidate();
        Value2.OnValidate();
    }

}

[Serializable]
public class ArrUnaryOp : Value
{
    [Serializable]
    public enum ArrUnaryOpType
    {
        Length,
        Reverse,
        GetRandomValue,
        Shuffled,
        RemovedNull,
        IsEmpty,
    }

    public ArrUnaryOpType Operator;
    public ScriptValue Array;

    public override void OnValidate()
    {
        Array.OnValidate();
    }
}

[Serializable]
public class ArrBinomialOp : Value
{
    [Serializable]
    public enum ArrBiOpType
    {
        AppendToArray,
        RemoveFromArray,
        Skip,
        SkipLast,
        FilteredArray,
        MappedArray,
        SortedArray,
        TrueForAll,
        TrueForAny,
        Contains,
        ElementAt,
        IndexOfElement,
        Count,
        Max,
        Min,
        Sum,
        Product,
        Distinct,
        Union,
        Intersect,
        DifferenceOfSets,
        Disjoint,
        Repeat,
    }

    public ArrBiOpType Operator;
    public ScriptValue Array;
    public ScriptValue EvaluationRule;

    public override void OnValidate()
    {
        Array.OnValidate();
        EvaluationRule.OnValidate();
    }
}

[Serializable]
public class VecUnaryOp : Value
{
    [Serializable]
    public enum VecUnaryOpType
    {
        X,
        Y,
        Z,
        Normalized,
        Magnitude,
        Inverse,
        DirectionFromAngles,
        AnglesFromDirection
    }

    public VecUnaryOpType Operator;
    public ScriptValue Vector;

    public override void OnValidate()
    {
        Vector.OnValidate();
    }
}

[Serializable]
public class VecBinomialOp : Value
{
    [Serializable]
    public enum VecBiOpType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Distance,
        AngleBetweenVectors,
        AngleDifference,
        DotProduct,
        CrossProduct,
        DirectionTowards
    }

    public VecBiOpType Operator;
    public ScriptValue Value1;
    public ScriptValue Value2;

    public override void OnValidate()
    {
        Value1.OnValidate();
        Value2.OnValidate();
    }
}

[Serializable]
public class StrUnaryOp : Value
{
    [Serializable]
    public enum StrUnaryOpType
    {
        Length,
        Upper,
        Lower,
        UpperLowerSwitch,
        Strip,
        Trim,
        ToInteger,
        ToReal,
        ToCharArray,
        ToPlayerAsName,
        ToItemAsName
    }

    public StrUnaryOpType Operator;
    public ScriptValue String;

    public override void OnValidate()
    {
        String.OnValidate();
    }
}

[Serializable]
public class StrBinomialOp : Value
{
    [Serializable]
    public enum StrBinomialOpType
    {
        Add,
        Repeat,
        Contains,
        Split,
        Skip,
        SkipLast,
    }

    public StrBinomialOpType Operator;
    public ScriptValue String;
    public ScriptValue EvaluationRule;

    public override void OnValidate()
    {
        String.OnValidate();
        EvaluationRule.OnValidate();
    }
}

[Serializable]
public class ArrayEvaluateHelper : Value
{
    [Serializable]
    public enum AEVHelperType
    {
        CurrentEvaluateElement,
        CurrentEvaluateIndex
    }

    public AEVHelperType Value;
    [Header("Increase with its nested level from 0.")]
    public ScriptValue AccessLevel;

    public override void OnValidate()
    {
        AccessLevel.OnValidate();
    }
}

[Serializable]
public class ConstValue : Value
{
    [Serializable]
    public enum ConstValueType
    {
        PI,
        E,
        GoldenRatio,
        C,
        TheAnswerToLifeTheUniverseAndEverything
    }

    public ConstValueType Value;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class EvaluateOnce : Value
{
    public ScriptValue Value;

    public override void OnValidate()
    {
        Value.OnValidate();
    }
}

[Serializable]
public class VCollisionType : Value
{
    public CollisionType Value;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class CollisionDetectTarget : Value
{
    public DetectType Value;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class EffectActionType : Value
{
    public EffectFlag Value;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class VEffectType : Value
{
    public EffectType Value;

    public override void OnValidate()
    {
    }
}

//[Serializable]
//public class VTeleportInvokeType : Value
//{
//    public TeleportInvokeType Value;

//    public override void OnValidate()
//    {
//    }
//}

[Serializable]
public class VWarheadActionType : Value
{
    public WarheadActionType Value;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class AnimationActionType : Value
{
    public AnimationType Value;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class VParameterType : Value
{
    public ParameterType Value;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class VMessageType : Value
{
    public MessageType Value;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class PlayerArray : Value
{
    public enum PlayerArrayType
    {
        AllPlayers,
        AlivePlayers,
        Scps,
        ScpsExcludeScp0492,
        Mtfs,
        Chaos,
        Dclass,
        Scientist,
        Spectators,
        Guards,
        FoundationSide,
        AntiFoundationSide,
        Humans,
    }

    public PlayerArrayType ArrayType;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class SingleTarget : Value
{
    public enum SingleTargetType
    {
        EventPlayer,
        SchematicEntity,
        ScriptEntity,
    };

    public SingleTargetType TargetType;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class VScp914Mode : Value
{
    public Scp914Mode Value;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class VItemType : Value
{
    public ItemType Value;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class VRoleType : Value
{
    public RoleTypeId Value;

    public override void OnValidate()
    {
    }
}

[Serializable]
public class ItemUnaryOp : Value
{
    [Serializable]
    public enum ItemUnaryOpType
    {
        ItemType,
        Entity,
        Owner,
        PrevOwner
    }

    public ScriptValue ItemOrPickup;
    public ItemUnaryOpType Operator;

    public override void OnValidate()
    {
        ItemOrPickup.OnValidate();
    }
}

[Serializable]
public class PlayerUnaryOp : Value
{
    [Serializable]
    public enum PlayerUnaryOpType
    {
        AHP,
        Cuffer,
        CurrentItem,
        CurrentSpectatingPlayers,
        CustomInfo,
        CustomName,
        DisplayNickname,
        GroupName,
        HP,
        HumeShield,
        Id,
        IsAlive,
        IsCHI,
        IsCuffed,
        IsDead,
        IsFoundationSide,
        IsFFon,
        IsHuman,
        IsInPocketDimension,
        IsInventoryEmpty,
        IsInventoryFull,
        IsJumping,
        IsNPC,
        IsNTF,
        IsReloading,
        IsScp,
        IsSpeaking,
        IsTutorial,
        IsUsingStamina,
        Items,
        MaxAHP,
        MaxHP,
        MaxHumeShield,
        Position,
        Role,
        FacingDirection,
        Scale,
        Stamina,
        EntityObject,
        UniqueRole,
        Velocity,
    }

    public PlayerUnaryOpType Operator;
    public ScriptValue Player;

    public override void OnValidate()
    {
        Player.OnValidate();
    }
}

[Serializable]
public class EntityUnaryOp : Value
{
    [Serializable]
    public enum EntityUnaryOpType
    {
        Name,
        Position,
        Rotation,
        Scale,
        Parent,
        IsActive,
        ChildCount,
        ToPlayer,
        ToItem,
    }

    public EntityUnaryOpType Operator;
    public ScriptValue Entity;

    public override void OnValidate()
    {
        Entity.OnValidate();
    }
}

[Serializable]
public class EntityBinomialOp : Value
{
    [Serializable]
    public enum EntityBinomialOpType
    {
        GetChildAt,
        WorldToLocal,
        LocalToWorld
    }

    public EntityBinomialOpType Operator;
    public ScriptValue Entity;
    public ScriptValue Value;

    public override void OnValidate()
    {
        Entity.OnValidate();
        Value.OnValidate();
    }
}