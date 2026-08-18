using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public override List<string> BannedRegions()
        {
            List<string> bannedRegions = base.BannedRegions();
            List<string> extraBannedRegions = new() { "AUSS", "AUHZ", "AUND" };
            return bannedRegions.Union(extraBannedRegions).ToList();
        }

        public override void Tick()
        {
            base.Tick();
            agression--;
        }
    }
}
