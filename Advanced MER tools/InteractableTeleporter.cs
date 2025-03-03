//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.ComponentModel;
////using NaughtyAttributes;
//using Newtonsoft.Json;
//using System.IO;
//using UnityEditor;
//using System;
//using System.Linq;

//public class InteractableTeleporter : FakeMono
//{
//    public new ITDTO data = new ITDTO();
//    public override DTO _data { get => data; }
//    public override DTO _ScriptValueData { get => ScriptValueData; }

//    private void OnValidate()
//    {
//        bool flag = false;
//        if (data.ActionType.HasFlag(IPActionType.UpgradeItem))
//        {
//            flag = true;
//            data.ActionType -= IPActionType.UpgradeItem;
//        }
//        //if (ActionType.HasFlag(IPActionType.PlayAnimation))
//        //{
//        //    flag = true;
//        //    ActionType -= IPActionType.PlayAnimation;
//        //}
//        if (data.ActionType.HasFlag(IPActionType.DropItems))
//        {
//            flag = true;
//            data.ActionType -= IPActionType.DropItems;
//        }
//        if (flag)
//            Debug.Log("You cannot use that option in InteractableTeleporters!");
//    }
//}

//[Serializable]
//public class ITDTO : DTO
//{
//    public override void OnValidate()
//    {
//    }

//    [Tooltip("Index means the index of teleporter that this script will be applied to, in loaded map. It starts from 1.")]
//    public int index;
//    public TeleportInvokeType InvokeType;
//    public IPActionType ActionType;
//    public List<AnimationDTO> animationDTOs;
//    public WarheadActionType warheadActionType;
//    public List<MessageModule> MessageModules;
//    public List<Commanding> commandings;
//    public List<ExplodeModule> ExplodeModules;
//    public List<EffectGivingModule> effectGivingModules;
//    public List<AudioModule> AudioModules;
//    public List<CGNModule> GroovieNoiseToCall;
//    public List<CFEModule> FunctionToCall;
//}

//[Serializable]
//public class FITDTO : DTO
//{
//    public override void OnValidate()
//    {
//        index.OnValidate();
//        InvokeType.OnValidate();
//        warheadActionType.OnValidate();
//        MessageModules.ForEach(x => x.OnValidate());
//        commandings.ForEach(x => x.OnValidate());
//        ExplodeModules.ForEach(x => x.OnValidate());
//        effectGivingModules.ForEach(x => x.OnValidate());
//        AudioModules.ForEach(x => x.OnValidate());
//        GroovieNoiseToCall.ForEach(x => x.OnValidate());
//        FunctionToCall.ForEach(x => x.OnValidate());
//    }

//    [Tooltip("Index means the index of teleporter that this script will be applied to, in loaded map. It starts from 1.")]
//    public ScriptValue index;
//    public ScriptValue InvokeType;
//    public IPActionType ActionType;
//    public List<FAnimationDTO> animationDTOs;
//    public ScriptValue warheadActionType;
//    public List<FMessageModule> MessageModules;
//    public List<FCommanding> commandings;
//    public List<FExplodeModule> ExplodeModules;
//    public List<FEffectGivingModule> effectGivingModules;
//    public List<FAudioModule> AudioModules;
//    public List<FCGNModule> GroovieNoiseToCall;
//    public List<FCFEModule> FunctionToCall;
//}

//public class InteractableTeleporterCompiler
//{
//    private static readonly Config Config = SchematicManager.Config;

//    [MenuItem("SchematicManager/Compile Interactable teleporter", priority = -11)]
//    public static void OnCompile()
//    {
//        string parentDirectoryPath = Directory.Exists(Config.ExportPath)
//            ? Config.ExportPath
//            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
//                "MapEditorReborn_CompiledSchematics");
//        if (!Directory.Exists(parentDirectoryPath))
//        {
//            Debug.LogError("Could not find schematic output!");
//            return;
//        }

//        File.WriteAllText(Path.Combine(parentDirectoryPath, "{map name here}-ITeleporters.json"), JsonConvert.SerializeObject(GameObject.FindObjectsOfType<InteractableTeleporter>().Where(x => !x.UseScriptValue).Select(x => x.data), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
//        File.WriteAllText(Path.Combine(parentDirectoryPath, "{map name here}-FITeleporters.json"), JsonConvert.SerializeObject(GameObject.FindObjectsOfType<InteractableTeleporter>().Where(x => x.UseScriptValue).Select(x => x.ScriptValueData), Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
//        Debug.Log("Successfully Imported Interactable Teleporters.");
//    }
//}