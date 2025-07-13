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
