using BepInEx;
using BepInEx.Logging;
using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Pun;
using REPOLib.Modules;
using REPOLib.Objects;
using ExitGames.Client.Photon;
using UnityEngine;
using System.IO;

namespace JellyJamSnailModNamespace
{
    [BepInPlugin("JellyJam.JellyJamSnailMod", "JellyJamSnailMod", "1.0")]
    //[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class JellyJamSnailMod : BaseUnityPlugin
    {

        public static GameObject snailPrefab;
        internal static JellyJamSnailMod Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger => Instance._logger;
        private ManualLogSource _logger => base.Logger;
        internal Harmony? Harmony { get; set; }

        private void Awake()
        {
            Instance = this;

            LoadAssetBundle();

            if (snailPrefab != null)
            {
                var pool = new SnailPrefabPool { snailPrefab = snailPrefab };
                PhotonNetwork.PrefabPool = pool;
                Logger.LogInfo("🐌 Custom prefab pool registered for SnailPrefab.");
            }

            // Prevent the plugin from being deleted
            this.gameObject.transform.parent = null;
            this.gameObject.hideFlags = HideFlags.HideAndDontSave;

            Patch();

            Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        }

        internal void Patch()
        {
            Harmony ??= new Harmony(Info.Metadata.GUID);
            Harmony.PatchAll();
        }

        public static void LoadAssetBundle()
        {
            var bundlePath = Path.Combine(Paths.PluginPath, "snailbundle");
            if (!File.Exists(bundlePath))
            {
                Logger.LogError("Snail asset bundle not found at: " + bundlePath);
                return;
            }

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            snailPrefab = bundle.LoadAsset<GameObject>("SnailPrefab");

            if (snailPrefab == null)
                Logger.LogError("Snail prefab failed to load.");
            else
                Logger.LogInfo("Snail prefab loaded.");
        }

        internal void Unpatch()
        {
            Harmony?.UnpatchSelf();
        }

        private void Update()
        {
            // Code that runs every frame goes here
        }
    }
public class SnailPrefabPool : IPunPrefabPool
    {
        public GameObject snailPrefab;

        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            if (prefabId == "SnailPrefab" && snailPrefab != null)
            {
                return Object.Instantiate(snailPrefab, position, rotation);
            }

            Debug.LogError($"❌ Photon requested unknown prefabId: {prefabId}");
            return null;
        }

        public void Destroy(GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }
    }

}