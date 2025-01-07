using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;
//using NaughtyAttributes;

public class FakeMono : MonoBehaviour
{
	[Header("Used for GroovyNoise")]
	[JsonIgnore]
	public int ScriptId;
	private void OnValidate()
	{
		PublicFunctions.AMERTs.Add(this);
		int t = GetInstanceID();
		if (ScriptId != t)
		{
			PublicFunctions.OnIDChange(ScriptId, t);
			ScriptId = t;
		}
	}
}

[Serializable]
public class CGNModule : RandomExecutionModule
{
	public int GroovieNoiseId;
	public bool Enable;
}

[Serializable]
public class DTO
{
	public bool Active;
	[HideInInspector]
	public string ObjectId;
	[HideInInspector]
	public int Code;
	//public List<CGNModule> GroovieNoiseToCall;
}

[Flags]
[Serializable]
public enum ColliderActionType
{
	ModifyHealth = 1,
	GiveEffect = 2,
	SendMessage = 4,
	PlayAnimation = 8,
	SendCommand = 16,
	Warhead = 32,
	Explode = 64,
	PlayAudio = 128,
	CallGroovieNoise = 256,
}

[Flags]
[Serializable]
public enum CollisionType
{
	OnEnter = 1,
	OnStay = 2,
	OnExit = 4,
}

[Flags]
[Serializable]
public enum DetectType
{
	Pickup = 1,
	Player = 2,
	Projectile = 4
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
public class ExplodeModule : RandomExecutionModule
{
	public bool FFon;
	public bool EffectOnly;
	public SVector3 LocalPosition;
}

[Flags]
[Serializable]
public enum EffectFlag
{
	Disable = 1,
	Enable = 2,
	ModifyDuration = 4,
	ForceDuration = 8,
	ModifyIntensity = 16,
	ForceIntensity = 32
}

public enum EffectType
{
	// Token: 0x0400034F RID: 847
	None,
	// Token: 0x04000350 RID: 848
	AmnesiaItems,
	// Token: 0x04000351 RID: 849
	AmnesiaVision,
	// Token: 0x04000352 RID: 850
	Asphyxiated,
	// Token: 0x04000353 RID: 851
	Bleeding,
	// Token: 0x04000354 RID: 852
	Blinded,
	// Token: 0x04000355 RID: 853
	Burned,
	// Token: 0x04000356 RID: 854
	Concussed,
	// Token: 0x04000357 RID: 855
	Corroding,
	// Token: 0x04000358 RID: 856
	Deafened,
	// Token: 0x04000359 RID: 857
	Decontaminating,
	// Token: 0x0400035A RID: 858
	Disabled,
	// Token: 0x0400035B RID: 859
	Ensnared,
	// Token: 0x0400035C RID: 860
	Exhausted,
	// Token: 0x0400035D RID: 861
	Flashed,
	// Token: 0x0400035E RID: 862
	Hemorrhage,
	// Token: 0x0400035F RID: 863
	Invigorated,
	// Token: 0x04000360 RID: 864
	BodyshotReduction,
	// Token: 0x04000361 RID: 865
	Poisoned,
	// Token: 0x04000362 RID: 866
	Scp207,
	// Token: 0x04000363 RID: 867
	Invisible,
	// Token: 0x04000364 RID: 868
	SinkHole,
	// Token: 0x04000365 RID: 869
	DamageReduction,
	// Token: 0x04000366 RID: 870
	MovementBoost,
	// Token: 0x04000367 RID: 871
	RainbowTaste,
	// Token: 0x04000368 RID: 872
	SeveredHands,
	// Token: 0x04000369 RID: 873
	Stained,
	// Token: 0x0400036A RID: 874
	Vitality,
	// Token: 0x0400036B RID: 875
	Hypothermia,
	// Token: 0x0400036C RID: 876
	Scp1853,
	// Token: 0x0400036D RID: 877
	CardiacArrest,
	// Token: 0x0400036E RID: 878
	InsufficientLighting,
	// Token: 0x0400036F RID: 879
	SoundtrackMute,
	// Token: 0x04000370 RID: 880
	SpawnProtected,
	// Token: 0x04000371 RID: 881
	Traumatized,
	// Token: 0x04000372 RID: 882
	AntiScp207,
	// Token: 0x04000373 RID: 883
	Scanned,
	// Token: 0x04000374 RID: 884
	PocketCorroding,
	// Token: 0x04000375 RID: 885
	SilentWalk,
	// Token: 0x04000376 RID: 886
	[Obsolete("Not functional in-game")]
	Marshmallow,
	// Token: 0x04000377 RID: 887
	Strangled,
	// Token: 0x04000378 RID: 888
	Ghostly,
	// Token: 0x04000379 RID: 889
	FogControl,
	// Token: 0x0400037A RID: 890
	Slowness,
	// Token: 0x0400037B RID: 891
	Scp1344,
	// Token: 0x0400037C RID: 892
	SeveredEyes,
	// Token: 0x0400037D RID: 893
	PitDeath,
	// Token: 0x0400037E RID: 894
	Blurred
}

[Flags]
[Serializable]
public enum TeleportInvokeType
{
	Enter = 1,
	Exit = 2,
	Collide = 4
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

[Flags]
[Serializable]
public enum DeadType
{
	Disappear = 1,
	GetRigidbody = 2,
	DynamicDisappearing = 4,
	Explode = 8,
	ResetHP = 16,
	PlayAnimation = 32,
	Warhead = 64,
	SendMessage = 128,
    DropItems = 256,
	SendCommand = 512,
	GiveEffect = 1024,
	PlayAudio = 2048,
	CallGroovieNoise = 4096
}

[Serializable]
public class GMDTO : RandomExecutionModule
{
	public List<int> Targets;
	public bool Enable;
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
public class RandomExecutionModule
{
	public float ChanceWeight;
	public bool ForceExecute;
	public float ActionDelay;
}

[Flags]
[Serializable]
public enum WarheadActionType
{
	Start = 1,
	Stop = 2,
	Lock = 4,
	UnLock = 8,
	Disable = 16,
	Enable = 32
}

[Serializable]
public enum AnimationType
{
	Start,
	Stop,
	ModifyParameter
}

[Serializable]
public enum ParameterType
{
	Integer,
	Float,
	Bool,
	Trigger
}

[Serializable]
public enum MessageType
{
	Cassie,
	BroadCast,
	Hint
}

[Flags]
[Serializable]
public enum SendType
{
	Interactor = 1,
	AllExceptAboveOne = 2,
	Alive = 4,
	Spectators = 8
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
public class Commanding : RandomExecutionModule
{
	[Tooltip("{p_i} = player id.\n{p_name}\n{p_pos}\n{p_room}\n{p_zone}\n{p_role}\n{o_pos} = object's exact position.\n{o_room}\n{o_zone}\n{p_item} = attacker's current item." +
		"\n{damage} (HealthObject only)")]
	public string CommandContext;
}

[Serializable]
public class SVector3
{
	public float x;
	public float y;
	public float z;
}


//Unused serializable.
//[Serializable]
//public enum CommandType
//{
//	RemoteAdmin,
//	ClientConsole
//}

//[Serializable]
//public enum ExecutorType
//{
//	Attacker,
//	LocalAdmin
//}


[Serializable]
public class WhitelistWeapon
{
	public ItemType ItemType;
	public uint CustomItemId;
}

[Flags]
[Serializable]
public enum Scp914Mode
{
	Rough = 1,
	Coarse = 2,
	OneToOne = 4,
	Fine = 8,
	VeryFine = 16
}

[Flags]
[Serializable]
public enum IPActionType
{
	Disappear = 1,
	Explode = 2,
	PlayAnimation = 4,
	Warhead = 8,
	SendMessage = 16,
	DropItems = 32,
	SendCommand = 64,
	UpgradeItem = 128,
	GiveEffect = 256,
	PlayAudio = 512,
	CallGroovieNoise = 1024
}

[Flags]
[Serializable]
public enum InvokeType
{
	Searching = 1,
	Picked = 2
}

public class PublicFunctions
{
	public static string FindPath(Transform transform)
	{
		string path = "";
		if (transform.TryGetComponent<Schematic>(out _))
		{
			return path;
		}
		while (true)
		{
			for (int i = 0; i < transform.parent.childCount; i++)
			{
				if (transform.parent.GetChild(i) == transform)
				{
					path += i.ToString();
				}
			}
			transform = transform.parent;
			if (transform.TryGetComponent<Schematic>(out _)) break;
			path += " ";
		}
		return path;
	}

