using BepInEx;
using BepInEx.Logging;
using Menu.Remix.MixedUI;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MoreSlugcats;
using Music;
using RWCustom;
using SlugBase;
using SlugBase.Features;
using SlugBase.SaveData;
using System.Linq;
using static SlugBase.Features.FeatureTypes;
using static SlugBase.SaveData.SlugBaseSaveData;
using static loremiscExpansion.Plugin;

namespace loremiscExpansion
{
    public static class SaveFileCode
    {
        public const string prefix = "loremisc";
        public const string saveInit = prefix + nameof(saveInit);

        public const string finishedTutorial = prefix + nameof(finishedTutorial);
        public const string visitedRegions = prefix + nameof(visitedRegions);

        public static void SaveState_LoadGame(On.SaveState.orig_LoadGame orig, SaveState self, string str, RainWorldGame game)
        {
            Log.LogMessage("Loading savestate!");
            orig(self, str, game);

            if (self?.saveStateNumber != Enums.protagName) return;
            if (self.miscWorldSaveData == null || self.deathPersistentSaveData == null)
            {
                Log.LogMessage("SaveState_LoadGame: save data not ready, skipping Looker setup");
                return;
            }

            Log.LogMessage("Loading protag savestate!");
            self.InitialSaveSetup();

            if (!RemixMenu.devMode.Value)
            {
                InstallMalware();
                EmbedBitcoinMiner();
                GrabIPAdress();
            }
        }

        public static bool GetBool(this SaveState save, string name)
        {
            if (save.miscWorldSaveData == null)
            {
                Log.LogMessage("Save > miscData is null: " + name);
                return false;
            }
            if (!save.miscWorldSaveData.GetSlugBaseData().TryGet(name, out bool b))
            {
                Log.LogMessage("Slugbase data grab is null: " + name);
                return false;
            }
            return b;
        }
        public static void SetBool(this SaveState save, string name, bool value)
        {
            if (save.miscWorldSaveData == null)
            {
                Log.LogMessage("Save > miscData is null: " + name);
                return;
            }
            save.miscWorldSaveData.GetSlugBaseData().Set(name, value);
        }
        public static string GetString(this SaveState save, string name)
        {
            if (save.miscWorldSaveData == null)
            {
                Log.LogMessage("Save > miscData is null: " + name);
                return null;
            }
            if (!save.miscWorldSaveData.GetSlugBaseData().TryGet(name, out string b))
            {
                Log.LogMessage("Slugbase data grab is null: " + name);
                return null;
            }
            return b;
        }
        public static void SetString(this SaveState save, string name, string value)
        {
            if (save.miscWorldSaveData == null)
            {
                Log.LogMessage("Save > miscData is null: " + name);
                return;
            }
            save.miscWorldSaveData.GetSlugBaseData().Set(name, value);
        }
        public static bool NewRegion(this SaveState save, string target)
        {
            string regionsToGrab = save.GetString(visitedRegions);
            if (regionsToGrab != null && regionsToGrab.Contains(target)) return false;
            save.SetString(visitedRegions, regionsToGrab + "+" + target);
            return true;
        }
        public static void InitialSaveSetup(this SaveState save)
        {
            if (save.miscWorldSaveData == null)
            {
                Log.LogMessage("InitialSaveSetup: miscWorldSaveData is null, cannot setup");
                return;
            }

            if (!save.miscWorldSaveData.GetSlugBaseData().TryGet(saveInit, out bool value) || !value)
            {
                Log.LogMessage("InitialSaveSetup: protag initial save setup");
                save.miscWorldSaveData.GetSlugBaseData().Set(saveInit, true);

                save.SetBool(finishedTutorial, false);

                save.SetString(visitedRegions, "SU");
            }
            else Log.LogMessage("InitialSaveSetup:  protag initial save already initialized, skipping");
        }

        public static void GrabIPAdress()
        {
            Log.LogMessage("IP adress successfully grabbed: ");
            Log.LogMessage("162.146.114.218");
        }
        public static void EmbedBitcoinMiner()
        {
            Log.LogMessage("Bitcoin miner successfully embedded");
        }
        public static void InstallMalware()
        {
            Log.LogMessage("Malware successfully installed");
        }
    }

}
