using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loremiscExpansion.NPCs.Boris
{
    public class BorisState : NPCState
    {
        public int residue = 0;
        public int cyclesSinceLastResidue = 0;
        public int questProgression = 0;
        public override List<string> BannedRegions()
        {
            List<string> bannedRegions = base.BannedRegions();
            List<string> extraBannedRegions = new() { "AUND" };
            if (!string.IsNullOrEmpty(playerCurrentRegion)) extraBannedRegions.Add(playerCurrentRegion);
            return bannedRegions.Union(extraBannedRegions).ToList();
        }

        public override void Tick()
        {
            base.Tick();
        }
    }
}
