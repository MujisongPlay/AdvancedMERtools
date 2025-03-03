using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class FunctionExecutor : MonoBehaviour
{
    private void OnValidate()
    {
        try
        {
            data.OnValidate();
        }
        catch { }
    }

    public FEDTO data;
}

[Serializable]
public class FEDTO
{
    public void OnValidate()
    {
        Conditions.ForEach(x => x.OnValidate());
        Actions.ForEach(x => x.OnValidate());
    }

    public string FunctionName;
    public string[] ArgumentsName;
    public List<ScriptValue> Conditions;
    public List<ScriptAction> Actions;
}


[Serializable]
public class ScriptAction
{
    public FunctionType ActionType;
    [SerializeReference]
    public Function function;

    public void OnValidate()
    {
        if (!IsFunctionTypeMatch(function, ActionType))
        {
            function = (Function)CreateFunctionInstance(ActionType);
        }
        function.OnValidate();
    }

    private bool IsFunctionTypeMatch(object functionInstance, FunctionType actionType)
    {
        if (functionInstance == null)
            return false;
        if (EnumToFunc.TryGetValue(actionType, out Type type))
            return functionInstance.GetType() == type;
        else
            return false;
    }

    private object CreateFunctionInstance(FunctionType actionType)
    {
        if (EnumToFunc.TryGetValue(actionType, out Type type))
            return Activator.CreateInstance(type);
        return Activator.CreateInstance(typeof(DFunction));
    }

    public static readonly Dictionary<FunctionType, Type> EnumToFunc = new Dictionary<FunctionType, Type>
    {
        { FunctionType.If, typeof(If) },
        { FunctionType.ElseIf, typeof(ElseIf) },
        { FunctionType.Else, typeof(Else) },
        { FunctionType.While, typeof(While) },
        { FunctionType.For, typeof(For) },
        { FunctionType.ForEach, typeof(ForEach) },
        { FunctionType.SetVariable, typeof(SetVariable) },
        { FunctionType.Return, typeof(Return) },
        { FunctionType.Wait, typeof(Wait) },
        { FunctionType.CallFunction, typeof(CallFunction) },
        { FunctionType.CallGroovyNoise, typeof(CallGroovyNoise) },
        { FunctionType.PlayAnimation, typeof(PlayAnimation) },
        { FunctionType.SendMessage, typeof(SendMessage) },
        { FunctionType.SendCommand, typeof(SendCommand) },
        { FunctionType.DropItems, typeof(DropItems) },
        { FunctionType.Explode, typeof(Explode) },
        { FunctionType.GiveEffect, typeof(GiveEffect) },
        { FunctionType.PlayAudio, typeof(PlayAudio) },
        { FunctionType.Warhead, typeof(FWarhead) },
        { FunctionType.ChangePlayerValue, typeof(ChangePlayerValue) },
        { FunctionType.PlayerAction, typeof(PlayerAction) },
        { FunctionType.ChangeEntityValue, typeof(ChangeEntityValue) }
    };
}

[Serializable]
public enum FunctionType
{
    If,
    ElseIf,
    Else,
    While,
    For,
    ForEach,
    Break,
    Continue,
    SetVariable,
    Return,
    Wait,
    CallFunction,
    CallGroovyNoise,
    PlayAnimation,
    SendMessage,
    SendCommand,
    DropItems,
    Explode,
    GiveEffect,
    PlayAudio,
    Warhead,
    ChangePlayerValue,
    PlayerAction,
    ChangeEntityValue
}

[Serializable]
public abstract class Function
{
    public abstract void OnValidate();
}

[Serializable]
public class DFunction : Function
{
    public override void OnValidate()
    {
    }
}

[Serializable]
public class ScriptValue
{
    public ValueType ValueType;
    [SerializeReference]
    public Value value;

    public void OnValidate()
    {
        if (!IsValueTypeMatch(value, ValueType))
        {
            value = (Value)CreateValueInstance(ValueType);
        }
        value.OnValidate();
    }

    private bool IsValueTypeMatch(object functionInstance, ValueType valueType)
    {
        if (functionInstance == null)
            return false;
        if (EnumToV.TryGetValue(valueType, out Type type))
            return functionInstance.GetType() == type;
        else
            return false;
    }

    private object CreateValueInstance(ValueType valueType)
    {
        if (EnumToV.TryGetValue(valueType, out Type type))
            return Activator.CreateInstance(type);
        return Activator.CreateInstance(typeof(DValue));
    }

