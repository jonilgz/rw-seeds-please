﻿using System.Collections.Generic;
using Verse;

namespace SeedsPleaseRevived
{
	public class Seed : DefModExtension
	{
		public float seedFactor = 1f;
		public float baseChance = 0.95f; //% chance of getting a seed returned on harvest
		public float extraChance = 0.15f; //% chance of getting a bonus seed
		public int extractionValue = 4; //How many seeds are extracted per recipe batch
		public Extractable extractable = Extractable.Normal;
		//Normal follows the default rules on if a plant has an extractable seed or not. True and false override the results.
        public enum Extractable { Normal, True, False }
		public ThingDef harvestOverride = null; //Make the plant drop something else instead of its normal product
		public float harvestFactor = 1f; //Multiply the usual yield, usually used with harvestOverride
		public List<ThingDef> sources = new List<ThingDef> (); //List of plants this seed may come from
		public ThingDef plant = null; //??
		public int priority = 10;
	}

	public class Seedless : DefModExtension { }
}