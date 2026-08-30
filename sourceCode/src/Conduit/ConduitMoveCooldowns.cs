using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static loremiscExpansion.Plugin;
using static loremiscExpansion.Conduit.ConduitHooks;

namespace loremiscExpansion.Conduit
{
    public class ConduitMoveCooldowns
    {
        private readonly Dictionary<ConduitMove, int> cooldowns = [];

        public void IncreaseCooldown(ConduitMove move)
        {
            cooldowns.TryGetValue(move, out int current);
            cooldowns[move] = Math.Min(current + cooldownGain, maxCooldown);

            foreach (var key in new List<ConduitMove>(cooldowns.Keys))
            {
                if (key == move) continue;
                cooldowns[key] = Math.Max(0, cooldowns[key] - otherMoveCooldownLoss);
            }
        }

        public float GetCooldownExhaustion(ConduitMove move)
        {
            cooldowns.TryGetValue(move, out int cooldown);
            return 1f - ((float)cooldown / (float)maxCooldown);
        }

        public void Tick()
        {
            if (cooldowns.Count == 0) return;
            foreach (var move in new List<ConduitMove>(cooldowns.Keys)) if (cooldowns[move] > 0) cooldowns[move]--;
        }
    }

    public enum ConduitMove
    {
        CricketJump,
        DodgeRoll,
        GroundBounce,
        WallBounce,
        CreatureBounce
    }
}
