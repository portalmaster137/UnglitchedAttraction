using System.Reflection;
using HarmonyLib;
using UnglitchedAttraction;

public class FreddlePatch : IPatch 
{
    public MethodBase TargetMethod => typeof(Freddles_AI1).GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);

    public HarmonyMethod Patch => new HarmonyMethod(
        typeof(FreddlePatch).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static)
    );

    public string debugName => "Freddles_AI1.Update";
    public PatchType patchType => PatchType.Prefix;

    private static void Prefix(Freddles_AI1 __instance)
    {
        typeof(Freddles_AI1).GetProperty("stade", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, 0f);
    }

}