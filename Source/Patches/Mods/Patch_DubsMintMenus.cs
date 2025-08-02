using System.Reflection;
using HarmonyLib;
using Verse;

namespace SeedsPleaseRevived
{
    [HarmonyPatch]
    static class Patch_DubsMintMenus
    {
        static MethodBase target;

        static bool Prepare()
        {
            target = AccessTools.DeclaredMethod(AccessTools.TypeByName("DubsMintMenus.Dialog_FancyDanPlantSetterBob"), "IsPlantAvailable");
            return target != null;
        }

        static MethodBase TargetMethod()
        {
            return target;
        }

        static bool Postfix(bool __result, ThingDef plantDef, Map map)
        {
            return Patch_IsPlantAvailable.Postfix(__result, plantDef, map);
        }
    }
}