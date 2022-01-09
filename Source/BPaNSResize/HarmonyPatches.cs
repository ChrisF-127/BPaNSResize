using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;

namespace BPaNSResize
{
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
