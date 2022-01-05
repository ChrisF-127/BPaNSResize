using HugsLib.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;

namespace BPaNSResize
{
	[StaticConstructorOnStartup]
	internal static class StaticStuff
	{
		public static ThingDef BiosculpterPodDef;
		public static GraphicData BiosculpterPodGraphicData_Standard;
		public static GraphicData BiosculpterPodGraphicData_2x2 = new GraphicData();
		public static GraphicData BiosculpterPodGraphicData_1x2 = new GraphicData();
		public static GraphicData BiosculpterPodGraphicData_1x3 = new GraphicData();

		public static Color OriginalSelectCycleColor;

		public static ThingDef NeuralSuperchargerDef;
		public static GraphicData NeuralSuperchargerGraphicData_Standard;
		public static GraphicData NeuralSuperchargerGraphicData_1x2 = new GraphicData();
		public static FleckDef NeuralSuperchargerChargedFloorDef;

		static StaticStuff()
		{
			// Get Biosculpter def
			BiosculpterPodDef = ThingDefOf.BiosculpterPod;
			// Get standard Biosculpter graphic
			BiosculpterPodGraphicData_Standard = BiosculpterPodDef.graphicData;

			// Standard shadow height / length
			var shadowDataVolumeY = BiosculpterPodGraphicData_Standard.shadowData.BaseY;

			// Initialize 2x2 Biosculpter Pod graphic
			BiosculpterPodGraphicData_2x2.CopyFrom(BiosculpterPodGraphicData_Standard);
			BiosculpterPodGraphicData_2x2.texPath = "BiosculpterPod/BiosculpterPod_2x2";
			BiosculpterPodGraphicData_2x2.drawSize = new Vector2(2, 2);
			BiosculpterPodGraphicData_2x2.shadowData = new ShadowData
			{
				volume = new Vector3(1.9f, shadowDataVolumeY, 1.9f)
			};
			BiosculpterPodGraphicData_2x2.ExplicitlyInitCachedGraphic();

			// Initialize 1x2 Biosculpter Pod graphic
			BiosculpterPodGraphicData_1x2.CopyFrom(BiosculpterPodGraphicData_Standard);
			BiosculpterPodGraphicData_1x2.texPath = "BiosculpterPod/BiosculpterPod_2x1";
			BiosculpterPodGraphicData_1x2.drawSize = new Vector2(1, 2);
			BiosculpterPodGraphicData_1x2.shadowData = new ShadowData
			{
				volume = new Vector3(0.9f, shadowDataVolumeY, 1.9f)
			};
			BiosculpterPodGraphicData_1x2.ExplicitlyInitCachedGraphic();

			// Initialize 1x3 Biosculpter Pod graphic
			BiosculpterPodGraphicData_1x3.CopyFrom(BiosculpterPodGraphicData_Standard);
			BiosculpterPodGraphicData_1x3.texPath = "BiosculpterPod/BiosculpterPod_3x1";
			BiosculpterPodGraphicData_1x3.drawSize = new Vector2(1, 3);
			BiosculpterPodGraphicData_1x3.shadowData = new ShadowData
			{
				volume = new Vector3(0.9f, shadowDataVolumeY, 2.9f)
			};
			BiosculpterPodGraphicData_1x3.ExplicitlyInitCachedGraphic();


			// Get Neural Supercharger def
			NeuralSuperchargerDef = ThingDefOf.NeuralSupercharger;
			NeuralSuperchargerGraphicData_Standard = NeuralSuperchargerDef.graphicData;

			// Initialize 1x2 Neural Supercharger graphic
			NeuralSuperchargerGraphicData_1x2.CopyFrom(NeuralSuperchargerGraphicData_Standard);
			NeuralSuperchargerGraphicData_1x2.texPath = "NeuralSupercharger/NeuralSupercharger_2x1";
			NeuralSuperchargerGraphicData_1x2.drawSize = new Vector2(1, 2);
			NeuralSuperchargerGraphicData_1x2.shadowData = new ShadowData
			{
				volume = new Vector3(0.9f, shadowDataVolumeY, 1.9f)
			};
			NeuralSuperchargerGraphicData_1x2.ExplicitlyInitCachedGraphic();

			// Get Neural Supercharger charged floor effect def
			NeuralSuperchargerChargedFloorDef = DefDatabase<FleckDef>.AllDefs.First((def) => def.defName == "NeuralSuperchargerChargedFloor");


			// Fix effecter position; necessary since we make the effect appear between the interaction spot and 1.5 cells away from it depending on rotation, 
			//	but it needs to be 1 cells away, which TargetInfo does not allow for without giving it a Thing with a fitting center which we do not have on 2x2
			EffecterDef biosculpterPod_Operating = null;
			EffecterDef biosculpterPod_Ready = null;
			foreach (var def in DefDatabase<EffecterDef>.AllDefs)
			{
				if (biosculpterPod_Operating == null
					   && def.defName == "BiosculpterPod_Operating")
					biosculpterPod_Operating = def;
				else if (biosculpterPod_Ready == null
					   && def.defName == "BiosculpterPod_Ready")
					biosculpterPod_Ready = def;

				if (biosculpterPod_Operating != null
					&& biosculpterPod_Ready != null)
					break;
			}
			biosculpterPod_Operating.offsetTowardsTarget = new FloatRange(0.5f, 0.5f);
			biosculpterPod_Ready.offsetTowardsTarget = new FloatRange(0.5f, 0.5f);

			// Resize FleckDefs for the Effecters to look more fitting for the smaller buildings
			FleckDef biosculpterScanner_Forward = null;
			FleckDef biosculpterScanner_Backward = null;
			FleckDef biosculpterScanner_Ready = null;
			foreach (var def in DefDatabase<FleckDef>.AllDefs)
			{
				if (biosculpterScanner_Forward == null
					   && def.defName == "BiosculpterScanner_Forward")
					biosculpterScanner_Forward = def;
				else if (biosculpterScanner_Backward == null
					   && def.defName == "BiosculpterScanner_Backward")
					biosculpterScanner_Backward = def;
				else if (biosculpterScanner_Ready == null
					   && def.defName == "BiosculpterScanner_Ready")
					biosculpterScanner_Ready = def;

				if (biosculpterScanner_Forward != null
					&& biosculpterScanner_Backward != null
					&& biosculpterScanner_Ready != null)
					break;
			}
			biosculpterScanner_Forward.graphicData.drawSize = new Vector2(1.5f, 0.5f); // standard is 3x1
			biosculpterScanner_Backward.graphicData.drawSize = new Vector2(1f, 0.5f); // standard is 2x1
			biosculpterScanner_Ready.graphicData.drawSize = new Vector2(1f, 2f); // standard is 2x2

			// Save original color for the ready effecter
			OriginalSelectCycleColor = BiosculpterPodDef.GetCompProperties<CompProperties_BiosculpterPod>().selectCycleColor;
		}
	}

