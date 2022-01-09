using HugsLib.Settings;
using System;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace BPaNSResize
{
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
		private SettingHandle<bool> _biosculpterPodInteractionSpotOverlap;

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

			_biosculpterPodInteractionSpotOverlap = Settings.GetHandle(
				"biosculpterPodInteractionSpotOverlap",
				"SY_BNR.BiosculpterPodInteractionSpotOverlapTitle".Translate(),
				"SY_BNR.BiosculpterPodInteractionSpotOverlapDesc".Translate(), 
				false);
			_biosculpterPodInteractionSpotOverlap.ValueChanged += (value) => ChangeBiosculpterPodInteractionSpotOverlap((SettingHandle<bool>)value);
			ChangeBiosculpterPodInteractionSpotOverlap(_biosculpterPodInteractionSpotOverlap);

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

		private void ChangeBiosculpterPodInteractionSpotOverlap(bool allowOverlapping)
		{
			if (!allowOverlapping)
				StaticStuff.BiosculpterPodDef.placeWorkers.AddIfNotContains(typeof(PlaceWorker_PreventInteractionSpotOverlap));
			else
				StaticStuff.BiosculpterPodDef.placeWorkers.Remove(typeof(PlaceWorker_PreventInteractionSpotOverlap));
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
			// Change thingDef size, graphic and ui icon
			thingDef.size = buildingSize;
			thingDef.graphicData = graphicData;
			thingDef.graphic = graphicData.Graphic;
			thingDef.uiIcon = (Texture2D)graphicData.Graphic.MatSouth.mainTexture;

			// Change interaction cell offset if desired
			if (interactionCellOffset != null)
				thingDef.interactionCellOffset = (IntVec3)interactionCellOffset;

			// Change blueprint size and graphic
			thingDef.blueprintDef.size = buildingSize;
			thingDef.blueprintDef.graphicData = graphicData_Blueprint;
			thingDef.blueprintDef.graphic = graphicData_Blueprint.Graphic;

			// Change install blueprint size and graphic
			thingDef.installBlueprintDef.size = buildingSize;
			thingDef.installBlueprintDef.graphicData = graphicData_Blueprint;
			thingDef.installBlueprintDef.graphic = graphicData_Blueprint.Graphic;

			// Change build frame size
			thingDef.frameDef.size = buildingSize;

			// Fix build copy icon proportions; I really wonder if there isn't a better way for doing this...
			foreach (var x in thingDef.designationCategory.AllResolvedDesignators)
			{
				if (x is Designator_Build build && build.PlacingDef == thingDef)
				{
					build.iconProportions = buildingSize.ToVector2();
					break;
				}
			}
		}
	}
}
