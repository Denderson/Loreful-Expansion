using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace loremiscExpansion
{
    public class Enums
    {
        public static void RegisterValues()
        {
            protag = new(nameof(protag), false);
            protagTimeline = new(nameof(protag), false);
            ScavCollector = new(nameof(ScavCollector), true);
            ScavCollectorUnlock = new(nameof(ScavCollectorUnlock), true);
        }

        public static void UnregisterValues()
        {
            protag?.Unregister();
            protagTimeline?.Unregister();
            ScavCollector?.Unregister();
            ScavCollectorUnlock?.Unregister();
        }

        public static SlugcatStats.Name protag; 
        public static SlugcatStats.Timeline protagTimeline;
        public static CreatureTemplate.Type ScavCollector;
        public static MultiplayerUnlocks.SandboxUnlockID ScavCollectorUnlock;
    }
}
