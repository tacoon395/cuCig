using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using UnityEngine;
using CUCoreLib.ContentReload;
using CUCoreLib.Data;
using CUCoreLib.Helpers;
using CUCoreLib.Registries;
using CUCoreLib.Saving;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ModNamespace
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("net.cucorelib", BepInDependency.DependencyFlags.HardDependency)] 
    public class Plugin : BaseUnityPlugin
    {
        public const string ModGUID = "com.lesothedoes.cuCig";
        public const string ModName = "Adds a soothing cig"; // To change .dll name, change the name in vars.targets
        public const string ModVersion = "1.0.1";

        internal static new ManualLogSource Logger;
        private readonly Harmony _harmony = new(ModGUID);
        public static Plugin Instance { get; private set; } = null!;

        public void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            _harmony.PatchAll();
            Logger.LogInfo($"Plugin {ModName} is loaded!");

            DoStuff();
            ContentReloadManager.EnableHotReload(ModGUID); // Required for hot reload, put any registry registrations in the RegisterReloadable method below
            RegisterReloadable();
        }

        private void RegisterReloadable()
        {
    
        }
        
        /// <summary>
        /// 
        /// </summary>
//        private void LoadModAssets()
//        {
//            // Embedded image assets
//            Sprite cucigSprite = AssetLoader.LoadEmbeddedSprite("images.CUNKcigarette.png");

            // Embedded animations, from many images!
//            AssetLoader.LoadFrameAnimationFromEmbeddedResources(
//                "cucig.smoke",
//                new[] { "images.CUNKcigarette.png", "images.CUNKcigarette1.png", "images.CUNKcigarette2.png" },
//                pixelsPerUnit: 8f,
//                framesPerSecond: 6f);
//        }

        private void DoStuff()
        {
            Sprite cucigSprite = AssetLoader.LoadEmbeddedSprite("images.CUNKcigarette.png");
            AssetLoader.LoadFrameAnimationFromEmbeddedResources(
                            "cucig.smoke",
                            new[] { "images.CUNKcigarette.png", "images.CUNKcigarette1.png", "images.CUNKcigarette2.png" },
                            pixelsPerUnit: 8f,
                            framesPerSecond: 6f);

            ItemRegistry.Register("cucig", new CustomItemInfo
            {
                fullName = "Cigarette",
                description = "An unknown brand cigarette.",
                category = "drug,food",
                slotRotation = 0f,
                weight = 0.1f,
                value = 15,
                usable = true,
                usableOnLimb = false,
                destroyAtZeroCondition = true,
                scaleWeightWithCondition = true,
                decayMinutes = 280f,
                tags = "cangetwet,medicine",
                rec = new Recognition(2),
                SpriteScale = 1f,
                SpriteScaleDimensions = (14f, 14f, true),
                SpawnFrequency = 1,
                DropPool = DropPool.All | DropPool.AllTraders,
                useAction = (body, item) =>
                {
                    body.thirst += 0.625f; ;
                    body.hunger -= 1.875f;
                    body.limbs[0].pain += 2f;
                    body.happiness += 15f;
                    body.bloodPressure += 20f;
                    body.heartRate += 20f;
                    item.condition -= 0.25f;
                    //body.ignoreDepression = true;
                    Sound.Play("eatCrunch", body.transform.position);
                }
            }, cucigSprite, 1);
        }
    }
}
