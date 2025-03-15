using HarmonyLib;
using System;
using UnityEngine;

namespace JellyJamTinyREPO
{
    [HarmonyPatch(typeof(PlayerController))]
    internal static class ExamplePlayerControllerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPatch(PlayerController __instance)
        {
            
        }



        [HarmonyPrefix, HarmonyPatch("Update")]
        private static void Update_Prefix(PlayerController __instance)
        {
            // Code to execute for each PlayerController *before* Update() is called.
            JellyJamTinyREPO.Logger.LogDebug($"{__instance} Update Prefix");

        }

        [HarmonyPostfix, HarmonyPatch("Update")]
        private static void Update_Postfix(PlayerController __instance)
        {
            // Code to execute for each PlayerController *after* Update() is called.
            JellyJamTinyREPO.Logger.LogDebug($"{__instance} Update Postfix");

            if (__instance != null)
            {
                __instance.EnergyCurrent = 40;
                __instance.gameObject.transform.localScale = new Vector3(0.20f, 0.20f, 0.20f);
                __instance.cameraGameObject.transform.localPosition = new Vector3(__instance.cameraGameObject.transform.localPosition.x, -1.2f, __instance.cameraGameObject.transform.localPosition.z);

            }

        }
    }
}