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

        public CollectorStats(AbstractCreature Host)
        {
            ID = Host.ID;
        }
    }
}