    public class BPaNSResize : HugsLib.ModBase
	{
		enum BiosculpterPodSize
		{
			Standard_3x2,
			Modded_2x2,
			Modded_1x2,
			Modded_1x3,
		}
		private SettingHandle<BiosculpterPodSize> _biosculpterPodSize;

		enum ColorSelector
		{
			R, G, B
		}
		private static SettingHandle<float> _biosculpterPodReadyEffecterColorR;
		private static SettingHandle<float> _biosculpterPodReadyEffecterColorG;
		private static SettingHandle<float> _biosculpterPodReadyEffecterColorB;

		enum NeuralSuperchargerSize
		{
			Standard_1x3,
			Modded_1x2,
		}
		private static SettingHandle<NeuralSuperchargerSize> _neuralSuperchargerSize;

		public override string ModIdentifier => "BiosculpterPodAndNeuralSuperchargerResize";

		public override void DefsLoaded()
		{
			_biosculpterPodSize = Settings.GetHandle(
				"biosculpterPodSize", 
				"SY_BNR.BiosculpterPodTitle".Translate(), 
				"SY_BNR.BiosculpterPodDesc".Translate(), 
				BiosculpterPodSize.Standard_3x2,
				enumPrefix: "SY_BNR.BiosculpterPodSize_");
			_biosculpterPodSize.ValueChanged += (value) => ChangeBiosculpterPodSize((SettingHandle<BiosculpterPodSize>)value);
			ChangeBiosculpterPodSize(_biosculpterPodSize);

			_biosculpterPodReadyEffecterColorR = Settings.GetHandle(
				"biosculpterPodReadyEffecterColorR",
				"SY_BNR.BiosculpterPodReadyEffecterColorRTitle".Translate(),
				"SY_BNR.BiosculpterPodReadyEffecterColorRDesc".Translate(),
				StaticStuff.OriginalSelectCycleColor.r);
			_biosculpterPodReadyEffecterColorR.ValueChanged += (value) => ChangeBiosculpterPodReadyColor(ColorSelector.R, (SettingHandle<float>)value);
			ChangeBiosculpterPodReadyColor(ColorSelector.R, _biosculpterPodReadyEffecterColorR);
			_biosculpterPodReadyEffecterColorG = Settings.GetHandle(
				"biosculpterPodReadyEffecterColorG",
				"SY_BNR.BiosculpterPodReadyEffecterColorGTitle".Translate(),
				"SY_BNR.BiosculpterPodReadyEffecterColorGDesc".Translate(),
				StaticStuff.OriginalSelectCycleColor.g);
			_biosculpterPodReadyEffecterColorG.ValueChanged += (value) => ChangeBiosculpterPodReadyColor(ColorSelector.G, (SettingHandle<float>)value);
			ChangeBiosculpterPodReadyColor(ColorSelector.G, _biosculpterPodReadyEffecterColorG);
			_biosculpterPodReadyEffecterColorB = Settings.GetHandle(
				"biosculpterPodReadyEffecterColorB",
				"SY_BNR.BiosculpterPodReadyEffecterColorBTitle".Translate(),
				"SY_BNR.BiosculpterPodReadyEffecterColorBDesc".Translate(),
				StaticStuff.OriginalSelectCycleColor.b);
			_biosculpterPodReadyEffecterColorB.ValueChanged += (value) => ChangeBiosculpterPodReadyColor(ColorSelector.B, (SettingHandle<float>)value);
			ChangeBiosculpterPodReadyColor(ColorSelector.B, _biosculpterPodReadyEffecterColorB);

			_neuralSuperchargerSize = Settings.GetHandle(
				"neuralSuperchargerSize", 
				"SY_BNR.NeuralSuperchargerTitle".Translate(), 
				"SY_BNR.NeuralSuperchargerDesc".Translate(), 
				NeuralSuperchargerSize.Standard_1x3,
				enumPrefix: "SY_BNR.NeuralSuperchargerSize_");
			_neuralSuperchargerSize.ValueChanged += (value) => ChangeNeuralSuperchargerSize((SettingHandle<NeuralSuperchargerSize>)value);
			ChangeNeuralSuperchargerSize(_neuralSuperchargerSize);
		}

