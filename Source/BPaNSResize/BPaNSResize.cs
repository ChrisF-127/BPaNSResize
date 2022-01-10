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

		private SettingHandle<bool> _biosculpterPodReadyEffecterAlwaysOn;
		private SettingHandle<bool> _biosculpterPodReadyEffecterAlwaysOff;
		enum ColorSelector
		{
			R, G, B
		}
		private SettingHandle<float> _biosculpterPodReadyEffecterColorR;
		private SettingHandle<float> _biosculpterPodReadyEffecterColorG;
		private SettingHandle<float> _biosculpterPodReadyEffecterColorB;

		enum NeuralSuperchargerSize
		{
			Standard_1x3,
			Modded_1x2,
		}
		private SettingHandle<NeuralSuperchargerSize> _neuralSuperchargerSize;

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

			_biosculpterPodInteractionSpotOverlap = Settings.GetHandle(
				"biosculpterPodInteractionSpotOverlap",
				"SY_BNR.BiosculpterPodInteractionSpotOverlapTitle".Translate(),
				"SY_BNR.BiosculpterPodInteractionSpotOverlapDesc".Translate(), 
				false);
			_biosculpterPodInteractionSpotOverlap.ValueChanged += (value) => ChangeBiosculpterPodInteractionSpotOverlap((SettingHandle<bool>)value);


			_biosculpterPodReadyEffecterAlwaysOn = Settings.GetHandle(
				"biosculpterPodReadyEffecterAlwaysOn",
				"SY_BNR.BiosculpterPodReadyEffecterAlwaysOnTitle".Translate(),
				"SY_BNR.BiosculpterPodReadyEffecterAlwaysOnDesc".Translate(),
				true);
			_biosculpterPodReadyEffecterAlwaysOn.ValueChanged += (value => ChangeBiosculpterPodReadyEffecterAlwaysOn((SettingHandle<bool>)value));
			_biosculpterPodReadyEffecterAlwaysOff = Settings.GetHandle(
				"biosculpterPodReadyEffecterAlwaysOff",
				"SY_BNR.BiosculpterPodReadyEffecterAlwaysOffTitle".Translate(),
				"SY_BNR.BiosculpterPodReadyEffecterAlwaysOffDesc".Translate(),
				true);
			_biosculpterPodReadyEffecterAlwaysOff.ValueChanged += (value => ChangeBiosculpterPodReadyEffecterAlwaysOff((SettingHandle<bool>)value));

			_biosculpterPodReadyEffecterColorR = Settings.GetHandle(
				"biosculpterPodReadyEffecterColorR",
				"SY_BNR.BiosculpterPodReadyEffecterColorRTitle".Translate(),
				"SY_BNR.BiosculpterPodReadyEffecterColorRDesc".Translate(),
				StaticStuff.OriginalSelectCycleColor.r);
			_biosculpterPodReadyEffecterColorR.ValueChanged += (value) => ChangeBiosculpterPodReadyEffecterColor(ColorSelector.R, (SettingHandle<float>)value);
			_biosculpterPodReadyEffecterColorG = Settings.GetHandle(
				"biosculpterPodReadyEffecterColorG",
				"SY_BNR.BiosculpterPodReadyEffecterColorGTitle".Translate(),
				"SY_BNR.BiosculpterPodReadyEffecterColorGDesc".Translate(),
				StaticStuff.OriginalSelectCycleColor.g);
			_biosculpterPodReadyEffecterColorG.ValueChanged += (value) => ChangeBiosculpterPodReadyEffecterColor(ColorSelector.G, (SettingHandle<float>)value);
			_biosculpterPodReadyEffecterColorB = Settings.GetHandle(
				"biosculpterPodReadyEffecterColorB",
				"SY_BNR.BiosculpterPodReadyEffecterColorBTitle".Translate(),
				"SY_BNR.BiosculpterPodReadyEffecterColorBDesc".Translate(),
				StaticStuff.OriginalSelectCycleColor.b);
			_biosculpterPodReadyEffecterColorB.ValueChanged += (value) => ChangeBiosculpterPodReadyEffecterColor(ColorSelector.B, (SettingHandle<float>)value);


			_neuralSuperchargerSize = Settings.GetHandle(
				"neuralSuperchargerSize", 
				"SY_BNR.NeuralSuperchargerTitle".Translate(), 
				"SY_BNR.NeuralSuperchargerDesc".Translate(), 
				NeuralSuperchargerSize.Standard_1x3,
				enumPrefix: "SY_BNR.NeuralSuperchargerSize_");
			_neuralSuperchargerSize.ValueChanged += (value) => ChangeNeuralSuperchargerSize((SettingHandle<NeuralSuperchargerSize>)value);


			ChangeBiosculpterPodSize(_biosculpterPodSize);
			ChangeBiosculpterPodInteractionSpotOverlap(_biosculpterPodInteractionSpotOverlap);

			ChangeBiosculpterPodReadyEffecterAlwaysOn(_biosculpterPodReadyEffecterAlwaysOn);
			ChangeBiosculpterPodReadyEffecterAlwaysOff(_biosculpterPodReadyEffecterAlwaysOff);

			ChangeBiosculpterPodReadyEffecterColor(ColorSelector.R, _biosculpterPodReadyEffecterColorR);
			ChangeBiosculpterPodReadyEffecterColor(ColorSelector.G, _biosculpterPodReadyEffecterColorG);
			ChangeBiosculpterPodReadyEffecterColor(ColorSelector.B, _biosculpterPodReadyEffecterColorB);

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

		private void ChangeBiosculpterPodReadyEffecterAlwaysOn(bool on)
		{
			if (on && _biosculpterPodReadyEffecterAlwaysOff.Value)
				_biosculpterPodReadyEffecterAlwaysOff.Value = false;

			if (on)
			{
				StaticStuff.BiosculpterScanner_Ready.fadeInTime = 0f;
				StaticStuff.BiosculpterScanner_Ready.fadeOutTime = 0f;
				// new motes are generated AT and not AFTER "ticksBetweenMotes" (e.g. a value of 1 generates a mote on every tick)
				// 	this causes the time to be 1 tick shorter than expected (179 instead of 180 ticks)!
				StaticStuff.BiosculpterScanner_Ready.solidTime = (StaticStuff.BiosculpterPod_Ready.children[0].ticksBetweenMotes - 1) / 60f;
			}
			else
			{
				StaticStuff.BiosculpterScanner_Ready.fadeInTime = StaticStuff.OriginalBiosculpterScanner_ReadyValues.Item1;
				StaticStuff.BiosculpterScanner_Ready.fadeOutTime = StaticStuff.OriginalBiosculpterScanner_ReadyValues.Item2;
				StaticStuff.BiosculpterScanner_Ready.solidTime = StaticStuff.OriginalBiosculpterScanner_ReadyValues.Item3;
			}
		}
		private void ChangeBiosculpterPodReadyEffecterAlwaysOff(bool off)
		{
			if (off && _biosculpterPodReadyEffecterAlwaysOn.Value)
				_biosculpterPodReadyEffecterAlwaysOn.Value = false;

			StaticStuff.BiosculpterPodDef.GetCompProperties<CompProperties_BiosculpterPod>().readyEffecter = off ? null : StaticStuff.BiosculpterPod_Ready;
		}
		private void ChangeBiosculpterPodReadyEffecterColor(ColorSelector rgb, float value)
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
