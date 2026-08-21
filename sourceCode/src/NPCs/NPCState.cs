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
        public static List<string> listOfRegions = ["AUSS", "AUBT", "AUTS", "AUFH", "DZ1", "AUFD", "AUND", "AUHZ", "AUSG", "AUTT", "AUFN"];
        public static string playerCurrentRegion = string.Empty;
        private static readonly Random random = new();

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
            encounters = 0;
            cyclesSinceLastEncounter = 0;
            wanderingScore = 3;
            visitedRegions = [];
            nextRegion = string.Empty;
            currentRegion = string.Empty;
        }

        /*public NPCState(string context)
        {
            string[] actualContext = context?.Split(';');
            if (actualContext != null && actualContext.Length > 2)
            {
                if (!int.TryParse(actualContext[0], out wanderingScore))
                {
                    Log.LogMessage("Wandering score not found!");
                    wanderingScore = 3;
                }
                nextRegion = actualContext[1];
                visitedRegions = actualContext[2].Split(',').ToList();
                currentRegion = visitedRegions.Count > 0 ? visitedRegions[visitedRegions.Count - 1] : listOfRegions[0];
            }
            else
            {
                currentRegion = listOfRegions[0];
                nextRegion = string.Empty;
                visitedRegions = [];
            }
        }*/

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
            SetWanderingScore(currentRegion);
            ChooseNextRegion();
        }

        public void ChooseNextRegion()
        {
            List<string> connectedRegions = RegionConnections.GetConnections(currentRegion);
            if (connectedRegions.Count == 0)
            {
                Log.LogMessage($"No known connections for region: {currentRegion}");
                nextRegion = currentRegion;
                return;
            }

            List<string> bannedRegions = BannedRegions();
            List<string> validOptions = connectedRegions.Where(r => !bannedRegions.Contains(r)).ToList();

            if (validOptions.Count == 0)
            {
                visitedRegions.Clear();
                validOptions = connectedRegions;
            }

            nextRegion = validOptions[random.Next(validOptions.Count)];
        }

        public virtual void SetWanderingScore(string region)
        {
            wanderingScore = 3;
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
            ApostleState.LoadApostleSpots();
            CollectorState.LoadCollectorSpots();
            BorisState.LoadBorisSpots();
        }
    }

    public static class RegionConnections
    {
        public static readonly string[] Regions = ["AUBT", "AUFD", "AUFN", "AUFP", "AUHD", "AULB", "AUND", "AUSS", "AUSG", "AUTS", "AUTT", "AUWE", "DZ1"];

        private static readonly Dictionary<string, int> indexLookup = Regions.Select((name, i) => (name, i)).ToDictionary(x => x.name, x => x.i);

        public static readonly bool[,] Matrix = BuildMatrix();

        private static bool[,] BuildMatrix()
        {
            bool[,] m = new bool[Regions.Length, Regions.Length];

            void Connect(string a, string b)
            {
                int ia = indexLookup[a];
                int ib = indexLookup[b];
                m[ia, ib] = true;
                m[ib, ia] = true;
            }

            Connect("DZ1", "AUTT");
            Connect("DZ1", "AULB");
            Connect("AUTT", "AUFD");
            Connect("AUTT", "AUFN");
            Connect("AUSG", "AULB");
            Connect("AUSG", "AUBT");
            Connect("AUBT", "AUFD");
            Connect("AUBT", "AUWE");
            Connect("AUFN", "AUSS");
            Connect("AUSS", "AUWE");
            Connect("AUWE", "AUTS");
            Connect("AUFD", "AUTS");
            Connect("AULB", "AUFP");
            Connect("AUFP", "AUHD");
            Connect("AUFP", "AUTS");
            Connect("AUFN", "AUND");

            return m;
        }

        public static bool AreConnected(string a, string b)
        {
            if (!indexLookup.TryGetValue(a, out int ia) || !indexLookup.TryGetValue(b, out int ib)) return false;
            return Matrix[ia, ib];
        }

        public static List<string> GetConnections(string region)
        {
            List<string> result = new();
            if (!indexLookup.TryGetValue(region, out int idx)) return result;
            for (int i = 0; i < Regions.Length; i++) if (Matrix[idx, i]) result.Add(Regions[i]);
            return result;
        }
    }
}
