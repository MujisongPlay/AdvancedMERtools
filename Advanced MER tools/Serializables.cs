using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;

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
	Explode = 64
}

[Flags]
[Serializable]
public enum CollisionType
{
	OnEnter = 1,
	OnStay = 2,
	OnExit = 4
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
	// Token: 0x040002F2 RID: 754
	None = -1,
	// Token: 0x040002F3 RID: 755
	AmnesiaItems,
	// Token: 0x040002F4 RID: 756
	AmnesiaVision,
	// Token: 0x040002F5 RID: 757
	Asphyxiated,
	// Token: 0x040002F6 RID: 758
	Bleeding,
	// Token: 0x040002F7 RID: 759
	Blinded,
	// Token: 0x040002F8 RID: 760
	Burned,
	// Token: 0x040002F9 RID: 761
	Concussed,
	// Token: 0x040002FA RID: 762
	Corroding,
	// Token: 0x040002FB RID: 763
	Deafened,
	// Token: 0x040002FC RID: 764
	Decontaminating,
	// Token: 0x040002FD RID: 765
	Disabled,
	// Token: 0x040002FE RID: 766
	Ensnared,
	// Token: 0x040002FF RID: 767
	Exhausted,
	// Token: 0x04000300 RID: 768
	Flashed,
	// Token: 0x04000301 RID: 769
	Hemorrhage,
	// Token: 0x04000302 RID: 770
	Invigorated,
	// Token: 0x04000303 RID: 771
	BodyshotReduction,
	// Token: 0x04000304 RID: 772
	Poisoned,
	// Token: 0x04000305 RID: 773
	Scp207,
	// Token: 0x04000306 RID: 774
	Invisible,
	// Token: 0x04000307 RID: 775
	SinkHole,
	// Token: 0x04000308 RID: 776
	DamageReduction,
	// Token: 0x04000309 RID: 777
	MovementBoost,
	// Token: 0x0400030A RID: 778
	RainbowTaste,
	// Token: 0x0400030B RID: 779
	SeveredHands,
	// Token: 0x0400030C RID: 780
	Stained,
	// Token: 0x0400030D RID: 781
	Vitality,
	// Token: 0x0400030E RID: 782
	Hypothermia,
	// Token: 0x0400030F RID: 783
	Scp1853,
	// Token: 0x04000310 RID: 784
	CardiacArrest,
	// Token: 0x04000311 RID: 785
	InsufficientLighting,
	// Token: 0x04000312 RID: 786
	SoundtrackMute,
	// Token: 0x04000313 RID: 787
	SpawnProtected,
	// Token: 0x04000314 RID: 788
	Traumatized,
	// Token: 0x04000315 RID: 789
	AntiScp207,
	// Token: 0x04000316 RID: 790
	Scanned,
	// Token: 0x04000317 RID: 791
	PocketCorroding,
	// Token: 0x04000318 RID: 792
	SilentWalk,
	// Token: 0x04000319 RID: 793
	[Obsolete("Not functional in-game")]
	Marshmallow,
	// Token: 0x0400031A RID: 794
	Strangled,
	// Token: 0x0400031B RID: 795
	Ghostly
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
	public string Animator;
	public string Animation;
	public AnimationType AnimationType;
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
	GiveEffect = 1024
}

[Serializable]
public class AnimationModule : RandomExecutionModule
{
	public GameObject Animator;
	public string AnimationName;
	public AnimationType AnimationType;
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
public class RandomExecutionModule
{
	public float ChanceWeight;
	public bool ForceExecute;
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
	Stop
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
	Rough = 0,
	Coarse = 1,
	OneToOne = 2,
	Fine = 3,
	VeryFine = 4
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
	GiveEffect = 256
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
}