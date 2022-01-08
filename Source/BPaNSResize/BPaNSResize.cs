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
		public static Color BlueprintColor;

		public static ThingDef BiosculpterPodDef;
		public static GraphicData BiosculpterPodGraphicData_Standard;
		public static GraphicData BiosculpterPodGraphicData_Standard_Blueprint;
		public static GraphicData BiosculpterPodGraphicData_2x2_Left = new GraphicData();
		public static GraphicData BiosculpterPodGraphicData_2x2_Left_Blueprint;
		public static GraphicData BiosculpterPodGraphicData_2x2_Right = new GraphicData();
		public static GraphicData BiosculpterPodGraphicData_2x2_Right_Blueprint;
		public static GraphicData BiosculpterPodGraphicData_1x2 = new GraphicData();
		public static GraphicData BiosculpterPodGraphicData_1x2_Blueprint;
		public static GraphicData BiosculpterPodGraphicData_1x3 = new GraphicData();
		public static GraphicData BiosculpterPodGraphicData_1x3_Blueprint;

		public static Color OriginalSelectCycleColor;

		public static ThingDef NeuralSuperchargerDef;
		public static GraphicData NeuralSuperchargerGraphicData_Standard;
		public static GraphicData NeuralSuperchargerGraphicData_Standard_Blueprint;
		public static GraphicData NeuralSuperchargerGraphicData_1x2 = new GraphicData();
		public static GraphicData NeuralSuperchargerGraphicData_1x2_Blueprint;
		public static FleckDef NeuralSuperchargerChargedFloorDef;

		static StaticStuff()
		{
			BlueprintColor = (Color)typeof(ThingDefGenerator_Buildings).GetField("BlueprintColor", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

			// Get Biosculpter def
			BiosculpterPodDef = ThingDefOf.BiosculpterPod;
			BiosculpterPodGraphicData_Standard = BiosculpterPodDef.graphicData;
			BiosculpterPodGraphicData_Standard_Blueprint = BiosculpterPodDef.blueprintDef.graphicData;
			var shadowDataVolumeY = BiosculpterPodGraphicData_Standard.shadowData.BaseY;

			// Initialize 2x2 Biosculpter Pod graphic (left handed)
			MakeGraphicData(
				BiosculpterPodGraphicData_Standard,
				ref BiosculpterPodGraphicData_2x2_Left,
				ref BiosculpterPodGraphicData_2x2_Left_Blueprint,
				"BiosculpterPod/BiosculpterPod_2x2_Left",
				new Vector2(2, 2),
				shadowDataVolumeY);

			// Initialize 2x2 Biosculpter Pod graphic (right handed)
			MakeGraphicData(
				BiosculpterPodGraphicData_Standard,
				ref BiosculpterPodGraphicData_2x2_Right,
				ref BiosculpterPodGraphicData_2x2_Right_Blueprint,
				"BiosculpterPod/BiosculpterPod_2x2_Right",
				new Vector2(2, 2),
				shadowDataVolumeY);

			// Initialize 1x2 Biosculpter Pod graphic
			MakeGraphicData(
				BiosculpterPodGraphicData_Standard,
				ref BiosculpterPodGraphicData_1x2,
				ref BiosculpterPodGraphicData_1x2_Blueprint,
				"BiosculpterPod/BiosculpterPod_1x2",
				new Vector2(1, 2),
				shadowDataVolumeY);

			// Initialize 1x3 Biosculpter Pod graphic
			MakeGraphicData(
				BiosculpterPodGraphicData_Standard,
				ref BiosculpterPodGraphicData_1x3,
				ref BiosculpterPodGraphicData_1x3_Blueprint,
				"BiosculpterPod/BiosculpterPod_1x3",
				new Vector2(1, 3),
				shadowDataVolumeY);


			// Get Neural Supercharger def
			NeuralSuperchargerDef = ThingDefOf.NeuralSupercharger;
			NeuralSuperchargerGraphicData_Standard = NeuralSuperchargerDef.graphicData;
			NeuralSuperchargerGraphicData_Standard_Blueprint = NeuralSuperchargerDef.blueprintDef.graphicData;
			shadowDataVolumeY = NeuralSuperchargerGraphicData_Standard.shadowData.BaseY;

			// Initialize 1x2 Neural Supercharger graphic
			MakeGraphicData(
				NeuralSuperchargerGraphicData_Standard,
				ref NeuralSuperchargerGraphicData_1x2,
				ref NeuralSuperchargerGraphicData_1x2_Blueprint,
				"NeuralSupercharger/NeuralSupercharger_1x2",
				new Vector2(1, 2),
				shadowDataVolumeY);

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

		private static void MakeGraphicData(GraphicData standard, ref GraphicData graphicData, ref GraphicData graphicData_Blueprint, string texPath, Vector2 drawSize, float shadowDataVolumeY)
		{
			graphicData = new GraphicData();
			graphicData.CopyFrom(standard);
			graphicData.texPath = texPath;
			graphicData.drawSize = drawSize;
			graphicData.shadowData = new ShadowData
			{
				volume = new Vector3(drawSize.x - .1f, shadowDataVolumeY, drawSize.y - .1f)
			};
			graphicData.ExplicitlyInitCachedGraphic();

			graphicData_Blueprint = new GraphicData();
			graphicData_Blueprint.CopyFrom(graphicData);
			graphicData_Blueprint.shaderType = ShaderTypeDefOf.EdgeDetect;
			graphicData_Blueprint.color = BlueprintColor;
			graphicData_Blueprint.colorTwo = Color.white;
			graphicData_Blueprint.shadowData = null;
			graphicData_Blueprint.ExplicitlyInitCachedGraphic();
		}
	}

    public class BPaNSResize : HugsLib.ModBase
	{
		enum BiosculpterPodSize
		{
			Standard_3x2,
			Modded_2x2_Left,
			Modded_2x2_Right,
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
			GraphicData graphicData_Blueprint;
			IntVec2 buildingSize;
			Vector2 fleckSize = new Vector2(1, 2);
			IntVec3 interactionCellOffset = new IntVec3(0, 0, 2);
			switch (value)
			{
				default:
				case BiosculpterPodSize.Standard_3x2:
					graphicData = StaticStuff.BiosculpterPodGraphicData_Standard;
					graphicData_Blueprint = StaticStuff.BiosculpterPodGraphicData_Standard_Blueprint;
					buildingSize = new IntVec2(3, 2);
					fleckSize = new Vector2(2, 2);
					break;
				case BiosculpterPodSize.Modded_2x2_Left:
					graphicData = StaticStuff.BiosculpterPodGraphicData_2x2_Left;
					graphicData_Blueprint = StaticStuff.BiosculpterPodGraphicData_2x2_Left_Blueprint;
					buildingSize = new IntVec2(2, 2);
					interactionCellOffset = new IntVec3(1, 0, 2);
					break;
				case BiosculpterPodSize.Modded_2x2_Right:
					graphicData = StaticStuff.BiosculpterPodGraphicData_2x2_Right;
					graphicData_Blueprint = StaticStuff.BiosculpterPodGraphicData_2x2_Right_Blueprint;
					buildingSize = new IntVec2(2, 2);
					break;
				case BiosculpterPodSize.Modded_1x2:
					graphicData = StaticStuff.BiosculpterPodGraphicData_1x2;
					graphicData_Blueprint = StaticStuff.BiosculpterPodGraphicData_1x2_Blueprint;
					buildingSize = new IntVec2(1, 2);
					break;
				case BiosculpterPodSize.Modded_1x3:
					graphicData = StaticStuff.BiosculpterPodGraphicData_1x3;
					graphicData_Blueprint = StaticStuff.BiosculpterPodGraphicData_1x3_Blueprint;
					buildingSize = new IntVec2(1, 3);
					break;
			}

			ChangeDef(StaticStuff.BiosculpterPodDef, graphicData, graphicData_Blueprint, buildingSize, interactionCellOffset);
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
			GraphicData graphicData_Blueprint;
			IntVec2 buildingSize;
			switch (value)
			{
				default:
				case NeuralSuperchargerSize.Standard_1x3:
					graphicData = StaticStuff.NeuralSuperchargerGraphicData_Standard;
					graphicData_Blueprint = StaticStuff.NeuralSuperchargerGraphicData_Standard_Blueprint;
					buildingSize = new IntVec2(1, 3);
					break;
				case NeuralSuperchargerSize.Modded_1x2:
					graphicData = StaticStuff.NeuralSuperchargerGraphicData_1x2;
					graphicData_Blueprint = StaticStuff.NeuralSuperchargerGraphicData_1x2_Blueprint;
					buildingSize = new IntVec2(1, 2);
					break;
			}

			ChangeDef(StaticStuff.NeuralSuperchargerDef, graphicData, graphicData_Blueprint, buildingSize);
			StaticStuff.NeuralSuperchargerChargedFloorDef.graphicData.drawSize = buildingSize.ToVector2();
		}

		private void ChangeDef(ThingDef thingDef, GraphicData graphicData, GraphicData graphicData_Blueprint, IntVec2 buildingSize, IntVec3? interactionCellOffset = null)
		{
			thingDef.size = buildingSize;
			thingDef.graphicData = graphicData;
			thingDef.graphic = graphicData.Graphic;
			thingDef.uiIcon = (Texture2D)graphicData.Graphic.MatSouth.mainTexture;

			if (interactionCellOffset != null)
				thingDef.interactionCellOffset = (IntVec3)interactionCellOffset;

			thingDef.blueprintDef.size = buildingSize;
			thingDef.blueprintDef.graphicData = graphicData_Blueprint;
			thingDef.blueprintDef.graphic = graphicData_Blueprint.Graphic;

			thingDef.installBlueprintDef.size = buildingSize;
			thingDef.installBlueprintDef.graphicData = graphicData_Blueprint;
			thingDef.installBlueprintDef.graphic = graphicData_Blueprint.Graphic;

			thingDef.frameDef.size = buildingSize;
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
			var rotation = parent.Rotation;
			var interactionCell = parent.InteractionCell.ToVector3();
			if (rotation == Rot4.South)
				return interactionCell + new Vector3(0.5f, 0, 2.0f);
			if (rotation == Rot4.West)
				return interactionCell + new Vector3(2.0f, 0, 0.35f);
			if (rotation == Rot4.North)
				return interactionCell + new Vector3(0.5f, 0, -0.9f);
			if (rotation == Rot4.East)
				return interactionCell + new Vector3(-1.0f, 0, 0.35f);
			return parent.DrawPos;
		}

		public static TargetInfo ModifyBiosculpterTargetInfo(ThingWithComps parent)
		{
			var rot = parent.Rotation;
			if (rot == Rot4.South)
				return new TargetInfo(parent.InteractionCell + new IntVec3(0, 0, 2), parent.Map);
			if (rot == Rot4.West)
				return new TargetInfo(parent.InteractionCell + new IntVec3(2, 0, 0), parent.Map);
			if (rot == Rot4.North)
				return new TargetInfo(parent.InteractionCell + new IntVec3(0, 0, -2), parent.Map);
			if (rot == Rot4.East)
				return new TargetInfo(parent.InteractionCell + new IntVec3(-2, 0, 0), parent.Map);
			return parent;
		}
	}
}
