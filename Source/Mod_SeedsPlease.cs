using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;
using System.Collections.Generic;
using static SeedsPleaseLite.ModSettings_SeedsPleaseLite;

namespace SeedsPleaseLite
{
	public class Mod_SeedsPlease : Mod
	{
		public Mod_SeedsPlease(ModContentPack content) : base(content)
		{
			base.GetSettings<ModSettings_SeedsPleaseLite>();
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			//========Setup tabs=========
			GUI.BeginGroup(inRect);

			// Tabs and main config area below the top
			float topOffset = 0f;
			Rect mainRect = new Rect(0f, topOffset, inRect.width, inRect.height - topOffset);
			Widgets.DrawMenuSection(mainRect);

			// Tabs
			var tabs = new List<TabRecord>();
			tabs.Add(new TabRecord("Main", delegate { selectedTab = Tab.main; }, selectedTab == Tab.main));
			tabs.Add(new TabRecord("Seedless", delegate { selectedTab = Tab.seedless; }, selectedTab == Tab.seedless));
			//tabs.Add(new TabRecord("Labels", delegate { selectedTab = Tab.labels; }, selectedTab == Tab.labels));
			float tabY = topOffset + 40f;
			TabDrawer.DrawTabs(new Rect(15f, tabY, inRect.width - 30f, Text.LineHeight), tabs);

			float contentY = tabY + Text.LineHeight + 8f;
			float contentHeight = inRect.height - (tabY + Text.LineHeight + 16f);
			Rect contentRect = new Rect(15f, contentY, inRect.width - 30f, contentHeight);

			if (selectedTab == Tab.main)
			{
				Listing_Standard options = new Listing_Standard();
				options.Begin(contentRect);
				options.GapLine();
				options.Label("SPL.Settings.MarketValueModifier".Translate("100%", "20%", "500%") + marketValueModifier.ToStringPercent(), -1f, "SPL.Settings.MarketValueModifier.Desc".Translate());
				marketValueModifier = options.Slider(marketValueModifier, 0.2f, 5f);

				options.Label("SPL.Settings.SeedExtractionModifier".Translate("100%", "20%", "500%") + extractionModifier.ToStringPercent(), -1f, "SPL.Settings.SeedExtractionModifier.Desc".Translate("4"));
				extractionModifier = options.Slider(extractionModifier, 0.2f, 5f);

				options.Label("SPL.Settings.SeedFactorModifier".Translate("100%", "20%", "500%") + seedFactorModifier.ToStringPercent(), -1f, "SPL.Settings.SeedFactorModifier.Desc".Translate("1"));
				seedFactorModifier = options.Slider(seedFactorModifier, 0.2f, 5f);

				options.CheckboxLabeled("SPL.Settings.NoUselessSeeds".Translate(), ref noUselessSeeds, "SPL.Settings.NoUselessSeeds.Desc".Translate());
				options.CheckboxLabeled("SPL.Settings.ClearSnow".Translate(), ref clearSnow, "SPL.Settings.ClearSnow.Desc".Translate());

				options.CheckboxLabeled("SPL.Settings.EdibleSeeds".Translate(), ref edibleSeeds, "SPL.Settings.EdibleSeeds.Desc".Translate());
				options.End();
			}
			else if (selectedTab == Tab.seedless)
			{
				Widgets.DrawMenuSection(contentRect);
				// Draw restart label above the scroll view
				Rect restartRect = new Rect(contentRect.x, contentRect.y, contentRect.width, 28f);
				Widgets.Label(restartRect, "SPL.RequiresRestart".Translate());
				float scrollY = contentRect.y + restartRect.height + 4f;
				float scrollHeight = contentRect.height - restartRect.height - 4f;
				Rect scrollRect = new Rect(contentRect.x, scrollY, contentRect.width, scrollHeight);
				Rect innerRect = new Rect(0f, 0f, contentRect.width - 16f, (OptionsDrawUtility.lineNumber + 1) * 22f); // +1 for extra space
				Widgets.BeginScrollView(scrollRect, ref scrollPos, innerRect, true);
				Listing_Standard options = new Listing_Standard();
				options.Begin(innerRect);
				options.DrawList(innerRect);
				options.End();
				Widgets.EndScrollView();
			}
			//else if (selectedTab == Tab.labels)
			//{
			//    Listing_Standard options = new Listing_Standard();
			//    options.Begin(contentRect);
			//    options.Label("SPL.Settings.LabelsDesc".Translate());
			//    options.End();
			//}
			GUI.EndGroup();
		}

		public override string SettingsCategory()
		{
			return "Seeds Please: Revived";
		}

		public override void WriteSettings()
		{
			try
			{
				SeedsPleaseUtility.ProcessInversions();	
			}
			catch (System.Exception ex)
			{
				Log.Error("[Seeds Please: Revived] Failed to process user settings. Skipping...\n" + ex);
			}
			
			base.WriteSettings();
		}
	}

	public class ModSettings_SeedsPleaseLite : ModSettings
	{
		public override void ExposeData()
		{
			Scribe_Values.Look(ref marketValueModifier, "marketValueModifier", 1f);
			Scribe_Values.Look(ref extractionModifier, "extractionModifier", 1f);
			Scribe_Values.Look(ref seedFactorModifier, "seedFactorModifier", 1f);
			Scribe_Values.Look(ref noUselessSeeds, "noUselessSeeds", true);
			Scribe_Values.Look(ref clearSnow, "clearSnow");
			Scribe_Values.Look(ref edibleSeeds, "edibleSeeds", true);
			Scribe_Collections.Look(ref seedlessInversions, "seedless", LookMode.Value);
			
			base.ExposeData();
		}

		public static float marketValueModifier = 1f, extractionModifier = 1f, seedFactorModifier = 1f;
		public static bool noUselessSeeds = true, clearSnow, edibleSeeds = true;
		public static HashSet<string> seedlessInversions;
		public static HashSet<ushort> seedlessCache;
		public static Tab selectedTab = Tab.main;
		public enum Tab { main, seedless, labels };
		public static Vector2 scrollPos;
	}
}