		private void ChangeBiosculpterPodSize(BiosculpterPodSize value)
		{
			GraphicData graphicData;
			IntVec2 buildingSize;
			Vector2 fleckSize;
			IntVec3 interactionCellOffset = new IntVec3(0, 0, 2);
			switch (value)
			{
				default:
				case BiosculpterPodSize.Standard_3x2:
					graphicData = StaticStuff.BiosculpterPodGraphicData_Standard;
					buildingSize = new IntVec2(3, 2);
					fleckSize = new Vector2(2, 2);
					break;
				case BiosculpterPodSize.Modded_2x2:
					graphicData = StaticStuff.BiosculpterPodGraphicData_2x2;
					buildingSize = new IntVec2(2, 2);
					fleckSize = new Vector2(1, 2);
					interactionCellOffset = new IntVec3(1, 0, 2);
					break;
				case BiosculpterPodSize.Modded_1x2:
					graphicData = StaticStuff.BiosculpterPodGraphicData_1x2;
					buildingSize = new IntVec2(1, 2);
					fleckSize = new Vector2(1, 2);
					break;
				case BiosculpterPodSize.Modded_1x3:
					graphicData = StaticStuff.BiosculpterPodGraphicData_1x3;
					buildingSize = new IntVec2(1, 3);
					fleckSize = new Vector2(1, 2);
					break;
			}
			
			StaticStuff.BiosculpterPodDef.graphicData = graphicData;
			StaticStuff.BiosculpterPodDef.graphic = graphicData.Graphic;
			StaticStuff.BiosculpterPodDef.size = buildingSize;
			StaticStuff.BiosculpterPodDef.interactionCellOffset = interactionCellOffset;
		}

