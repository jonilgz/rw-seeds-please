using RimWorld;
using Verse;

namespace SeedsPleaseRevived
{
    public static class ResourceBank
    {
        public static readonly string[] knownPrefixes = new string[]
        {
            "VG_Plant", "VGP_", "RC2_", "RC_Plant", "TKKN_Plant", "TKKN_", "TM_", "Ogre_AdvHyd_", "Plant_", "WildPlant", "Wild", "Plant", "tree", "Tree"
        };

        [DefOf]
        public static class Defs
        {
            public static JobDef SowWithSeeds;
            public static ThingCategoryDef SeedExtractable, SeedsCategory;
            public static RecipeDef ExtractSeeds;
        }
    }
}