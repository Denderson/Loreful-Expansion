using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loremiscExpansion
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

        public void CreatureState_CycleTick(On.CreatureState.orig_CycleTick orig, CreatureState self)
        {
            //See comment in Plugin.cs for why this is not currently used
            throw new NotImplementedException();
        }
    }
}
