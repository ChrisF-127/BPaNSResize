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
			set => Util.SetValue(ref  _biosculpterPodSize, value, v => BPaNSResize.Instance.ChangeBiosculpterPodSize(v));
		}
		private bool _biosculpterPodInteractionSpotOverlap = Default_BiosculpterPodInteractionSpotOverlap;
		public bool BiosculpterPodInteractionSpotOverlap
		{
			get => _biosculpterPodInteractionSpotOverlap;
			set => Util.SetValue(ref _biosculpterPodInteractionSpotOverlap, value, v => BPaNSResize.Instance.ChangeBiosculpterPodInteractionSpotOverlap(v));
		}

		private bool _biosculpterPodReadyEffecterAlwaysOn = Default_BiosculpterPodReadyEffecterAlwaysOn;
		public bool BiosculpterPodReadyEffecterAlwaysOn
		{
			get => _biosculpterPodReadyEffecterAlwaysOn;
			set => Util.SetValue(ref _biosculpterPodReadyEffecterAlwaysOn, value, v => BPaNSResize.Instance.ChangeBiosculpterPodReadyEffecterAlwaysOn(v));
		}
		private bool _biosculpterPodReadyEffecterAlwaysOff = Default_BiosculpterPodReadyEffecterAlwaysOff;
		public bool BiosculpterPodReadyEffecterAlwaysOff
		{
			get => _biosculpterPodReadyEffecterAlwaysOff;
			set => Util.SetValue(ref _biosculpterPodReadyEffecterAlwaysOff, value, v => BPaNSResize.Instance.ChangeBiosculpterPodReadyEffecterAlwaysOff(v));
		}

		private float _biosculpterPodReadyEffecterColorR = Default_BiosculpterPodReadyEffecterColorR;
		public float BiosculpterPodReadyEffecterColorR
		{
			get => _biosculpterPodReadyEffecterColorR;
			set => Util.SetValue(ref _biosculpterPodReadyEffecterColorR, value, v => BPaNSResize.Instance.ChangeBiosculpterPodReadyEffecterColor(ColorSelector.R, v));
		}
		private float _biosculpterPodReadyEffecterColorG = Default_BiosculpterPodReadyEffecterColorG;
		public float BiosculpterPodReadyEffecterColorG
		{
			get => _biosculpterPodReadyEffecterColorG;
			set => Util.SetValue(ref _biosculpterPodReadyEffecterColorG, value, v => BPaNSResize.Instance.ChangeBiosculpterPodReadyEffecterColor(ColorSelector.G, v));
		}
		private float _biosculpterPodReadyEffecterColorB = Default_BiosculpterPodReadyEffecterColorB;
		public float BiosculpterPodReadyEffecterColorB
		{
			get => _biosculpterPodReadyEffecterColorB;
			set => Util.SetValue(ref _biosculpterPodReadyEffecterColorB, value, v => BPaNSResize.Instance.ChangeBiosculpterPodReadyEffecterColor(ColorSelector.B, v));
		}

		private NeuralSuperchargerSize _neuralSuperchargerSize = Default_NeuralSuperchargerSize;
		public NeuralSuperchargerSize NeuralSuperchargerSize
		{
			get => _neuralSuperchargerSize;
			set => Util.SetValue(ref _neuralSuperchargerSize, value, v => BPaNSResize.Instance.ChangeNeuralSuperchargerSize(v));
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
	}
}
