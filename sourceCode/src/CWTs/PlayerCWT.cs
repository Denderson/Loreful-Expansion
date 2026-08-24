using System.Runtime.CompilerServices;

namespace loremiscExpansion.CWTs
{
    public static class PlayerCWT
    {

        public static readonly ConditionalWeakTable<Player, DataClass> playerCWT = new();
        public static bool TryGetData(Player key, out DataClass data)
        {
            if (key != null)
            {
                data = playerCWT.GetOrCreateValue(key);
            }
            else data = null;

            return data != null;
        }
        public class DataClass
        {
            public bool rolling = false;
            public int rollAnimation = 0;
            public int rollDuration = 0;

            public int dodgeRollWindow = 0;
            public int dodgeRollDirection = 0;
            public int wallBounceWindow = 0;

            public int cricketJumpCooldown = 0;

            public int rollCooldown = 0;
            public int breathOrbs = 7;
            public int breathTimer = 0;
            public float sporePoison = 0;

            public int rollIframes = 0;

            public float camo = 0f;
            public UnityEngine.Color? camoColor = null;
        }
    }
}
