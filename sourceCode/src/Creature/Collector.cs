using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loremiscExpansion.Creature
{
        public class Collector : Scavenger
        {

            public Collector(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
            {

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
