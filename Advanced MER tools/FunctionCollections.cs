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
public class If : Function
{
    public ScriptValue Statement;
    public List<ScriptAction> Actions;

    public override void OnValidate()
    {
        Statement.OnValidate();
        Actions.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class ElseIf : Function
{
    public ScriptValue Statement;
    public List<ScriptAction> Actions;

    public override void OnValidate()
    {
        Statement.OnValidate();
        Actions.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class Else : Function
{
    public List<ScriptAction> Actions;

    public override void OnValidate()
    {
        Actions.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class While : Function
{
    public ScriptValue Condition;
    public List<ScriptAction> Actions;

    public override void OnValidate()
    {
        Condition.OnValidate();
        Actions.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class For : Function
{
    public ScriptValue RepeatCount;
    public List<ScriptAction> Actions;

    public override void OnValidate()
    {
        RepeatCount.OnValidate();
        Actions.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class ForEach : Function
{
    public ScriptValue Array;
    public string ControlVariable;
    public List<ScriptAction> Actions;

    public override void OnValidate()
    {
        Array.OnValidate();
        Actions.ForEach(x => x.OnValidate());
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
}

[Serializable]
public class Return : Function
{
    public ScriptValue ReturnValue;

    public override void OnValidate()
    {
        ReturnValue.OnValidate();
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
}

[Serializable]
public class CallFunction : Function
{
    public List<FCFEModule> FunctionModules;

    public override void OnValidate()
    {
        FunctionModules.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class CallGroovyNoise : Function
{
    [Header("Caution: IDs won't be updated automatically!!")]
    public List<FCGNModule> Modules;

    public override void OnValidate()
    {
        Modules.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class PlayAnimation : Function
{
    public List<FAnimationDTO> AnimationModules;

    public override void OnValidate()
    {
        AnimationModules.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class SendMessage : Function
{
    public List<FMessageModule> MessageModules;

    public override void OnValidate()
    {
        MessageModules.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class SendCommand : Function
{
    public List<FCommanding> CommandModules;

    public override void OnValidate()
    {
        CommandModules.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class DropItems : Function
{
    public List<FDropItem> DropItemsModules;

    public override void OnValidate()
    {
        DropItemsModules.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class Explode : Function
{
    public List<FExplodeModule> ExplodeModules;

    public override void OnValidate()
    {
        ExplodeModules.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class GiveEffect : Function
{
    public List<FEffectGivingModule> EffectModules;

    public override void OnValidate()
    {
        EffectModules.ForEach(x => x.OnValidate());
    }
}

[Serializable]
public class PlayAudio : Function
{
    public List<FAudioModule> AudioModules;

    public override void OnValidate()
    {
        AudioModules.ForEach(x => x.OnValidate());
    }
}