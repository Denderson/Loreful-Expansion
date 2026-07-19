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
            protagName = new(nameof(protagName), true);
            protagTimeline = new(nameof(protagTimeline), true);
            ScavCollector = new(nameof(ScavCollector), true);
            ScavCollectorUnlock = new(nameof(ScavCollectorUnlock), true);
        }

        public static void UnregisterValues()
        {
            protagName?.Unregister();
            protagTimeline?.Unregister();
            ScavCollector?.Unregister();
            ScavCollectorUnlock?.Unregister();
        }

        public static SlugcatStats.Name protagName; 
        public static SlugcatStats.Name protagTimeline;
        public static CreatureTemplate.Type ScavCollector;
        public static MultiplayerUnlocks.SandboxUnlockID ScavCollectorUnlock;
    }
}
