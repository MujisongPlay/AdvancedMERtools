using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CustomCollider : MonoBehaviour
{
    public CollisionType CollisionType;
    public ColliderActionType ActionFlag;
    public DetectType DetectType;
    //public bool InvisibleCollider;
    //[Description("Bounds' extra size. That helps it detect even if primitives are collidable.")]
    //public float ContactOffSet;
    //[ShowIf("ActionFlag", ColliderActionType.ModifyHealth)]
	[Header("Modify Health")]
    public float Amount;
	//[ShowIf("ActionFlag", ColliderActionType.GiveEffect)]
	//[ReorderableList]
	public List<EffectGivingModule> EffectGivingModules;
	//[ShowIf("ActionFlag", ColliderActionType.PlayAnimation)]
	//[ReorderableList]
	public List<AnimationModule> AnimationModules;
	//[ShowIf("ActionFlag", ColliderActionType.SendCommand)]
	//[ReorderableList]
	public List<Commanding> Commandings;
	//[ShowIf("ActionFlag", ColliderActionType.SendMessage)]
	//[ReorderableList]
	public List<MessageModule> MessageModules;
	//[ShowIf("ActionFlag", ColliderActionType.Explode)]
	//[ReorderableList]
	public List<ExplodeModule> ExplodeModules;
	//[BoxGroup("Warhead")]
	//[ShowIf("ActionFlag", ColliderActionType.Warhead)]
	public WarheadActionType warheadAction = WarheadActionType.Start;
}

[Serializable]
public class CCDTO
{
	public ColliderActionType ColliderActionType;
	public CollisionType CollisionType;
    public DetectType DetectType;
	public string ObjectId;
	//public bool Invisible;
    //public float ContactOffSet;
    public float Amount;
	public List<AnimationDTO> animationDTOs;
	public WarheadActionType warheadActionType;
	public List<MessageModule> MessageModules;
	public List<DropItem> dropItems;
	public List<Commanding> commandings;
	public List<ExplodeModule> ExplodeModules;
	public List<EffectGivingModule> effectGivingModules;
}

public class CustomColliderCompiler
{
    private static readonly Config Config = SchematicManager.Config;

    [MenuItem("SchematicManager/Compile custom collider", priority = -11)]
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

        List<CCDTO> interactables = new List<CCDTO> { };

        foreach (CustomCollider ip in schematic.transform.GetComponentsInChildren<CustomCollider>())
        {
            CCDTO DTO = new CCDTO
            {
                ObjectId = PublicFunctions.FindPath(ip.transform),
                ColliderActionType = ip.ActionFlag,
                CollisionType = ip.CollisionType,
                //Invisible = ip.InvisibleCollider,
                DetectType = ip.DetectType,
                //ContactOffSet = ip.ContactOffSet
            };
            foreach (ColliderActionType type in Enum.GetValues(typeof(ColliderActionType)))
            {
                if (ip.ActionFlag.HasFlag(type))
                {
                    switch (type)
                    {
                        case ColliderActionType.ModifyHealth:
                            DTO.Amount = ip.Amount;
                            break;
                        case ColliderActionType.Explode:
                            DTO.ExplodeModules = ip.ExplodeModules;
                            break;
                        case ColliderActionType.PlayAnimation:
                            DTO.animationDTOs = new List<AnimationDTO> { };
                            foreach (AnimationModule module in ip.AnimationModules)
                            {
                                DTO.animationDTOs.Add(new AnimationDTO
                                {
                                    Animator = PublicFunctions.FindPath(module.Animator.transform),
                                    Animation = module.AnimationName,
                                    AnimationType = module.AnimationType,
                                    ForceExecute = module.ForceExecute,
                                    ChanceWeight = module.ChanceWeight
                                });
                            }
                            break;
                        case ColliderActionType.Warhead:
                            DTO.warheadActionType = ip.warheadAction;
                            break;
                        case ColliderActionType.SendMessage:
                            DTO.MessageModules = ip.MessageModules;
                            break;
                        case ColliderActionType.SendCommand:
                            DTO.commandings = ip.Commandings;
                            break;
                        case ColliderActionType.GiveEffect:
                            DTO.effectGivingModules = ip.EffectGivingModules;
                            break;
                        default:
                            break;
                    }
                }
            }

            interactables.Add(DTO);
        }

        string serializedData = JsonConvert.SerializeObject(interactables, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{schematic.gameObject.name}-Colliders.json"), serializedData);
        Debug.Log("Successfully Imported Custom Colliders.");
    }
}
