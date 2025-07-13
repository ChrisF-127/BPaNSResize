using System;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace BPaNSResize
{
	public class BPaNSResize : Mod
	{
		#region PROPERTIES
		public static BPaNSResize Instance { get; private set; }
		public static BPaNSResizeSettings Settings { get; private set; }
		#endregion

		#region CONSTRUCTORS
		public BPaNSResize(ModContentPack content) : base(content)
		{
			Instance = this;

			LongEventHandler.ExecuteWhenFinished(Initialize);
		}
		#endregion

		#region PUBLIC METHODS
		public void ChangeBiosculpterPodSize(BiosculpterPodSize value)
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

		public void ChangeBiosculpterPodInteractionSpotOverlap(bool allowOverlapping)
		{
			if (!allowOverlapping)
				StaticStuff.BiosculpterPodDef.placeWorkers.AddIfNotContains(typeof(PlaceWorker_PreventInteractionSpotOverlap));
			else
				StaticStuff.BiosculpterPodDef.placeWorkers.Remove(typeof(PlaceWorker_PreventInteractionSpotOverlap));
		}

		public void ChangeBiosculpterPodReadyEffecterAlwaysOn(bool on)
		{
			if (on && Settings.BiosculpterPodReadyEffecterAlwaysOff)
				Settings.BiosculpterPodReadyEffecterAlwaysOff = false;

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
		public void ChangeBiosculpterPodReadyEffecterAlwaysOff(bool off)
		{
			if (off && Settings.BiosculpterPodReadyEffecterAlwaysOn)
				Settings.BiosculpterPodReadyEffecterAlwaysOn = false;

			StaticStuff.BiosculpterPodDef.GetCompProperties<CompProperties_BiosculpterPod>().readyEffecter = off ? null : StaticStuff.BiosculpterPod_Ready;
		}
		public void ChangeBiosculpterPodReadyEffecterColor(ColorSelector rgb, float value)
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

		public void ChangeNeuralSuperchargerSize(NeuralSuperchargerSize value)
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

		public void ChangeDef(ThingDef thingDef, GraphicData graphicData, GraphicData graphicData_Blueprint, IntVec2 buildingSize, IntVec3? interactionCellOffset = null)
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

		#region OVERRIDES
		public override string SettingsCategory() =>
			"Biosculpter Pod and Neural Supercharger Resize";

		public override void DoSettingsWindowContents(Rect inRect)
		{
			base.DoSettingsWindowContents(inRect);

			Settings.DoSettingsWindowContents(inRect);
		}
		#endregion

		#region PRIVATE METHODS
		private void Initialize()
		{
			Settings = GetSettings<BPaNSResizeSettings>();
		}
		#endregion
	}
}
