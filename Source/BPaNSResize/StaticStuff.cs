using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;
using System.Reflection;
using System;

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
		public static EffecterDef BiosculpterPod_Ready;
		public static FleckDef BiosculpterScanner_Ready;
		// FadeIn, FadeOut, Solid
		public static Tuple<float, float, float> OriginalBiosculpterScanner_ReadyValues;

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
			foreach (var def in DefDatabase<EffecterDef>.AllDefs)
			{
				if (biosculpterPod_Operating == null
					   && def.defName == "BiosculpterPod_Operating")
					biosculpterPod_Operating = def;
				else if (BiosculpterPod_Ready == null
					   && def.defName == "BiosculpterPod_Ready")
					BiosculpterPod_Ready = def;

				if (biosculpterPod_Operating != null
					&& BiosculpterPod_Ready != null)
					break;
			}
			biosculpterPod_Operating.offsetTowardsTarget = new FloatRange(0.5f, 0.5f);
			BiosculpterPod_Ready.offsetTowardsTarget = new FloatRange(0.5f, 0.5f);

			// Resize FleckDefs for the Effecters to look more fitting for the smaller buildings
			FleckDef biosculpterScanner_Forward = null;
			FleckDef biosculpterScanner_Backward = null;
			foreach (var def in DefDatabase<FleckDef>.AllDefs)
			{
				if (biosculpterScanner_Forward == null
					   && def.defName == "BiosculpterScanner_Forward")
					biosculpterScanner_Forward = def;
				else if (biosculpterScanner_Backward == null
					   && def.defName == "BiosculpterScanner_Backward")
					biosculpterScanner_Backward = def;
				else if (BiosculpterScanner_Ready == null
					   && def.defName == "BiosculpterScanner_Ready")
					BiosculpterScanner_Ready = def;

				if (biosculpterScanner_Forward != null
					&& biosculpterScanner_Backward != null
					&& BiosculpterScanner_Ready != null)
					break;
			}
			biosculpterScanner_Forward.graphicData.drawSize = new Vector2(1.5f, 0.5f); // standard is 3x1
			biosculpterScanner_Backward.graphicData.drawSize = new Vector2(1f, 0.5f); // standard is 2x1
			BiosculpterScanner_Ready.graphicData.drawSize = new Vector2(1f, 2f); // standard is 2x2
			OriginalBiosculpterScanner_ReadyValues = new Tuple<float, float, float>(BiosculpterScanner_Ready.fadeInTime, BiosculpterScanner_Ready.fadeOutTime, BiosculpterScanner_Ready.solidTime);

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
}