	public static void OnIDChange(int original, int New)
    {
		//Debug.Log("!!");
		AMERTs.RemoveWhere(x => x == null);
		//Debug.Log(AMERTs.Count);
		foreach (FakeMono noise in AMERTs)
        {
			if (noise is GroovyNoise && (noise as GroovyNoise).data.Settings != null)
            {
				foreach (GMDTO id in (noise as GroovyNoise).data.Settings)
				{
					for (int i = 0; i < id.Targets.Count; i++)
					{
						if (id.Targets[i] == original)
						{
							id.Targets[i] = New;
						}
					}
				}
			}
			else
            {
				//Debug.Log(noise.GetType());
                object arr;
				object obj;
				FieldInfo info = noise.GetType().GetField("data", BindingFlags.Instance | BindingFlags.Public);
                if (info != null && (obj = info.GetValue(noise)) != null)
                {
					FieldInfo info1 = obj.GetType().GetField("GroovieNoiseToCall", BindingFlags.Instance | BindingFlags.Public);
					if (info1 != null && (arr = info1.GetValue(obj)) != null)
					{
						foreach (CGNModule module in (List<CGNModule>)arr)
						{
							if (module.GroovieNoiseId == original)
								module.GroovieNoiseId = New;
						}
					}
                }
            }
        }
    }

	public static HashSet<FakeMono> AMERTs = new HashSet<FakeMono> { };
}
