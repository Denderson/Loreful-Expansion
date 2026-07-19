using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loremiscExpansion.Creatures.Scavs.Collector
{
    public class CollectorStats
    {
        EntityID ID;
        public int WanderingScore = 2;
        public string VisitedRegions;
        public string NextRegion = null;
        int PersonalReputation = 0;

        public CollectorStats(AbstractCreature Host)
        {
            ID = Host.ID;
        }

        public void WanderToRegion ()
        {
           
        }
        static public void CreatureState_CycleTick(On.CreatureState.orig_CycleTick orig, CreatureState self)
        {
            orig(self);
            if (self.creature.creatureTemplate.type == Enums.ScavCollector)
            {           
                //WanderingScore--;
            }

        }
    }
}
