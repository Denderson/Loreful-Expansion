using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace loremiscExpansion
{
    public class RemixMenu : OptionInterface
    {
        public static readonly Color unfinishedColor = new(0.85f, 0.35f, 0.4f);
        private OpCheckBox TCheckBox (Configurable<bool> config, int x, int y, bool isUnfinished = false)
        {
            if (config == null)
            {
                Plugin.Log.LogError("Error with " + x + " " + y);
                return null;
            }
            OpCheckBox checkBox = new(config, x * 160, 503 - y * 80) { description = config.info.description };
            if (isUnfinished) checkBox.colorEdge = unfinishedColor;
            return checkBox;
        }
        private OpCheckBox TCheckBox(Configurable<bool> config, int x, int y, Color checkboxColor)
        {
            if (config == null)
            {
                Plugin.Log.LogError("Error with " + x + " " + y);
                return null;
            }
            OpCheckBox checkBox = new(config, x * 160, 503 - y * 80) { description = config.info.description };
            checkBox.colorEdge = checkboxColor;
            return checkBox;
        }
        private static OpFloatSlider TFloatSlider(Configurable<float> config, int y, float decideMax, bool isUnfinished = false)
        {
            if (config == null)
            {
                Plugin.Log.LogError("Error with " + y);
                return null;
            }
            OpFloatSlider slider = new(config, new Vector2(0, 460 - y * 80), 100) { max = decideMax, description = config.info.description };
            if (isUnfinished) slider.colorEdge = unfinishedColor;
            if (isUnfinished) slider.colorLine = unfinishedColor;
            return slider;
        }
        private static OpFloatSlider TFloatSlider(Configurable<float> config, int y, float decideMax, Color sliderColor)
        {
            if (config == null)
            {
                Plugin.Log.LogError("Error with " + y);
                return null;
            }
            OpFloatSlider slider = new(config, new Vector2(0, 460 - y * 80), 100) { max = decideMax, description = config.info.description };
            slider.colorEdge = sliderColor;
            slider.colorLine = sliderColor;
            return slider;
        }
        private static OpLabel TLabel(string text, float x, float y, bool isUnfinished = false)
        {
            OpLabel label = new(x * 160 + 30, 500 - y * 80, text);
            if (isUnfinished) label.color = new Color(0.85f, 0.35f, 0.4f);
            return label;
        }
        private static OpLabel TLabel(string text, float x, float y, Color labelColor)
        {
            OpLabel label = new(x * 160 + 30, 500 - y * 80, text);
            label.color = labelColor;
            return label;
        }

        private static int _creditsY = -1;
        private static OpLabel CreditsLabel(string text, float x, int increase = 1)
        {
            if (x == 0) _creditsY += increase;
            return new OpLabel(200 * x, 500 - _creditsY * 25, text);
        }
        private static OpLabel RegionLabel(string text, float y, Color labelColor)
        {
            OpLabel label = new(410, 480 - y * 80, text, true);
            label.color = labelColor;
            return label;
        }
        private static OpLabel SliderLabel(string text, int y, bool isUnfinished = false)
        {
            OpLabel label = new(110, 460 - y * 80, text) { description = text };
            if (isUnfinished) label.color = new Color(0.85f, 0.35f, 0.4f);
            return label;
        }
        private static OpLabel SliderLabel(string text, int y, Color labelColor)
        {
            OpLabel label = new(110, 460 - y * 80, text) { description = text };
            label.color = labelColor;
            return label;
        }

        public RemixMenu(Plugin plugin)
        {
            noTutorials = config.Bind("loremisc_noTutorials", false, new ConfigurableInfo("Disables tutorial popups"));

            unlockAll = config.Bind("loremisc_unlockAll", false, new ConfigurableInfo("Unlocks every item and creature for arena"));
            devMode = config.Bind("loremisc_devMode", false, new ConfigurableInfo("Idk"));
        }

        public override void Initialize()
        {

            base.Initialize();

            Tabs = new[] { new OpTab(this, "General"), new OpTab(this, "Debug") { colorButton = unfinishedColor } };

            // General tab
            UIelement[] UIArrayElements = new UIelement[]
            {
                new OpLabel(0, 550, "General options", true), new OpLabel(160, 550, "(red means not implemeted yet)", true){color = unfinishedColor},

                TLabel("No tutorials", 0, 0),
                TCheckBox(noTutorials, 0, 0),
            };
            Tabs[0].AddItems(UIArrayElements);

            // Debug tab
            UIArrayElements = new UIelement[]
            {
                new OpLabel(0, 550, "Debug", true){color = unfinishedColor}, new OpLabel(160, 550, "(for debugging purposes obviously)", true){color = unfinishedColor},

                TLabel("Dev mode", 0, 0, unfinishedColor),
                TCheckBox(devMode, 0, 0, unfinishedColor),

                TLabel("Difficulty chosen", 0, 1, unfinishedColor),
                TCheckBox(unlockAll, 0, 1, unfinishedColor)
            };
            Tabs[1].AddItems(UIArrayElements);
        }

        public static Configurable<bool> 
            noTutorials,
            unlockAll, devMode;
    }
}
