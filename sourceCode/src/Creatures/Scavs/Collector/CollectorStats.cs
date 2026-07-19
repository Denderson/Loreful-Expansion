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
        public List<string> VisitedRegions = new List<string>();
        public string NextRegion = null;
        int PersonalReputation = 0;

        public CollectorStats(AbstractCreature Host)
        {
            ID = Host.ID;
        }

        static public void LoadRegions()
        {

        }
    }
}
