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
		public static GraphicData BiosculpterPodGraphicData_2x1 = new GraphicData();
		public static GraphicData BiosculpterPodGraphicData_3x1 = new GraphicData();

		public static Color OriginalSelectCycleColor;

		public static ThingDef NeuralSuperchargerDef;
		public static GraphicData NeuralSuperchargerGraphicData_Standard;
		public static GraphicData NeuralSuperchargerGraphicData_2x1 = new GraphicData();

		static StaticStuff()
		{
			BiosculpterPodDef = ThingDefOf.BiosculpterPod;
			BiosculpterPodGraphicData_Standard = BiosculpterPodDef.graphicData;

			var shadowDataVolumeY = BiosculpterPodGraphicData_Standard.shadowData.BaseY;
			BiosculpterPodGraphicData_2x2.CopyFrom(BiosculpterPodGraphicData_Standard);
			BiosculpterPodGraphicData_2x2.texPath = "BiosculpterPod/BiosculpterPod_2x2";
			BiosculpterPodGraphicData_2x2.drawSize = new Vector2(2, 2);
			BiosculpterPodGraphicData_2x2.shadowData = new ShadowData
			{
				volume = new Vector3(1.9f, shadowDataVolumeY, 1.9f)
			};
			BiosculpterPodGraphicData_2x2.ExplicitlyInitCachedGraphic();

			BiosculpterPodGraphicData_2x1.CopyFrom(BiosculpterPodGraphicData_Standard);
			BiosculpterPodGraphicData_2x1.texPath = "BiosculpterPod/BiosculpterPod_2x1";
			BiosculpterPodGraphicData_2x1.drawSize = new Vector2(1, 2);
			BiosculpterPodGraphicData_2x1.shadowData = new ShadowData
			{
				volume = new Vector3(0.9f, shadowDataVolumeY, 1.9f)
			};
			BiosculpterPodGraphicData_2x1.ExplicitlyInitCachedGraphic();

			BiosculpterPodGraphicData_3x1.CopyFrom(BiosculpterPodGraphicData_Standard);
			BiosculpterPodGraphicData_3x1.texPath = "BiosculpterPod/BiosculpterPod_3x1";
			BiosculpterPodGraphicData_3x1.drawSize = new Vector2(1, 3);
			BiosculpterPodGraphicData_3x1.shadowData = new ShadowData
			{
				volume = new Vector3(0.9f, shadowDataVolumeY, 2.9f)
			};
			BiosculpterPodGraphicData_3x1.ExplicitlyInitCachedGraphic();


			NeuralSuperchargerDef = ThingDefOf.NeuralSupercharger;
			NeuralSuperchargerGraphicData_Standard = NeuralSuperchargerDef.graphicData;

			NeuralSuperchargerGraphicData_2x1.CopyFrom(NeuralSuperchargerGraphicData_Standard);
			NeuralSuperchargerGraphicData_2x1.texPath = "NeuralSupercharger/NeuralSupercharger_2x1";
			NeuralSuperchargerGraphicData_2x1.drawSize = new Vector2(1, 2);
			NeuralSuperchargerGraphicData_2x1.shadowData = new ShadowData
			{
				volume = new Vector3(1.9f, shadowDataVolumeY, 0.9f)
			};
			NeuralSuperchargerGraphicData_2x1.ExplicitlyInitCachedGraphic();


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

		enum Colors
		{
			Standard,

			Red,
			Orange,
			Yellow,
			Chartreuse,
			Green,
			Spring,
			Cyan,
			Azure,
			Blue,
			Violet,
			Magenta,
			Rose,

			White,
			Gray,
			Black,
		}
		private static SettingHandle<Colors> _biosculpterPodReadyEffecterColor;

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
			_biosculpterPodSize.ValueChanged += (val) => ChangeBiosculpterPodSize((SettingHandle<BiosculpterPodSize>)val);
			ChangeBiosculpterPodSize(_biosculpterPodSize);

			_biosculpterPodReadyEffecterColor = Settings.GetHandle(
				"BiosculpterPodReadyEffecterColor",
				"SY_BNR.BiosculpterPodReadyEffecterColorTitle".Translate(),
				"SY_BNR.BiosculpterPodReadyEffecterColorDesc".Translate(),
				Colors.Standard,
				enumPrefix: "SY_BNR.Color_");
			_biosculpterPodReadyEffecterColor.ValueChanged += (val) => ChangeBiosculpterPodReadyColor((SettingHandle<Colors>)val);
			ChangeBiosculpterPodReadyColor(_biosculpterPodReadyEffecterColor);

			_neuralSuperchargerSize = Settings.GetHandle(
				"neuralSuperchargerSize", 
				"SY_BNR.NeuralSuperchargerTitle".Translate(), 
				"SY_BNR.NeuralSuperchargerDesc".Translate(), 
				NeuralSuperchargerSize.Standard_1x3,
				enumPrefix: "SY_BNR.NeuralSuperchargerSize_");
			_neuralSuperchargerSize.ValueChanged += (val) => ChangeNeuralSuperchargerSize((SettingHandle<NeuralSuperchargerSize>)val);
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
					graphicData = StaticStuff.BiosculpterPodGraphicData_2x1;
					buildingSize = new IntVec2(1, 2);
					fleckSize = new Vector2(1, 2);
					break;
				case BiosculpterPodSize.Modded_1x3:
					graphicData = StaticStuff.BiosculpterPodGraphicData_3x1;
					buildingSize = new IntVec2(1, 3);
					fleckSize = new Vector2(1, 2);
					break;
			}
			
			StaticStuff.BiosculpterPodDef.graphicData = graphicData;
			StaticStuff.BiosculpterPodDef.graphic = graphicData.Graphic;
			StaticStuff.BiosculpterPodDef.size = buildingSize;
			StaticStuff.BiosculpterPodDef.interactionCellOffset = interactionCellOffset;
		}

		private void ChangeBiosculpterPodReadyColor(Colors value)
		{
			Color color;
			switch (value)
			{
				case Colors.Standard:
					color = StaticStuff.OriginalSelectCycleColor;
					break;

				default:
					color = Color.HSVToRGB((((float)value - 1f) / 12f), 1f, 1f);
					break;

				case Colors.White:
					color = new Color(1f, 1f, 1f);
					break;
				case Colors.Gray:
					color = new Color(.5f, .5f, .5f);
					break;
				case Colors.Black:
					color = new Color(0f, 0f, 0f);
					break;
			}

			StaticStuff.BiosculpterPodDef.GetCompProperties<CompProperties_BiosculpterPod>().selectCycleColor = color;
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
					graphicData = StaticStuff.NeuralSuperchargerGraphicData_2x1;
					size = new IntVec2(1, 2);
					break;
			}

			StaticStuff.NeuralSuperchargerDef.graphicData = graphicData;
			StaticStuff.NeuralSuperchargerDef.graphic = graphicData.Graphic;
			StaticStuff.NeuralSuperchargerDef.size = size;
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
