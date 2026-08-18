using loremiscExpansion.NPCs.Apostle;
using loremiscExpansion.NPCs.Boris;
using loremiscExpansion.NPCs.Collector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static loremiscExpansion.Plugin;
using static loremiscExpansion.SaveFileCode;

namespace loremiscExpansion.NPCs
{
    public class NPCState
    {
        public static List<string> listOfRegions = new() { "AUSS", "AUBT", "AUTS", "AUFH", "DZ1", "AUFD", "AUND", "AUHZ", "AUSG", "AUTT", "AUFN" };
        public static string playerCurrentRegion = string.Empty;

        public bool dead;

        public int encounters = 0;
        public int cyclesSinceLastEncounter = 0;

        public int wanderingScore;
        public List<string> visitedRegions;
        public string nextRegion;
        public string currentRegion;

        public NPCState() 
        {
            dead = false;
            wanderingScore = 3;
            nextRegion = string.Empty;
            visitedRegions = new List<string>();
        }

        public NPCState(string context)
        {
            string[] actualContext = context?.Split(';');
            if (actualContext != null && actualContext.Length > 2)
            {
                if (!int.TryParse(actualContext[0], out wanderingScore))
                {
                    Log.LogMessage("Wandering score not found!");
                    wanderingScore = 3;
                }
                else
                {
                    Log.LogMessage("Wandering score found!");
                    Log.LogMessage(wanderingScore);
                }
                nextRegion = actualContext[1];
                visitedRegions = actualContext[2].Split(',').ToList();
            }
        }

        public virtual void Tick()
        {
            cyclesSinceLastEncounter++;
            if (wanderingScore > 0)
            {
                wanderingScore--;
                return;
            }
            if (nextRegion == string.Empty) ChooseNextRegion();
            visitedRegions.Add(currentRegion);
            currentRegion = nextRegion;
            ChooseNextRegion();
        }

        public void ChooseNextRegion()
        {
            List<string> possibleRegions = listOfRegions;
            List<string> bannedRegions = BannedRegions();
            if (possibleRegions.All(x => bannedRegions.Contains(x))) visitedRegions.Clear();
            foreach (string region in possibleRegions) if (bannedRegions.Contains(region)) possibleRegions.Remove(region);
            Random random = new();
            int randomIndex = random.Next(listOfRegions.Count);
            nextRegion = listOfRegions[randomIndex];

            // get region complexity and set wanderingScore to that value here
            wanderingScore = 3; // placeholder for now
        }

        public virtual List<string> BannedRegions()
        {
            return visitedRegions;
        }
    }
    public static class NPCStateHooks
    {
        public static void ApplyHooks()
        {
            On.SaveState.RainCycleTick += SaveState_RainCycleTick;
            On.RainWorld.Start += RainWorld_Start;
        }

        public static void SaveState_RainCycleTick(On.SaveState.orig_RainCycleTick orig, SaveState self, RainWorldGame game, bool depleteSwarmRoom)
        {
            orig(self, game, depleteSwarmRoom);
            if (self == null)
            {
                Log.LogMessage("Savestate is null!");
                return;
            }

            ApostleState apostleState = self.GetApostleState();
            if (apostleState == null)
            {
                Log.LogMessage("Apostle state is null!");
                apostleState = new ApostleState();
            }
            apostleState.Tick();
            self.SetApostleState(apostleState);

            CollectorState collectorState = self.GetCollectorState();
            if (collectorState == null)
            {
                Log.LogMessage("Collector state is null!");
                collectorState = new CollectorState();
            }
            collectorState.Tick();
            self.SetCollectorState(collectorState);

            BorisState borisState = self.GetBorisState();
            if (borisState == null)
            {
                Log.LogMessage("Boris state is null!");
                borisState = new BorisState();
            }
            borisState.Tick();
            self.SetBorisState(borisState);
        }

        public static void RainWorld_Start(On.RainWorld.orig_Start orig, RainWorld self)
        {
            orig(self);
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
        }
    }
}
