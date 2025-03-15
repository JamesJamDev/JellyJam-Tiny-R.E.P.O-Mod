using HarmonyLib;
using UnityEngine;

namespace JellyJamTinyREPO
{
    [HarmonyPatch(typeof(PlayerController))]
    internal static class ExamplePlayerControllerPatch
    {
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
        }
    }
}