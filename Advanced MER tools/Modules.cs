using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class FakeMono : MonoBehaviour
{
	[Header("Used for GroovyNoise")]
	[JsonIgnore]
	public int ScriptId;
	public string ScriptGroup;
	public bool UseScriptValue;
	public DTO data;
	public DTO ScriptValueData;
	public virtual DTO _data { get => data; }
	public virtual DTO _ScriptValueData { get => ScriptValueData; }
	private void OnValidate()
	{
		PublicFunctions.AMERTs.Add(this);
		int t = GetInstanceID();
		if (ScriptId != t)
		{
			PublicFunctions.OnIDChange(ScriptId, t);
			ScriptId = t;
		}
		if (_data != null && _ScriptValueData != null)
        {
			_data.parent = this;
			_ScriptValueData.parent = this;
			_data.Code = _ScriptValueData.Code = ScriptId;
			_data.UseScriptValue = _ScriptValueData.UseScriptValue = UseScriptValue;
			_data.ScriptGroup = _ScriptValueData.ScriptGroup = ScriptGroup;
			_data.ObjectId = _ScriptValueData.ObjectId = PublicFunctions.FindPath(transform);
			try
			{
				_data.OnValidate();
			}
			catch { }
			try
			{
				_ScriptValueData.OnValidate();
			}
			catch { }
		}
	}
}

[Serializable]
public abstract class DTO
{
	public abstract void OnValidate();

	public bool Active;
	[HideInInspector]
	public string ObjectId;
	[HideInInspector]
	public int Code;
	[HideInInspector]
	public string ScriptGroup;
	[HideInInspector]
	public bool UseScriptValue;
	[HideInInspector]
	[JsonIgnore]
	public FakeMono parent;
}

[Serializable]
public class CGNModule : RandomExecutionModule
{
	public int GroovieNoiseId;
	public string GroovieNoiseGroup;
}

[Serializable]
public class FCGNModule : FRandomExecutionModule
{
    public override void OnValidate()
    {
		base.OnValidate();
		GroovieNoiseId.OnValidate();
		GroovieNoiseGroup.OnValidate();
    }

    public ScriptValue GroovieNoiseId;
	public ScriptValue GroovieNoiseGroup;
}

[Serializable]
public class CFEModule : RandomExecutionModule
{
	public string FunctionName;
}

[Serializable]
public class FCFEModule : FRandomExecutionModule
{
    public override void OnValidate()
    {
		base.OnValidate();
		FunctionName.OnValidate();
		FunctionArguments.ForEach(x => x.OnValidate());
    }

    public ScriptValue FunctionName;
	public List<ScriptValue> FunctionArguments;
}

[Serializable]
public class EffectGivingModule : RandomExecutionModule
{
	public EffectFlag EffectFlag;
	public EffectType effectType;
	public SendType GivingTo;
	public byte Inensity;
	public float Duration;
}

[Serializable]
public class FEffectGivingModule : FRandomExecutionModule
{
    public override void OnValidate()
    {
        base.OnValidate();
		EffectFlag.OnValidate();
		effectType.OnValidate();
		GivingTo.OnValidate();
		Inensity.OnValidate();
		Duration.OnValidate();
    }

    public ScriptValue EffectFlag;
	public ScriptValue effectType;
	public ScriptValue GivingTo;
	public ScriptValue Inensity;
	public ScriptValue Duration;
}

[Serializable]
public class ExplodeModule : RandomExecutionModule
{
	public bool FFon;
	public bool EffectOnly;
	public SVector3 LocalPosition;
}

[Serializable]
public class FExplodeModule : FRandomExecutionModule
{
    public override void OnValidate()
    {
        base.OnValidate();
		FFon.OnValidate();
		EffectOnly.OnValidate();
		LocalPosition.OnValidate();
    }

    public ScriptValue FFon;
	public ScriptValue EffectOnly;
	public ScriptValue LocalPosition;
}

[Serializable]
public class AnimationDTO : RandomExecutionModule
{
	[JsonIgnore]
	public GameObject Animator;
	[HideInInspector]
	public string AnimatorAdress;
	public string AnimationName;
	public AnimationType AnimationType;
	public ParameterType ParameterType;
	public string ParameterName;
	[Header("If parameter type is bool or trigger, input 0 for false, and input 1 for true.")]
	public string ParameterValue;
}

[Serializable]
public class FAnimationDTO : FRandomExecutionModule
{
    public override void OnValidate()
    {
        base.OnValidate();
		AnimationName.OnValidate();
		AnimationType.OnValidate();
		ParameterType.OnValidate();
		ParameterName.OnValidate();
		ParameterValue.OnValidate();
		AnimatorAdress = PublicFunctions.FindPath(Animator);
    }

