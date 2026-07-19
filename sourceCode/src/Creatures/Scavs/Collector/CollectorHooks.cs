using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using static loremiscExpansion.Plugin;

namespace loremiscExpansion.Creatures.Scavs.Collector
{
    internal class CollectorHooks
    {

        static public void RegionState_CycleTick(On.RegionState.orig_RainCycleTick orig, RegionState self, int ticks, int foodRepBonus)
        {
            orig(self, ticks, foodRepBonus);
            /*if (self.creature.creatureTemplate.type == Enums.ScavCollector)
            {
                ((self.creature.realizedCreature) as Collector).myStats.WanderingScore--;
                if (((self.creature.realizedCreature) as Collector).myStats.WanderingScore == 0)
                {

                }
            }
            */

        }

        public void WanderToRegion()
        {

        }

        public static void RainWorld_Start(On.RainWorld.orig_Start orig, RainWorld self)
        {
            orig(self);
            // load here stuff that requires AssetManager here (like ResolveFilePath)

            string path;
            try
            {
                path = AssetManager.ResolveFilePath("lorefulExpansion/collectorSpots.txt");
            }
            catch (Exception ex)
            {
                Log.LogWarning($"CollectorStats.LoadRegions: AssetManager not ready or path resolution failed: {ex.Message}");
                return;
            }

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Log.LogWarning("collectorSpots.txt not found.");
                return;
            }

            CollectorStats.LoadRegions();
        }

    }
}
