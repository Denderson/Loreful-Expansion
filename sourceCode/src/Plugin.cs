using BepInEx;
using BepInEx.Logging;
using LizardCosmetics;
using loremiscExpansion.NPCs.Apostle.lsfUtils.DevtoolsObjects.LocalGravity;
using loremiscExpansion.NPCs.Boris.lsfUtils.DevtoolsObjects.LocalGravity;
using loremiscExpansion.NPCs.Collector;
using loremiscExpansion.NPCs.Collector.lsfUtils.DevtoolsObjects.LocalGravity;
using Menu;
using Menu.Remix.MixedUI;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RWCustom;
using SlugBase.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Permissions;
using UnityEngine;
using static Pom.Pom;

namespace loremiscExpansion
{
    [BepInDependency("slime-cubed.slugbase")]
    [BepInPlugin("loremiscExpansion", "loremiscExpansion", "0.1.00")]
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
                Log = Logger;

                On.SaveState.LoadGame += SaveFileCode.SaveState_LoadGame;

                //NPCs.NPCStateHooks.ApplyHooks();
                Conduit.ConduitHooks.ApplyHooks();

                RegisterManagedObject<ApostleSpot, ApostleSpotData, ManagedRepresentation>("ApostleSpot", "Aurelia");
                RegisterManagedObject<CollectorSpot, CollectorSpotData, ManagedRepresentation>("CollectorSpot", "Aurelia");
                RegisterManagedObject<BorisSpot, BorisSpotData, ManagedRepresentation>("SepulcherSpot", "Aurelia");

                Logger.LogMessage("loremisc hooks success!");
            }
            catch (Exception e)
            {
                Logger.LogMessage("loremisc hooks fail!!!");
                Logger.LogError(e);
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