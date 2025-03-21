using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using BepInEx;
using Steamworks.ServerList;

namespace JellyJamTinyREPO
{
    [HarmonyPatch(typeof(LevelGenerator))]
    internal static class ToiletTraversal
    {
        public static List<GameObject> toilets = new List<GameObject>();


        [HarmonyPatch("PlayerSpawn")]
        [HarmonyPostfix]
        private static void StartPatch()
        {
            // Empty the Current list
            toilets.Clear();

            JellyJamTinyREPO.Logger.LogInfo("Starting Toilet Check.");

            // Get all the toilets in the lobby
            var arr = GameObject.FindObjectsOfType<ToiletFun>();

            // Append them all to the list
            foreach (ToiletFun item in arr)
            {
                toilets.Add(item.gameObject);
                JellyJamTinyREPO.Logger.LogInfo("Found a Toilet.");
            }

            JellyJamTinyREPO.Logger.LogInfo($"Done Toilet Check, Total: {toilets.Count}");
        }
    }
    //PlayerAvatar.instance.PlayerDeath(0); this kills the player
    [HarmonyPatch(typeof(ToiletFun))]
    internal static class ToiletDetector
    {
        [HarmonyPatch("ExplosionRPC")]
        [HarmonyPostfix]
        private static void EndFlush(ToiletFun __instance)
        {

            JellyJamTinyREPO.Logger.LogInfo("Toilet Flushed.");

            if (Vector3.Distance(__instance.gameObject.transform.position, PlayerController.instance.transform.position) <= 1)
            {
                PlayerAvatar player = PlayerController.instance.playerAvatar.GetComponent<PlayerAvatar>();

                // Teleport the player to a random 
                
                int toiletID = UnityEngine.Random.Range(0, ToiletTraversal.toilets.Count);
                JellyJamTinyREPO.Logger.LogInfo("Going to Toilet ID: " + toiletID);
                GameObject _pos = ToiletTraversal.toilets[toiletID];


                player.tumble.physGrabObject.Teleport(_pos.transform.position + new Vector3(0, 1, 0), player.transform.rotation);
                JellyJamTinyREPO.Logger.LogInfo("Trying Teleport");
            }
        }
    }
}
