using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;
using System.Linq;
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

			Rect rect = new Rect(0f, 32f, inRect.width, inRect.height - 32f);
			Widgets.DrawMenuSection(rect);
			
			Listing_Standard options = new Listing_Standard();
			options.Begin(inRect.ContractedBy(15f));
			options.Label("SPL.RequiresRestart".Translate());
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

			//============

			//Record positioning before closing out the lister...
			Rect seedlessFilterRect = inRect.ContractedBy(15f);
			// curY and listingRect are not accessible; use options.GetCurY() if available, otherwise estimate position
			float curY = options.GetType().GetProperty("CurY", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.GetValue(options) as float? ?? 0f;
			seedlessFilterRect.y = curY + 95f;
			seedlessFilterRect.height = inRect.height - curY - 105f; //Use remaining space

			// ColumnWidth: set to inRect width minus margin
			options.ColumnWidth = inRect.width - 30f;
			options.End();

			//========Setup tabs=========
			var tabs = new List<TabRecord>();
			tabs.Add(new TabRecord("Seedless", delegate { selectedTab = Tab.seedless; }, selectedTab == Tab.seedless));
			tabs.Add(new TabRecord("Labels", delegate { selectedTab = Tab.labels; }, selectedTab == Tab.labels));
			
			Widgets.DrawMenuSection(seedlessFilterRect); //Used to make the background light grey with white border
			TabDrawer.DrawTabs(new Rect(seedlessFilterRect.x, seedlessFilterRect.y, seedlessFilterRect.width, Text.LineHeight), tabs);

			//========Between tabs and scroll body=========
			options.Begin(new Rect (seedlessFilterRect.x + 10, seedlessFilterRect.y + 10, seedlessFilterRect.width - 10f, seedlessFilterRect.height - 10f));
				if (selectedTab == Tab.seedless)
				{
					options.Label("SPL.Settings.SeedlessDesc".Translate());
				}
				else
				{
					options.Label("SPL.Settings.LabelsDesc".Translate());
				}
			options.End();
			//========Scroll area=========
			seedlessFilterRect.y += 30f;
			seedlessFilterRect.yMax -= 30f;
			Rect weaponsFilterInnerRect = new Rect(0f, 0f, seedlessFilterRect.width - 30f, (OptionsDrawUtility.lineNumber + 2) * 22f);
			Widgets.BeginScrollView(seedlessFilterRect, ref scrollPos, weaponsFilterInnerRect , true);
				options.Begin(weaponsFilterInnerRect);
				options.DrawList(inRect);
				options.End();
			Widgets.EndScrollView();
			
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
		public static Tab selectedTab = Tab.seedless;
		public enum Tab { seedless, labels };
		public static Vector2 scrollPos;
	}
}