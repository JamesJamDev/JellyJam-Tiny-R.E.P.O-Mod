using BepInEx;
using BepInEx.Logging;
using ExitGames.Client.Photon;
using HarmonyLib;
using REPOLib.Modules;
using UnityEngine;

namespace JellyJamTinyREPO
{
    [BepInPlugin("JellyJam.JellyJamTinyREPO", "JellyJamTinyREPO", "1.0")]
    [BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class JellyJamTinyREPO : BaseUnityPlugin
    {
        internal static JellyJamTinyREPO Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger => Instance._logger;
        private ManualLogSource _logger => base.Logger;
        internal Harmony? Harmony { get; set; }

        public static NetworkedEvent ShrinkEvent;

        private void Awake()
        {
            Instance = this;

            ShrinkEvent = new NetworkedEvent("Shrink Players", ShrinkPlayers);



            // Prevent the plugin from being deleted
            this.gameObject.transform.parent = null;
            this.gameObject.hideFlags = HideFlags.HideAndDontSave;

            Patch();

            Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        }

        private static void ShrinkPlayers(EventData eventData)
        {
            if (eventData.CustomData is not object[] data || data.Length == 0)
                return;

            int playerId = (int)data[0]; // Get the player ID (or -1 for all)

            foreach (var player in GameObject.FindObjectsOfType<PlayerController>()) // Replace with your player class
            {
                // Use an identifier to check which player should be shrunk
                if (player.GetComponent<Photon.Realtime.Player>()?.ActorNumber == playerId || playerId == -1)
                {
                    player.transform.localScale = new Vector3(PlayerControllerPatch.playerSize, PlayerControllerPatch.playerSize, PlayerControllerPatch.playerSize); // Shrink player
                    player.playerAvatarPrefab.transform.localScale = new Vector3(PlayerControllerPatch.playerSize + 0.05f, PlayerControllerPatch.playerSize + 0.05f, PlayerControllerPatch.playerSize + 0.05f);
                }
            }

            JellyJamTinyREPO.Logger.LogInfo("Players Shrunk");
        }



        internal void Patch()
        {
            Harmony ??= new Harmony(Info.Metadata.GUID);
            Harmony.PatchAll();
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
}