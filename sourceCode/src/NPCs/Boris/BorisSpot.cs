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

namespace loremiscExpansion.NPCs.Boris
{
    namespace lsfUtils.DevtoolsObjects.LocalGravity
    {
        public class BorisSpot : UpdatableAndDeletable
        {
            public BorisSpotData data;
            Vector2 pos;

            public BorisSpot(PlacedObject placedObject, Room room)
            {
                BorisSpotData maybedata = placedObject.data as BorisSpotData;
                data = maybedata ?? throw new ArgumentException($"{nameof(PlacedObject)} was null or didn't contain a {nameof(BorisSpotData)} instance");
                pos = placedObject.pos;
                this.room = room;
            }

            public bool InRange(Vector2 pos)
            {
                return Custom.DistLess(pos, this.pos, data.radius.magnitude);
            }
        }

        public class BorisSpotData(PlacedObject po) : ManagedData(po, [])
        {
            [Vector2Field("Radius", defX: 80f, defY: 0f, Vector2Field.VectorReprType.circle)]
            public Vector2 radius;
        }
    }
}
