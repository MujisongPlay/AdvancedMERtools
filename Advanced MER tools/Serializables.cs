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
	CallFunction = 512,
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

[Serializable]
public enum EffectType
{
	None,
	// Token: 0x0400036B RID: 875
	AmnesiaItems,
	// Token: 0x0400036C RID: 876
	AmnesiaVision,
	// Token: 0x0400036D RID: 877
	Asphyxiated,
	// Token: 0x0400036E RID: 878
	Bleeding,
	// Token: 0x0400036F RID: 879
	Blinded,
	// Token: 0x04000370 RID: 880
	Burned,
	// Token: 0x04000371 RID: 881
	Concussed,
	// Token: 0x04000372 RID: 882
	Corroding,
	// Token: 0x04000373 RID: 883
	Deafened,
	// Token: 0x04000374 RID: 884
	Decontaminating,
	// Token: 0x04000375 RID: 885
	Disabled,
	// Token: 0x04000376 RID: 886
	Ensnared,
	// Token: 0x04000377 RID: 887
	Exhausted,
	// Token: 0x04000378 RID: 888
	Flashed,
	// Token: 0x04000379 RID: 889
	Hemorrhage,
	// Token: 0x0400037A RID: 890
	Invigorated,
	// Token: 0x0400037B RID: 891
	BodyshotReduction,
	// Token: 0x0400037C RID: 892
	Poisoned,
	// Token: 0x0400037D RID: 893
	Scp207,
	// Token: 0x0400037E RID: 894
	Invisible,
	// Token: 0x0400037F RID: 895
	SinkHole,
	// Token: 0x04000380 RID: 896
	DamageReduction,
	// Token: 0x04000381 RID: 897
	MovementBoost,
	// Token: 0x04000382 RID: 898
	RainbowTaste,
	// Token: 0x04000383 RID: 899
	SeveredHands,
	// Token: 0x04000384 RID: 900
	Stained,
	// Token: 0x04000385 RID: 901
	Vitality,
	// Token: 0x04000386 RID: 902
	Hypothermia,
	// Token: 0x04000387 RID: 903
	Scp1853,
	// Token: 0x04000388 RID: 904
	CardiacArrest,
	// Token: 0x04000389 RID: 905
	InsufficientLighting,
	// Token: 0x0400038A RID: 906
	SoundtrackMute,
	// Token: 0x0400038B RID: 907
	SpawnProtected,
	// Token: 0x0400038C RID: 908
	Traumatized,
	// Token: 0x0400038D RID: 909
	AntiScp207,
	// Token: 0x0400038E RID: 910
	Scanned,
	// Token: 0x0400038F RID: 911
	PocketCorroding,
	// Token: 0x04000390 RID: 912
	SilentWalk,
	// Token: 0x04000391 RID: 913
	[Obsolete("Not functional in-game")]
	Marshmallow,
	// Token: 0x04000392 RID: 914
	Strangled,
	// Token: 0x04000393 RID: 915
	Ghostly,
	// Token: 0x04000394 RID: 916
	FogControl,
	// Token: 0x04000395 RID: 917
	Slowness,
	// Token: 0x04000396 RID: 918
	Scp1344,
	// Token: 0x04000397 RID: 919
	SeveredEyes,
	// Token: 0x04000398 RID: 920
	PitDeath,
	// Token: 0x04000399 RID: 921
	Blurred,
	// Token: 0x0400039A RID: 922
	[Obsolete("Only availaible for Christmas and AprilFools.")]
	BecomingFlamingo,
	// Token: 0x0400039B RID: 923
	[Obsolete("Only availaible for Christmas and AprilFools.")]
	Scp559,
	// Token: 0x0400039C RID: 924
	[Obsolete("Only availaible for Christmas and AprilFools.")]
	Scp956Target,
	// Token: 0x0400039D RID: 925
	[Obsolete("Only availaible for Christmas and AprilFools.")]
	Snowed
}

[Flags]
[Serializable]
public enum TeleportInvokeType
{
	Enter = 1,
	Exit = 2,
	Collide = 4
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
	CallGroovieNoise = 4096,
	CallFunction = 8192
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
	CallGroovieNoise = 1024,
	CallFunction = 2048,
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
	public static string FindPath(GameObject mono)
    {
		if (mono == null)
			return "";
		return FindPath(mono.transform);
    }

	public static string FindPath(Transform transform)
	{
		string path = "";
		if (transform.TryGetComponent<Schematic>(out _))
		{
			return path;
		}
		while (transform.parent != null)
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
