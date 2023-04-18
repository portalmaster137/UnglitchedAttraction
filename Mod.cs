using System;
using System.Linq;
using HarmonyLib;
using MelonLoader;

[assembly: MelonInfo(typeof(UnglitchedAttraction.Mod), "UnglitchedAttraction", "1.0.0", "Porta")]
[assembly: MelonGame("PowerLine Studios", "The Glitched Attraction")]

namespace UnglitchedAttraction
{
    public class Mod : MelonMod
    {
        public static bool MASTER_ENABLED;
        public static bool DEBUG;
        public static HarmonyLib.Harmony? harmony;

        public override void OnInitializeMelon()
        {
            harmony = new HarmonyLib.Harmony("UnglitchedAttraction");
            if (harmony is null) {
                MelonLogger.Error("Failed to create Harmony instance");
                return;
            }
            var prefs = LoadPrefs.Register();
            MelonLogger.Msg("UnglitchedAttraction prefs loaded");
            MASTER_ENABLED = prefs.enabled.Value;
            DEBUG = prefs.debug.Value;
            if (MASTER_ENABLED) {
                PatchModular();
                MelonLogger.Msg("UnglitchedAttraction enabled");
            }
        }

        public static void PatchModular()
        {
            if (harmony is null) {
                MelonLogger.Error("Failed to create Harmony instance");
                return;
            }
            var patches = typeof(Mod).Assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IPatch)));
            var trans = typeof(Mod).Assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ITranspiler)));
            if (DEBUG) {
                MelonLogger.Msg("Patching " + patches.Count() + trans.Count() + " methods");
            }
            foreach (var transpiler in trans)
            {
                var ins = Activator.CreateInstance(transpiler) as ITranspiler;
                if (ins is null) {
                    MelonLogger.Warning("Failed to create transpiler instance: " + transpiler);
                    continue;
                }
                harmony.Patch(ins.TargetMethod, transpiler: ins.Patch);
            }
            foreach (var patch in patches)
            {
                var ins = Activator.CreateInstance(patch) as IPatch;
                if (ins is null) {
                    MelonLogger.Warning("Failed to create patch instance: " + patch);
                    continue;
                }
                switch (ins.patchType)
                {
                    case PatchType.Prefix:
                        harmony.Patch(ins.TargetMethod, prefix: ins.Patch);
                        break;
                    case PatchType.Postfix:
                        harmony.Patch(ins.TargetMethod, postfix: ins.Patch);
                        break;
                    default:
                        MelonLogger.Warning("Unknown patch type: " + ins.patchType);
                        break;
                }
                 MelonLogger.Msg("Patched " + ins.debugName);
            }  
        }
    }

    public enum PatchType
    {
        Prefix,
        Postfix,
        Transpiler
    }

    public interface IPatch
    {
        System.Reflection.MethodBase TargetMethod { get; }
        HarmonyMethod Patch { get; }
        string debugName { get; }
        PatchType patchType { get; }
    }

    public interface ITranspiler {
        System.Reflection.MethodBase TargetMethod { get; }
        HarmonyMethod Patch { get; }
        string debugName { get; }
    }

}