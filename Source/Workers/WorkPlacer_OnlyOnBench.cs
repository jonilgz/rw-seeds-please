using Verse;
using RimWorld;
using System.Linq;

namespace SeedsPleaseRevived
{
	public class WorkPlacer_OnlyOnBench : PlaceWorker
	{
		public WorkPlacer_OnlyOnBench() { }
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{		
			//Try to determine if this is a workbench that deals with food
			if (map.thingGrid.ThingsListAtFast(loc).Any(x => x.def.building != null && x.def.building.isMealSource && x is not Building_NutrientPasteDispenser)) return true;
			return new AcceptanceReport("SPL.SeedSpotError".Translate());
		}
	}
}