using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loremiscExpansion.Creatures.Scavs.Collector
{
        public class Collector : Scavenger
        {
            public CollectorStats myStats = null;

            public Collector(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
            {
                CollectorStats myStats = new CollectorStats(abstractCreature);
            }
            public override void InitiateGraphicsModule()
            {
                graphicsModule ??= new ScavengerGraphics(this);
                graphicsModule.Reset();
            }
            public override void LoseAllGrasps()
            {
                ReleaseGrasp(0);
            }

    }
}
