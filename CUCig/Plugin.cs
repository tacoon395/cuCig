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
using Random = UnityEngine.Random;

namespace ModNamespace
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("net.cucorelib", BepInDependency.DependencyFlags.HardDependency)] 
    public class Plugin : BaseUnityPlugin
    {
        public const string ModGUID = "com.lesothedoes.cuCig";
        public const string ModName = "cuCig"; // To change .dll name, change the name in vars.targets
        public const string ModVersion = "1.0.3.1";

        internal static new ManualLogSource Logger;
        private readonly Harmony _harmony = new(ModGUID);
        public static Plugin Instance { get; private set; } = null!;

        private Sprite cucigSprite;
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

        private void LoadModAudio()
        {
            AudioClip cigPuff = AssetLoader.LoadEmbeddedAudio("audio.cigPuff.ogg");
        }
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
                weight = 0.03f,
                value = 10,
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
                    float num = UnityEngine.Random.Range(0f, 1f);
                    body.thirst += num + 0.625f;
                    body.hunger -= num +0.875f;
                    //stonefruit is 10f, used as reference for a full cigarette. we divide 10 by 4 to get 2.5f, which is the amount of pain increase for a full cigarette
                    // body.limbs[1] is the chest, body.limbs[0] is the head. We increase pain in chest and decrease pain in head to simulate the effect of smoking on the body.
                    body.limbs[1].pain += 10f * 0.25f;
                    body.limbs[0].pain -= 10f * 0.25f;
                    //happiness of 20f is a bit too much as it brings basic happy moodle
                    body.happiness += UnityEngine.Random.Range(80f, 100f) * 0.25f;
                    //real blood pressure increase is 10-20mmHg, but we multiply by 0.25 to make it fit condition going down by 0.25
                    body.bloodPressure += UnityEngine.Random.Range(10f, 20f) * 0.25f;
                    //real heartrate increase is 10-15bpm, but we multiply by 0.25 to make it fit condition going down by 0.25
                    body.heartRate += UnityEngine.Random.Range(10f, 15f) * 0.25f;
                    item.condition -= 0.25f;
                    body.respiratoryRate += 15f;
                    //lungs need to be lowered
                    body.stamina -= UnityEngine.Random.Range(15f, 25f);
                    body.energy += UnityEngine.Random.Range(10f, 20f);
                    // spO2 is apparently not affected by smoking per https://pmc.ncbi.nlm.nih.gov/articles/PMC3999162/
                    //body.bloodOxygen -= UnityEngine.Random.Range(1f, 2.875f) * 0.25f;
                    body.adrenaline += UnityEngine.Random.Range(1f, 15f) * 0.25f;
                    body.temporarySlowdown -= 5f;
                    body.dirtyness += UnityEngine.Random.Range(10f, 20f) * 0.25f;
                    //body.ignoreDepression = true;
                    CUCoreUtils.PlaySoundAt(
                        AssetLoader.GetCachedAudioClip("audio.cigPuff.ogg"),
                        volume: 0.33f,
                        delay: 0.1f,
                        position: (Vector2)transform.position,
                        pitch: 1f
                        );
                    //Sound.Play("eatCrunch", body.transform.position);
                }
            }, cucigSprite, 1);
        }
    }
}
