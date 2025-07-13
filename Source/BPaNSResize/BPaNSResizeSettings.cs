using RimWorld;
using SyControlsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BPaNSResize
{
	#region ENUMS
	public enum BiosculpterPodSize
	{
		Default_3x2,
		Modded_2x2_Left,
		Modded_2x2_Right,
		Modded_1x2,
		Modded_1x3,
	}

	public enum ColorSelector
	{
		R, G, B
	}

	public enum NeuralSuperchargerSize
	{
		Default_1x3,
		Modded_1x2,
	}
	#endregion

	public class BPaNSResizeSettings : ModSettings
	{
		#region CONSTANTS
		public const BiosculpterPodSize Default_BiosculpterPodSize = BiosculpterPodSize.Default_3x2;
		public const bool Default_BiosculpterPodInteractionSpotOverlap = false;

		public const bool Default_BiosculpterPodReadyEffecterAlwaysOn = false;
		public const bool Default_BiosculpterPodReadyEffecterAlwaysOff = false;

		public const NeuralSuperchargerSize Default_NeuralSuperchargerSize = NeuralSuperchargerSize.Default_1x3;
		#endregion

		#region PROPERTIES
		private BiosculpterPodSize _biosculpterPodSize = Default_BiosculpterPodSize;
		public BiosculpterPodSize BiosculpterPodSize
		{
			get => _biosculpterPodSize;
			set => Util.SetValue(ref  _biosculpterPodSize, value, v => ChangeBiosculpterPodSize(v));
		}
		private bool _biosculpterPodInteractionSpotOverlap = Default_BiosculpterPodInteractionSpotOverlap;
		public bool BiosculpterPodInteractionSpotOverlap
		{
			get => _biosculpterPodInteractionSpotOverlap;
			set => Util.SetValue(ref _biosculpterPodInteractionSpotOverlap, value, v => ChangeBiosculpterPodInteractionSpotOverlap(v));
		}

		private bool _biosculpterPodReadyEffecterAlwaysOn = Default_BiosculpterPodReadyEffecterAlwaysOn;
		public bool BiosculpterPodReadyEffecterAlwaysOn
		{
			get => _biosculpterPodReadyEffecterAlwaysOn;
			set => Util.SetValue(ref _biosculpterPodReadyEffecterAlwaysOn, value, v => ChangeBiosculpterPodReadyEffecterAlwaysOn(v));
		}
		private bool _biosculpterPodReadyEffecterAlwaysOff = Default_BiosculpterPodReadyEffecterAlwaysOff;
		public bool BiosculpterPodReadyEffecterAlwaysOff
		{
			get => _biosculpterPodReadyEffecterAlwaysOff;
			set => Util.SetValue(ref _biosculpterPodReadyEffecterAlwaysOff, value, v => ChangeBiosculpterPodReadyEffecterAlwaysOff(v));
		}

		private float _biosculpterPodReadyEffecterColorR = Default_BiosculpterPodReadyEffecterColorR;
		public float BiosculpterPodReadyEffecterColorR
		{
			get => _biosculpterPodReadyEffecterColorR;
			set => Util.SetValue(ref _biosculpterPodReadyEffecterColorR, value, v => ChangeBiosculpterPodReadyEffecterColor(ColorSelector.R, v));
		}
		private float _biosculpterPodReadyEffecterColorG = Default_BiosculpterPodReadyEffecterColorG;
		public float BiosculpterPodReadyEffecterColorG
		{
			get => _biosculpterPodReadyEffecterColorG;
			set => Util.SetValue(ref _biosculpterPodReadyEffecterColorG, value, v => ChangeBiosculpterPodReadyEffecterColor(ColorSelector.G, v));
		}
		private float _biosculpterPodReadyEffecterColorB = Default_BiosculpterPodReadyEffecterColorB;
		public float BiosculpterPodReadyEffecterColorB
		{
			get => _biosculpterPodReadyEffecterColorB;
			set => Util.SetValue(ref _biosculpterPodReadyEffecterColorB, value, v => ChangeBiosculpterPodReadyEffecterColor(ColorSelector.B, v));
		}

		private NeuralSuperchargerSize _neuralSuperchargerSize = Default_NeuralSuperchargerSize;
		public NeuralSuperchargerSize NeuralSuperchargerSize
		{
			get => _neuralSuperchargerSize;
			set => Util.SetValue(ref _neuralSuperchargerSize, value, v => ChangeNeuralSuperchargerSize(v));
		}
		#endregion

		#region FIELDS
		public static readonly float Default_BiosculpterPodReadyEffecterColorR = StaticStuff.OriginalSelectCycleColor.r;
		public static readonly float Default_BiosculpterPodReadyEffecterColorG = StaticStuff.OriginalSelectCycleColor.g;
		public static readonly float Default_BiosculpterPodReadyEffecterColorB = StaticStuff.OriginalSelectCycleColor.b;

		public static readonly BiosculpterPodSize[] BiosculpterPodSizes = (BiosculpterPodSize[])Enum.GetValues(typeof(BiosculpterPodSize));
		public static readonly NeuralSuperchargerSize[] NeuralSuperchargerSizes = (NeuralSuperchargerSize[])Enum.GetValues(typeof(NeuralSuperchargerSize));

		private TargetWrapper<BiosculpterPodSize> _biosculpterPodSizeWrapper;
		private TargetWrapper<NeuralSuperchargerSize> _neuralSuperchargerSizeWrapper;
		#endregion

		#region PUBLIC METHODS
		public void DoSettingsWindowContents(Rect inRect)
		{
			var width = inRect.width;
			var offsetY = 0.0f;

			ControlsBuilder.Begin(inRect);
			try
			{
				if (_biosculpterPodSizeWrapper == null)
					_biosculpterPodSizeWrapper = new TargetWrapper<BiosculpterPodSize>(BiosculpterPodSize);
				BiosculpterPodSize = ControlsBuilder.CreateDropdown(
					ref offsetY,
					width,
					"SY_BNR.BiosculpterPodTitle".Translate(),
					"SY_BNR.BiosculpterPodDesc".Translate(),
					_biosculpterPodSizeWrapper,
					Default_BiosculpterPodSize,
					BiosculpterPodSizes,
					e => e.ToString());
				BiosculpterPodInteractionSpotOverlap = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SY_BNR.BiosculpterPodInteractionSpotOverlapTitle".Translate(),
					"SY_BNR.BiosculpterPodInteractionSpotOverlapDesc".Translate(),
					BiosculpterPodInteractionSpotOverlap,
					Default_BiosculpterPodInteractionSpotOverlap);

				BiosculpterPodReadyEffecterAlwaysOn = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SY_BNR.BiosculpterPodReadyEffecterAlwaysOnTitle".Translate(),
					"SY_BNR.BiosculpterPodReadyEffecterAlwaysOnDesc".Translate(),
					BiosculpterPodReadyEffecterAlwaysOn,
					Default_BiosculpterPodReadyEffecterAlwaysOn);
				BiosculpterPodReadyEffecterAlwaysOff = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SY_BNR.BiosculpterPodReadyEffecterAlwaysOffTitle".Translate(),
					"SY_BNR.BiosculpterPodReadyEffecterAlwaysOffDesc".Translate(),
					BiosculpterPodReadyEffecterAlwaysOff,
					Default_BiosculpterPodReadyEffecterAlwaysOff);

				BiosculpterPodReadyEffecterColorR = ControlsBuilder.CreateNumeric(
					ref offsetY,
					width,
					"SY_BNR.BiosculpterPodReadyEffecterColorRTitle".Translate(),
					"SY_BNR.BiosculpterPodReadyEffecterColorRDesc".Translate(),
					BiosculpterPodReadyEffecterColorR,
					Default_BiosculpterPodReadyEffecterColorR,
					nameof(BiosculpterPodReadyEffecterColorR));
				BiosculpterPodReadyEffecterColorG = ControlsBuilder.CreateNumeric(
					ref offsetY,
					width,
					"SY_BNR.BiosculpterPodReadyEffecterColorGTitle".Translate(),
					"SY_BNR.BiosculpterPodReadyEffecterColorGDesc".Translate(),
					BiosculpterPodReadyEffecterColorG,
					Default_BiosculpterPodReadyEffecterColorG,
					nameof(BiosculpterPodReadyEffecterColorG));
				BiosculpterPodReadyEffecterColorB = ControlsBuilder.CreateNumeric(
					ref offsetY,
					width,
					"SY_BNR.BiosculpterPodReadyEffecterColorBTitle".Translate(),
					"SY_BNR.BiosculpterPodReadyEffecterColorBDesc".Translate(),
					BiosculpterPodReadyEffecterColorB,
					Default_BiosculpterPodReadyEffecterColorB,
					nameof(BiosculpterPodReadyEffecterColorB));


				if (_neuralSuperchargerSizeWrapper == null)
					_neuralSuperchargerSizeWrapper = new TargetWrapper<NeuralSuperchargerSize>(NeuralSuperchargerSize);
				NeuralSuperchargerSize = ControlsBuilder.CreateDropdown(
					ref offsetY,
					width,
					"SY_BNR.NeuralSuperchargerTitle".Translate(),
					"SY_BNR.NeuralSuperchargerDesc".Translate(),
					_neuralSuperchargerSizeWrapper,
					Default_NeuralSuperchargerSize,
					NeuralSuperchargerSizes,
					e => e.ToString());
			}
			finally
			{
				ControlsBuilder.End(offsetY);
			}
		}
		#endregion

		#region OVERRIDES
		public override void ExposeData()
		{
			base.ExposeData();

			bool boolValue;
			float floatValue;

			var biosculpterPodSizeValue = BiosculpterPodSize;
			Scribe_Values.Look(ref biosculpterPodSizeValue, nameof(BiosculpterPodSize), Default_BiosculpterPodSize);
			BiosculpterPodSize = biosculpterPodSizeValue;
			boolValue = BiosculpterPodInteractionSpotOverlap;
			Scribe_Values.Look(ref boolValue, nameof(BiosculpterPodInteractionSpotOverlap), Default_BiosculpterPodInteractionSpotOverlap);
			BiosculpterPodInteractionSpotOverlap = boolValue;

			boolValue = BiosculpterPodReadyEffecterAlwaysOn;
			Scribe_Values.Look(ref boolValue, nameof(BiosculpterPodReadyEffecterAlwaysOn), Default_BiosculpterPodReadyEffecterAlwaysOn);
			BiosculpterPodReadyEffecterAlwaysOn = boolValue;
			boolValue = BiosculpterPodReadyEffecterAlwaysOff;
			Scribe_Values.Look(ref boolValue, nameof(BiosculpterPodReadyEffecterAlwaysOff), Default_BiosculpterPodReadyEffecterAlwaysOff);
			BiosculpterPodReadyEffecterAlwaysOff = boolValue;

			floatValue = BiosculpterPodReadyEffecterColorR;
			Scribe_Values.Look(ref floatValue, nameof(BiosculpterPodReadyEffecterColorR), Default_BiosculpterPodReadyEffecterColorR);
			BiosculpterPodReadyEffecterColorR = floatValue;
			floatValue = BiosculpterPodReadyEffecterColorG;
			Scribe_Values.Look(ref floatValue, nameof(BiosculpterPodReadyEffecterColorG), Default_BiosculpterPodReadyEffecterColorG);
			BiosculpterPodReadyEffecterColorG = floatValue;
			floatValue = BiosculpterPodReadyEffecterColorB;
			Scribe_Values.Look(ref floatValue, nameof(BiosculpterPodReadyEffecterColorB), Default_BiosculpterPodReadyEffecterColorB);
			BiosculpterPodReadyEffecterColorB = floatValue;

			var neuralSuperchargerSize = NeuralSuperchargerSize;
			Scribe_Values.Look(ref neuralSuperchargerSize, nameof(NeuralSuperchargerSize), Default_NeuralSuperchargerSize);
			NeuralSuperchargerSize = neuralSuperchargerSize;
		}
		#endregion

		#region PRIVATE METHODS
		private void ChangeBiosculpterPodSize(BiosculpterPodSize value)
		{
			GraphicData graphicData;
			GraphicData graphicData_Blueprint;
			IntVec2 buildingSize;
			IntVec3 interactionCellOffset = new IntVec3(0, 0, 2);
			switch (value)
			{
				default:
				case BiosculpterPodSize.Default_3x2:
					graphicData = StaticStuff.BiosculpterPodGraphicData_Standard;
					graphicData_Blueprint = StaticStuff.BiosculpterPodGraphicData_Standard_Blueprint;
					buildingSize = new IntVec2(3, 2);
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
			if (on && BiosculpterPodReadyEffecterAlwaysOff)
				BiosculpterPodReadyEffecterAlwaysOff = false;

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
			if (off && BiosculpterPodReadyEffecterAlwaysOn)
				BiosculpterPodReadyEffecterAlwaysOn = false;

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
				case NeuralSuperchargerSize.Default_1x3:
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
		#endregion
	}
}
