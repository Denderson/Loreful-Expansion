using loremiscExpansion.NPCs.Boris.lsfUtils.DevtoolsObjects.LocalGravity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static loremiscExpansion.Plugin;

namespace loremiscExpansion.NPCs.Collector
{
    public class CollectorState : NPCState
    {
        public float currentPlayerReputation = 0;
        public int pearls = 0;
        public int attacks = 0;
        public int agression = 0;
        public bool givenBorisResidue = false;
        public bool huntingBoris = false;

        public CollectorState() : base()
        {
            currentPlayerReputation = 0;
            pearls = 0;
            attacks = 0;
            givenBorisResidue = false;
            huntingBoris = false;

            currentRegion = "AUSG";
        }

        public static List<string> collectorSpots = [];
        public override List<string> BannedRegions()
        {
            List<string> bannedRegions = base.BannedRegions();
            List<string> extraBannedRegions = ["AUSS", "AUHZ", "AUND"];
            return bannedRegions.Union(extraBannedRegions).ToList();
        }

        public override void Tick()
        {
            base.Tick();
            agression--;
        }

        public override void SetWanderingScore(string region)
        {
            if (collectorSpots == null || collectorSpots.Count() <= 0)
            {
                Log.LogMessage("Collector spots are null!");
                base.SetWanderingScore(region);
                return;
            }
            wanderingScore = 0;
            foreach (string spot in collectorSpots) if (spot.StartsWith(region)) wanderingScore++;
        }

        public static void LoadCollectorSpots()
        {
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
            StreamReader reader = new(path);
            collectorSpots = reader.ReadToEnd().Split('\r', '\n').ToList();
        }
    }
}
