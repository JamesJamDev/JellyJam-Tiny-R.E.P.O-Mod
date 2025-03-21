﻿using ExitGames.Client.Photon;
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
        // Player Stats 

        // Size & Camera Offset
        public static float playerSize = 0.25f;
        private static float cameraOffset = -1.05f; //-1.05

        // Move Speeds
        private static float walkSpeed = 0.10f;
        private static float runSpeed = 0.15f;
        private static float crouchSpeed = 0.05f;

        // Jump
        private static float jumpModifier = 12f;

        // Grab / Interaction
        private static float grabDis = 2f;
        private static float minHeldDis = 0.5f;
        private static float maxHeldDis = 1.5f;

        private static bool infiniteEnergy = true;


        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPatch(PlayerController __instance)
        {
            TriggerShrinkEvent();

            if (SemiFunc.IsMasterClientOrSingleplayer())
            {
                LocalShrink(__instance);
            }
        }

        public static void LocalShrink(PlayerController __instance)
        {
            __instance.transform.localScale = new UnityEngine.Vector3(PlayerControllerPatch.playerSize, PlayerControllerPatch.playerSize, PlayerControllerPatch.playerSize); // Shrink player
            __instance.playerAvatarPrefab.transform.localScale = new UnityEngine.Vector3(PlayerControllerPatch.playerSize + 0.05f, PlayerControllerPatch.playerSize + 0.05f, PlayerControllerPatch.playerSize + 0.05f);
        }

        [HarmonyPostfix, HarmonyPatch("Update")]
        private static void Update_Postfix(PlayerController __instance)
        {
            // Make adjustments to the player stats each frame
            // This is likely not a smart way of doing it but hey, it works
            if (__instance != null)
            {
                SetOverride(__instance);
            }

        }

        public static void TriggerShrinkEvent(int playerId = -1)
        {
            object[] data = new object[] { playerId }; // Send -1 to shrink all players
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(JellyJamTinyREPO.ShrinkEvent.EventCode, data, options, SendOptions.SendReliable);
        }

        private static void SetOverride(PlayerController __instance)
        {
            /* Lets detail all the changes we make incase anyone wants to adjust the values
             * Adjust the values at the top of the script to change them down here easily
             */

            // Adjust the collision to check if the player is grounded
            // This is to prevent the player from jumping on walls
            __instance.CollisionGrounded.Collider.radius = 0.04f;

            // Adjust size of player (x, y, z) scale
            //__instance.gameObject.transform.localScale = new Vector3(playerSize, playerSize, playerSize);

            // This is just to adjust the camera position as it does not follow the actual player scale
            CameraCrouchPosition.instance.gameObject.transform.localPosition = new UnityEngine.Vector3(CameraCrouchPosition.instance.gameObject.transform.localPosition.x, cameraOffset, CameraCrouchPosition.instance.gameObject.transform.localPosition.z);

            // Move Speed for walking
            __instance.playerOriginalMoveSpeed = walkSpeed;

            // Move speed for sprinting
            __instance.playerOriginalSprintSpeed = runSpeed;

            // Move speed for crouching
            __instance.playerOriginalCrouchSpeed = crouchSpeed;

            // How high you jump
            __instance.JumpForce = jumpModifier;

            // Distance you can grab objects from
            PhysGrabber.instance.grabRange = grabDis;

            if (infiniteEnergy)
            {
                __instance.EnergyCurrent = 40;
            }

            // Minimum / Max distance for held items (when you move with scroll wheel)
            PhysGrabber.instance.minDistanceFromPlayer = minHeldDis;
            PhysGrabber.instance.maxDistanceFromPlayer = maxHeldDis;
        }
    }
}