    public static readonly Dictionary<ValueType, Type> EnumToV = new Dictionary<ValueType, Type>
    {
        { ValueType.Integer, typeof(Integer) },
        { ValueType.Real, typeof(Real) },
        { ValueType.Bool, typeof(Bool) },
        { ValueType.String, typeof(String) },
        { ValueType.Compare, typeof(Compare) },
        { ValueType.IfThenElse, typeof(IfThenElse) },
        { ValueType.Array, typeof(Array) },
        { ValueType.Variable, typeof(Variable) },
        { ValueType.Argument, typeof(Argument) },
        { ValueType.Function, typeof(VFunction) },
        { ValueType.Vector, typeof(Vector) },
        { ValueType.NumUnaryOp, typeof(NumUnaryOp) },
        { ValueType.NumBinomialOp, typeof(NumBinomialOp) },
        { ValueType.ArrUnaryOp, typeof(ArrUnaryOp) },
        { ValueType.ArrBinomialOp, typeof(ArrBinomialOp) },
        { ValueType.VecUnaryOp, typeof(VecUnaryOp) },
        { ValueType.VecBinomialOp, typeof(VecBinomialOp) },
        { ValueType.StrUnaryOp, typeof(StrUnaryOp) },
        { ValueType.StrBinomialOp, typeof(StrBinomialOp) },
        { ValueType.ArrayEvaluateHelper, typeof(ArrayEvaluateHelper) },
        { ValueType.ConstValue, typeof(ConstValue) },
        { ValueType.EvaluateOnce, typeof(EvaluateOnce) },
        { ValueType.CollisionType, typeof(VCollisionType) },
        { ValueType.CollisionDetectTarget, typeof(CollisionDetectTarget) },
        { ValueType.EffectType, typeof(VEffectType) },
        { ValueType.EffectActionType, typeof(EffectActionType) },
        //{ ValueType.TeleportInvokeType, typeof(VTeleportInvokeType) },
        { ValueType.WarheadActionType, typeof(VWarheadActionType) },
        { ValueType.AnimationActionType, typeof(AnimationActionType) },
        { ValueType.ParameterType, typeof(VParameterType) },
        { ValueType.MessageType, typeof(VMessageType) },
        { ValueType.PlayerArray, typeof(PlayerArray) },
        { ValueType.Scp914Mode, typeof(VScp914Mode) },
        { ValueType.ItemType, typeof(VItemType) },
        { ValueType.RoleType, typeof(VRoleType) },
        { ValueType.PlayerUnaryOp, typeof(PlayerUnaryOp) },
        { ValueType.SingleTarget, typeof(SingleTarget) },
        { ValueType.ItemUnaryOp, typeof(ItemUnaryOp) },
        { ValueType.EntityUnaryOp, typeof(EntityUnaryOp) },
        { ValueType.EntityBinomialOp, typeof(EntityBinomialOp) }
    };
}

[Serializable]
public enum ValueType
{
    Integer,
    Real,
    Bool,
    String,
    Null,
    Compare,
    IfThenElse,
    EmptyArray,
    Array,
    Variable,
    Argument,
    Function,
    ZeroVector,
    Vector,
    NumUnaryOp,
    NumBinomialOp,
    ArrUnaryOp,
    ArrBinomialOp,
    VecUnaryOp,
    VecBinomialOp,
    StrUnaryOp,
    StrBinomialOp,
    ArrayEvaluateHelper,
    ConstValue,
    EvaluateOnce,
    CollisionType,
    CollisionDetectTarget,
    EffectType,
    EffectActionType,
    WarheadActionType,
    AnimationActionType,
    ParameterType,
    MessageType,
    PlayerArray,
    Scp914Mode,
    ItemType,
    RoleType,
    SingleTarget,
    ItemUnaryOp,
    PlayerUnaryOp,
    EntityUnaryOp,
    EntityBinomialOp
}

[Serializable]
public abstract class Value
{
    public abstract void OnValidate();
}

[Serializable]
public class DValue : Value
{
    public override void OnValidate()
    {
    }
}

public class FunctionExecutorCompiler
{
    private static readonly Config Config = SchematicManager.Config;

    [MenuItem("SchematicManager/Compile Function Executor", priority = -11)]
    public static void OnCompile()
    {
        foreach (Schematic schematic in GameObject.FindObjectsOfType<Schematic>())
        {
            Compile(schematic);
        }
    }

    public static void Compile(Schematic schematic)
    {
        string parentDirectoryPath = Directory.Exists(Config.ExportPath)
            ? Config.ExportPath
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "MapEditorReborn_CompiledSchematics");
        string schematicDirectoryPath = Path.Combine(parentDirectoryPath, schematic.gameObject.name);

        if (!Directory.Exists(parentDirectoryPath))
        {
            Debug.LogError("Could Not find root object's compiled directory!");
            return;
        }

        Directory.CreateDirectory(schematicDirectoryPath);

        List<FEDTO> interactables = new List<FEDTO> { };

        foreach (FunctionExecutor ip in schematic.transform.GetComponentsInChildren<FunctionExecutor>())
        {
            interactables.Add(ip.data);
        }

        string serializedData = JsonConvert.SerializeObject(interactables, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto }).Replace("Assembly-CSharp", "AdvancedMERTools");

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-Functions.json"), serializedData);
        Debug.Log("Successfully Imported Functions.");
    }
}