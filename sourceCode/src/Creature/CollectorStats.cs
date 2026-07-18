using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loremiscExpansion.Creature
{
    public class CollectorStats
    {
        EntityID ID;
        int WanderingScore = 2;
        string VisitedRegions;
        string NextRegion = null;
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
        }
    }
}