    [JsonIgnore]
	public GameObject Animator;
	[HideInInspector]
	public string AnimatorAdress;
	public ScriptValue AnimationName;
	public ScriptValue AnimationType;
	public ScriptValue ParameterType;
	public ScriptValue ParameterName;
	[Header("If parameter type is bool or trigger, input 0 for false, and input 1 for true.")]
	public ScriptValue ParameterValue;
}

[Serializable]
public class GMDTO : RandomExecutionModule
{
	public List<int> Targets;
	public List<string> TargetGroups;
	public bool Enable;
}

[Serializable]
public class FGMDTO : FRandomExecutionModule
{
    public override void OnValidate()
    {
        base.OnValidate();
		Targets.OnValidate();
		TargetGroups.OnValidate();
		Enable.OnValidate();
    }

    public ScriptValue Targets;
	public ScriptValue TargetGroups;
	public ScriptValue Enable;
}

[Serializable]
public class MessageModule : RandomExecutionModule
{
	public SendType SendType;
	[Tooltip("{p_i} = player id.\n{p_name}\n{p_pos}\n{p_room}\n{p_zone}\n{p_role}\n{o_pos} = object's exact position.\n{o_room}\n{o_zone}\n{p_item} = player's current item." +
		"\n{damage} (HealthObject only)")]
	public string MessageContent;
	public MessageType MessageType;
	public float Duration;
}

[Serializable]
public class FMessageModule : FRandomExecutionModule
{
    public override void OnValidate()
    {
        base.OnValidate();
		SendType.OnValidate();
		MessageContent.OnValidate();
		MessageType.OnValidate();
		Duration.OnValidate();
    }

    public ScriptValue SendType;
	public ScriptValue MessageContent;
	public ScriptValue MessageType;
	public ScriptValue Duration;
}

[Serializable]
public class AudioModule : RandomExecutionModule
{
	public string AudioName;
	[Header("0: Loop")]
	public int PlayCount;
	public bool IsSpatial;
	public float MaxDistance;
	public float MinDistance;
	public float Volume;
	public SVector3 LocalPlayPosition;
}

[Serializable]
public class FAudioModule : FRandomExecutionModule
{
    public override void OnValidate()
    {
        base.OnValidate();
		AudioName.OnValidate();
		PlayCount.OnValidate();
		IsSpatial.OnValidate();
		MaxDistance.OnValidate();
		MinDistance.OnValidate();
		Volume.OnValidate();
		LocalPlayPosition.OnValidate();
    }

    public ScriptValue AudioName;
	[Header("0: Loop")]
	public ScriptValue PlayCount;
	public ScriptValue IsSpatial;
	public ScriptValue MaxDistance;
	public ScriptValue MinDistance;
	public ScriptValue Volume;
	public ScriptValue LocalPlayPosition;
}

[Serializable]
public class RandomExecutionModule
{
	public float ChanceWeight;
	public bool ForceExecute;
	public float ActionDelay;
}

[Serializable]
public class FRandomExecutionModule : Value
{
    public override void OnValidate()
    {
		ChanceWeight.OnValidate();
		ForceExecute.OnValidate();
		ActionDelay.OnValidate();
    }

	[JsonIgnore]
	[HideInInspector]
	public DTO parent;
    public ScriptValue ChanceWeight;
	public ScriptValue ForceExecute;
	public ScriptValue ActionDelay;
}

[Serializable]
public class DropItem : RandomExecutionModule
{
	public ItemType ItemType;
	public uint CustomItemId;
	public int Count;
	public SVector3 DropLocalPosition;
}

[Serializable]
public class FDropItem : FRandomExecutionModule
{
    public override void OnValidate()
    {
        base.OnValidate();
		ItemType.OnValidate();
		CustomItemId.OnValidate();
		Count.OnValidate();
		DropLocalPosition.OnValidate();
    }

    public ScriptValue ItemType;
	public ScriptValue CustomItemId;
	public ScriptValue Count;
	public ScriptValue DropLocalPosition;
}

[Serializable]
public class Commanding : RandomExecutionModule
{
	[Tooltip("{p_i} = player id.\n{p_name}\n{p_pos}\n{p_room}\n{p_zone}\n{p_role}\n{o_pos} = object's exact position.\n{o_room}\n{o_zone}\n{p_item} = attacker's current item." +
		"\n{damage} (HealthObject only)")]
	public string CommandContext;
}

[Serializable]
public class FCommanding : FRandomExecutionModule
{
	public ScriptValue CommandContext;

    public override void OnValidate()
    {
		CommandContext.OnValidate();
    }
}

[Serializable]
public class SVector3
{
	public float x;
	public float y;
	public float z;
}

[Serializable]
public class WhitelistWeapon
{
	public ItemType ItemType;
	public uint CustomItemId;
}

[Serializable]
public class FWhitelistWeapon : Value
{
    public override void OnValidate()
    {
		ItemType.OnValidate();
		CustomItemId.OnValidate();
    }

    public ScriptValue ItemType;
	public ScriptValue CustomItemId;
}