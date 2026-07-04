using BepInEx;
using BepInEx.Logging;
using Menu;
using SlugBase.Features;
using System;
using UnityEngine;
using LizardCosmetics;
using Menu.Remix.MixedUI;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RWCustom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Permissions;

namespace loremiscExpansion
{
    [BepInDependency("slime-cubed.slugbase")]
    [BepInPlugin("loremiscExpansion", "loremiscExpansion", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log { get; private set; }

        public static RemixMenu remixMenu;
        public bool remixInit;
        public bool isInit;

        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            On.RainWorld.OnModsInit += LoadRemixMenu;

            try
            {
                On.SaveState.LoadGame += SaveFileCode.SaveState_LoadGame;

                On.Player.Update += Player_Update;

                Logger.LogMessage("loremisc hooks success!");
            }
            catch (Exception e)
            {
                Logger.LogMessage("loremisc hooks fail!!!");
                Logger.LogError(e);
            }
        }

        public static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            if (self?.SlugCatClass == Enums.protagName)
            {
                self.airInLungs = 1f;
            }
        }

        private void LoadRemixMenu(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            if (remixInit)
            {
                return;
            }
            remixInit = true;
            remixMenu = new RemixMenu(this);
            try
            {
                MachineConnector.SetRegisteredOI("loremiscExpansion", remixMenu);
            }
            catch (Exception ex)
            {
                Debug.Log($"Loremisc: Hook_OnModsInit options failed init error {remixMenu}{ex}");
                Logger.LogError(ex);
            }
            Enums.RegisterValues();
        }
        private void LoadResources(RainWorld rainWorld)
        {
        }
    }
}