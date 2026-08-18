using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static loremiscExpansion.Plugin;
using static loremiscExpansion.SaveFileCode;

namespace loremiscExpansion.NPCs
{
    public class NPCMovement
    {
        public static List<string> listOfRegions = new(); // Add the region list here

        public int wanderingScore;
        public List<string> visitedRegions;
        public string nextRegion;

        /*public override string ToString()
        {
            string text = string.Empty;
            text += wanderingScore.ToString();
            text += ";";
            text += nextRegion.ToString();
            text += ";";
            for (int i = 0; i < visitedRegions.Count; i++)
            {
                if (i != 0) text += ",";
                text += visitedRegions[i].ToString();
            }
            Log.LogMessage(text);
            return text;
        }*/

        public NPCMovement() 
        {
            wanderingScore = 3;
            nextRegion = string.Empty;
            visitedRegions = new List<string>();
        }

        public NPCMovement(string context)
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

        public void Tick()
        {
            if (wanderingScore > 0)
            {
                wanderingScore--;
                return;
            }
            if (nextRegion == string.Empty)
            {

            }
        }

        public void ChooseNextRegion()
        {
            List<string> possibleRegions = listOfRegions;
            List<string> bannedRegions = BannedRegions();
            foreach (string region in possibleRegions) if (bannedRegions.Contains(region)) possibleRegions.Remove(region);
            Random random = new();
            int randomIndex = random.Next(listOfRegions.Count);
            nextRegion = listOfRegions[randomIndex];
        }

        public virtual List<string> BannedRegions()
        {
            return visitedRegions;
        }
    }

    public class ApostleMovement : NPCMovement
    {
        public override List<string> BannedRegions()
        {
            List<string> bannedRegions = base.BannedRegions();
            // Add regions Apostle cannot move to here
            return bannedRegions;
        }
    }

    public class CollectorMovement : NPCMovement
    {
        public override List<string> BannedRegions()
        {
            List<string> bannedRegions = base.BannedRegions();
            // Add regions Collector cannot move to here
            return bannedRegions;
        }
    }

    public class BorisMovement : NPCMovement
    {
        public override List<string> BannedRegions()
        {
            List<string> bannedRegions = base.BannedRegions();
            // Add regions Boris cannot move to here
            return bannedRegions;
        }
    }

    public static class NPCMovementHooks
    {
        public static void ApplyHooks()
        {
            //On.SaveState.RainCycleTick += SaveState_RainCycleTick;

            // Will likely crash stuff for now, enable when finished
        }

        public static void SaveState_RainCycleTick(On.SaveState.orig_RainCycleTick orig, SaveState self, RainWorldGame game, bool depleteSwarmRoom)
        {
            orig(self, game, depleteSwarmRoom);
            List<NPCMovement> list = SaveFileCode.GetNPCMovements(self);
            foreach (NPCMovement movement in list)
            {
                movement.Tick();
            }
            SaveFileCode.SetNPCMovements(self, list);
        }
    }
}
