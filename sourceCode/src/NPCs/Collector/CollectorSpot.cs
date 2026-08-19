using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RWCustom;
using UnityEngine;
using static Pom.Pom;

namespace loremiscExpansion.NPCs.Collector
{
    namespace lsfUtils.DevtoolsObjects.LocalGravity
    {
        public class CollectorSpot : UpdatableAndDeletable
        {
            public CollectorSpotData data;
            Vector2 pos;

            public CollectorSpot(PlacedObject placedObject, Room room)
            {
                CollectorSpotData maybedata = placedObject.data as CollectorSpotData;
                data = maybedata ?? throw new ArgumentException($"{nameof(PlacedObject)} was null or didn't contain a {nameof(CollectorSpotData)} instance");
                pos = placedObject.pos;
                this.room = room;
            }

            public bool InRange(Vector2 pos)
            {
                return Custom.DistLess(pos, this.pos, data.radius.magnitude);
            }
        }

        public class CollectorSpotData(PlacedObject po) : ManagedData(po, [])
        {
            [Vector2Field("Radius", defX: 80f, defY: 0f, Vector2Field.VectorReprType.circle)]
            public Vector2 radius;
        }
    }
}