		private void ChangeBiosculpterPodReadyColor(ColorSelector rgb, float value)
		{
			switch (rgb)
			{
				case ColorSelector.R:
					StaticStuff.BiosculpterPodDef.GetCompProperties<CompProperties_BiosculpterPod>().selectCycleColor.r = value;
					break;
				case ColorSelector.G:
					StaticStuff.BiosculpterPodDef.GetCompProperties<CompProperties_BiosculpterPod>().selectCycleColor.g = value;
					break;
				case ColorSelector.B:
					StaticStuff.BiosculpterPodDef.GetCompProperties<CompProperties_BiosculpterPod>().selectCycleColor.b = value;
					break;
			}
		}

		private void ChangeNeuralSuperchargerSize(NeuralSuperchargerSize value)
		{
			GraphicData graphicData;
			IntVec2 size;
			switch (value)
			{
				default:
				case NeuralSuperchargerSize.Standard_1x3:
					graphicData = StaticStuff.NeuralSuperchargerGraphicData_Standard;
					size = new IntVec2(1, 3);
					break;
				case NeuralSuperchargerSize.Modded_1x2:
					graphicData = StaticStuff.NeuralSuperchargerGraphicData_1x2;
					size = new IntVec2(1, 2);
					break;
			}

			StaticStuff.NeuralSuperchargerDef.graphicData = graphicData;
			StaticStuff.NeuralSuperchargerDef.graphic = graphicData.Graphic;
			StaticStuff.NeuralSuperchargerDef.size = size;

			StaticStuff.NeuralSuperchargerChargedFloorDef.graphicData.drawSize = size.ToVector2();
		}
	}

	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		static HarmonyPatches()
		{
			Harmony harmony = new Harmony("syrus.bpansresize");
			
			harmony.Patch(
				typeof(CompBiosculpterPod).GetMethod("PostDraw"),
				transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.CompBiosculpterPod_PostDraw_Transpiler)));
			harmony.Patch(
				typeof(CompBiosculpterPod).GetMethod("CompTick"),
				transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.CompBiosculpterPod_CompTick_Transpiler)));
		}

		static IEnumerable<CodeInstruction> CompBiosculpterPod_PostDraw_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var list = new List<CodeInstruction>(instructions);
			for (int i = 0; i < list.Count; i++)
			{
				var instruction = list[i];
				//Log.Message(instruction.ToString());
				if (i < list.Count - 2
					&& instruction.opcode == OpCodes.Callvirt
					&& list[i + 2].opcode == OpCodes.Call
					&& instruction.operand is MethodBase method0
					&& list[i + 2].operand is MethodBase method1
					&& method0.Name == "get_DrawPos"
					&& method1.Name == "FloatingOffset")
				{
					instruction.opcode = OpCodes.Call;
					instruction.operand = typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.ModifyPawnDrawOffset));
					//Log.Warning("REPLACED: " + instruction.ToString());
				}
				yield return instruction;
			}
		}

		static IEnumerable<CodeInstruction> CompBiosculpterPod_CompTick_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var list = new List<CodeInstruction>(instructions);
			for (int i = 0; i < list.Count; i++)
			{
				var instruction = list[i];
				//Log.Message(instruction.ToString());
				if (i > 2
					&& instruction.opcode == OpCodes.Call
					&& list[i - 3].opcode == OpCodes.Ldfld
					&& instruction.operand is MethodBase method
					&& list[i - 3].operand is FieldInfo fieldInfo
					&& method.DeclaringType == typeof(TargetInfo)
					&& method.Name == "op_Implicit"
					&& (fieldInfo.Name == "readyEffecter" || fieldInfo.Name == "operatingEffecter"))
				{
					instruction.opcode = OpCodes.Call;
					instruction.operand = typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.ModifyBiosculpterTargetInfo));
					//Log.Warning("REPLACED: " + instruction.ToString());
				}
				yield return instruction;
			}
		}


		public static Vector3 ModifyPawnDrawOffset(ThingWithComps parent)
		{
			var size = parent.def.size;
			if (size.x == 2 && size.z == 2) // 2x2
			{
				var rotation = parent.Rotation;
				if (rotation == Rot4.North)
					return parent.DrawPos + new Vector3(0.5f, 0, 0.1f);
				if (rotation == Rot4.South)
					return parent.DrawPos + new Vector3(-0.5f, 0, 0);
				if (rotation == Rot4.East)
					return parent.DrawPos + new Vector3(0, 0, -0.5f);
				if (rotation == Rot4.West)
					return parent.DrawPos + new Vector3(0, 0, 0.3f);
			}
			else if (size.x == 1 && size.z == 2) // 1x2
			{
				var rotation = parent.Rotation;
				if (rotation == Rot4.North)
					return parent.DrawPos;// + new Vector3(0, 0, 0);
				if (rotation == Rot4.South)
					return parent.DrawPos + new Vector3(0, 0, -0.1f);
				if (rotation == Rot4.East)
					return parent.DrawPos + new Vector3(0, 0, -0.1f);
				if (rotation == Rot4.West)
					return parent.DrawPos + new Vector3(0, 0, -0.1f);
			}
			else if (size.x == 1 && size.z == 3) // 1x3
			{
				var rotation = parent.Rotation;
				if (rotation == Rot4.North)
					return parent.DrawPos + new Vector3(0, 0, 0.5f);
				if (rotation == Rot4.South)
					return parent.DrawPos + new Vector3(0, 0, -0.5f);
				if (rotation == Rot4.East)
					return parent.DrawPos + new Vector3(0.5f, 0, -0.1f);
				if (rotation == Rot4.West)
					return parent.DrawPos + new Vector3(-0.5f, 0, -0.1f);
			}
			return parent.DrawPos;
		}

		public static TargetInfo ModifyBiosculpterTargetInfo(ThingWithComps parent)
		{
			var rot = parent.Rotation;
			if (rot == Rot4.North)
				return new TargetInfo(parent.InteractionCell + new IntVec3(0, 0, -2), parent.Map);
			else if (rot == Rot4.East)
				return new TargetInfo(parent.InteractionCell + new IntVec3(-2, 0, 0), parent.Map);
			else if (rot == Rot4.South)
				return new TargetInfo(parent.InteractionCell + new IntVec3(0, 0, 2), parent.Map);
			else if (rot == Rot4.West)
				return new TargetInfo(parent.InteractionCell + new IntVec3(2, 0, 0), parent.Map);
			return parent;
		}
	}
}
