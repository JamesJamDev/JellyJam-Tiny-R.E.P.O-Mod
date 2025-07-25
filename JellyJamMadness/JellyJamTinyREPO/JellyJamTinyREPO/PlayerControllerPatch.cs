using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace JellyJamTinyREPO
{
    [HarmonyPatch(typeof(PlayerController))]
    internal static class PlayerControllerPatch
    {

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPatch(PlayerController __instance)
        {
            
        }
    }
}