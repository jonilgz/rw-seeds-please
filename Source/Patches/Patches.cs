using Verse;
using RimWorld;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace SeedsPleaseRevived
{
    //This patch controls the dropping of seeds upon harvest
    [HarmonyPatch(typeof(Plant), nameof(Plant.PlantCollected))]
    public class Patch_PlantCollected
    {
        public static void Prefix(Plant __instance, Pawn by)
        {
            Seed seedDefX = __instance.def.blueprintDef?.GetModExtension<Seed>();

            if (seedDefX != null && 
            !__instance.def.blueprintDef.thingCategories.NullOrEmpty() && 
            __instance.Growth >= __instance.def.plant.harvestMinGrowth)
            {
                ThingDef seedDef = __instance.def.blueprintDef;

                //Roll to check if a seed is gotten
                if (seedDefX.seedFactor > 0 && Rand.Chance(seedDefX.baseChance))
                {
                    //Try for a bonus seed
                    int count = Rand.Chance(seedDefX.extraChance) ? 2 : 1;

                    float stackCount = seedDefX.seedFactor * count * SeedsPleaseRevived.ModSettings_SeedsPleaseRevived.seedFactorModifier;
                    if (stackCount < 1f)
                    {
                        if (Rand.Chance(stackCount)) stackCount = 1f;
                        else return;
                    } 

                    Thing newSeeds = ThingMaker.MakeThing(seedDef, null);
                    newSeeds.stackCount = Mathf.RoundToInt(stackCount);

                    GenPlace.TryPlaceThing(newSeeds, by.Position, by.Map, ThingPlaceMode.Near);

                    if (!(by.Faction?.def.isPlayer ?? true)) newSeeds.SetForbidden(true);
                }
            }
        }
    }

    //This is responsible for determining which crops show up on the list when you configue a grow zone
    [HarmonyPatch (typeof(Command_SetPlantToGrow), nameof(Command_SetPlantToGrow.IsPlantAvailable))]
    static class Patch_IsPlantAvailable
    {
        public static bool Postfix(bool __result, ThingDef plantDef, Map map)
        {
            if (__result && (plantDef?.blueprintDef?.HasModExtension<Seed>() ?? false))
            {
                return map.listerThings.ThingsOfDef(plantDef.blueprintDef).Count > 0;
            }
            return __result;
        }
    }

    // Patch for ThingSetMaker_ResourcePod.PossiblePodContentsDefs removed for RimWorld 1.6 compatibility.

    //This patchs traders to adjust their stock generation so they won't try to sell seeds you can't even grow
    [HarmonyPatch(typeof(StockGenerator_Tag), nameof(StockGenerator_Tag.GenerateThings))]
    static class Patch_GenerateThings
    {
        static void Prefix(List<ThingDef> ___excludedThingDefs)
        {
            if (SeedsPleaseRevived.ModSettings_SeedsPleaseRevived.noUselessSeeds) 
            {
                //Get a list of wild plants that grow in player's map(s)
                List<ThingDef> wildBiomePlants = new List<ThingDef>();
                foreach (Map map in Current.Game.Maps)
                {
                    if (map.IsPlayerHome) foreach (var x in map.Biome.wildPlants) wildBiomePlants.Add(x.plant);
                }
                
                //Get a list of seeds that are sensitive to biome restrictions
                var allDefs = DefDatabase<ThingDef>.AllDefsListForReading;
                for (int i = allDefs.Count; i-- > 0;)
                {
                    var seed = allDefs[i];
                    var modExt = seed.GetModExtension<Seed>();
                    if (modExt != null && modExt.sources.Any(y => y.plant.mustBeWildToSow && y.plant.purpose != PlantPurpose.Beauty))
                    {
                        //Of those seeds, determine which ones are useless and add them to the excluded defs list
                        if (!wildBiomePlants.Intersect(seed.GetModExtension<Seed>().sources).Any())
                        {
                            if (___excludedThingDefs == null) ___excludedThingDefs = new List<ThingDef>();
                            ___excludedThingDefs.Add(seed);
                        }
                    }
                }
            }
        }
    }
}