using System.Reflection;
using HarmonyLib;
using UnglitchedAttraction;

public class FredbearPatch : IPatch
{
    public MethodBase TargetMethod => typeof(nFredbear_AI1).GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);

    public HarmonyMethod Patch => new HarmonyMethod(
        typeof(FredbearPatch).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static)
    );

    public string debugName => "nFredbear_AI1.Update";

    public PatchType patchType => PatchType.Prefix;

    public static void Prefix(nFredbear_AI1 __instance)
    {
        __instance.stade = -1f;
    }
}