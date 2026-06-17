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
        }

        public static void UnregisterValues()
        {
            protagName?.Unregister();
            protagTimeline?.Unregister();
        }

        public static SlugcatStats.Name protagName; 
        public static SlugcatStats.Name protagTimeline;
    }
}
