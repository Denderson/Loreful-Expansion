using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loremiscExpansion.Conduit
{
    public static class ConduitHooks
    {
        public static void ApplyHooks()
        {
            On.Player.Update += Player_Update;
        }

        public static bool IsConduit(this Player self)
        {
            return self != null && self.SlugCatClass == Enums.protagName;
        }

        public static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            if (self.IsConduit())
            {
                self.airInLungs = 1f;
            }
        }
    }
}
