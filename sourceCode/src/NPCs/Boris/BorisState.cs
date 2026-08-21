using loremiscExpansion.NPCs.Apostle.lsfUtils.DevtoolsObjects.LocalGravity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static loremiscExpansion.Plugin;

namespace loremiscExpansion.NPCs.Boris
{
    public class BorisState : NPCState
    {
        public int residue = 0;
        public int cyclesSinceLastResidue = 0;
        public int questProgression = 0;
        public bool metApostle = false;
        public bool metCollector = false;

        public BorisState() : base() 
        {
            residue = 0;
            cyclesSinceLastResidue = 0;
            questProgression = 0;
            metApostle = false;
            metCollector = false;

            currentRegion = "AUSS";
        }

        public static List<string> borisSpots = [];
        public override List<string> BannedRegions()
        {
            List<string> bannedRegions = base.BannedRegions();
            List<string> extraBannedRegions = ["AUND"];
            if (!string.IsNullOrEmpty(playerCurrentRegion)) extraBannedRegions.Add(playerCurrentRegion);
            return bannedRegions.Union(extraBannedRegions).ToList();
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void SetWanderingScore(string region)
        {
            if (borisSpots == null || borisSpots.Count() <= 0)
            {
                Log.LogMessage("Boris spots are null!");
                base.SetWanderingScore(region);
                return;
            }
            wanderingScore = 0;
            foreach (string spot in borisSpots) if (spot.StartsWith(region)) wanderingScore++;
        }

        public static void LoadBorisSpots()
        {
            string path;
            try
            {
                path = AssetManager.ResolveFilePath("lorefulExpansion/sepulcherSpots.txt");
            }
            catch (Exception ex)
            {
                Log.LogWarning($"CollectorStats.LoadRegions: AssetManager not ready or path resolution failed: {ex.Message}");
                return;
            }

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Log.LogWarning("sepulcherSpots.txt not found.");
                return;
            }
            StreamReader reader = new(path);
            borisSpots = reader.ReadToEnd().Split('\r', '\n').ToList();
        }
    }
}
