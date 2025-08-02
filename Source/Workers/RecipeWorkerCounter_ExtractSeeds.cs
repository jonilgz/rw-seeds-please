using RimWorld;
using Verse;
using System.Linq;

namespace SeedsPleaseLite
{
	/// <summary>
	/// This is responsible for the seed extraction recipe's ability to do the "Do until X" bill type, explaining how/what to count.
	/// </summary>
	public class RecipeWorkerCounter_ExtractSeeds : RecipeWorkerCounter
	{
		public override bool CanCountProducts(Bill_Production bill)
		{
			if (bill.ingredientFilter.AllowedThingDefs.Count<ThingDef>() != 1)
			{
				LongEventHandler.QueueLongEvent(() => HelperMessage(), null, false, null);
				return false;
			}
			return true;
		}

		void HelperMessage()
		{
			Messages.Clear();
			Messages.Message("SPL.BillHelp".Translate(), MessageTypeDefOf.RejectInput, false);
		}

		public override int CountProducts(Bill_Production bill)
		{
			if (bill.ingredientFilter.AllowedThingDefs.Count<ThingDef>() != 1) return 0;
			return bill.Map.resourceCounter.GetCount(bill.ingredientFilter.AllowedThingDefs.First<ThingDef>().butcherProducts[0].thingDef);
		}

		public override string ProductsDescription(Bill_Production bill)
		{
			if (bill.ingredientFilter.AllowedThingDefs.Count<ThingDef>() != 1) return "Invalid";
			return bill.ingredientFilter.AllowedThingDefs.First<ThingDef>().butcherProducts[0].thingDef.label;
		}

		// Removed CanPossiblyStoreInStockpile as it does not exist in RimWorld 1.6 base class.
	}
}
