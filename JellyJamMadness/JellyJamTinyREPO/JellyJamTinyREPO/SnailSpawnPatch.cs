using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using UnityEngine.AI; // For PhotonNetwork.Instantiate

namespace JellyJamSnailModNamespace
{
    [HarmonyPatch(typeof(GameDirector), "gameStateMain")]
    public class SnailSpawnPatch
    {
        private static float timer = 0f;
        private static bool snailSpawned = false;

        static void Postfix(GameDirector __instance)
        {
            if (snailSpawned || JellyJamSnailMod.snailPrefab == null)
                return;

            timer += Time.deltaTime;

            if (timer < 30f)
                return;

            // Always spawn at the first SpawnPoint + Vector3.up
            SpawnPoint[] spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();

            if (spawnPoints.Length == 0)
            {
                JellyJamSnailMod.Logger.LogError("No spawn points found — cannot spawn snail.");
                return;
            }

            Vector3 spawnPosition = spawnPoints[0].transform.position + Vector3.up;
            Quaternion spawnRotation = Quaternion.identity;

            GameObject snail;
            if (GameManager.Multiplayer())
            {
                snail = PhotonNetwork.Instantiate(JellyJamSnailMod.snailPrefab.name, spawnPosition, spawnRotation);
                JellyJamSnailMod.Logger.LogInfo("Snail spawned via PhotonNetwork at backup spawn point.");
            }
            else
            {
                snail = Object.Instantiate(JellyJamSnailMod.snailPrefab, spawnPosition, spawnRotation);
                JellyJamSnailMod.Logger.LogInfo("Snail spawned locally at backup spawn point.");
            }

            snail.name = "NetworkedSnail";
            snailSpawned = true;

            // Add components dynamically
            if (snail.GetComponent<SnailChase>() == null)
            {
                snail.AddComponent<SnailChase>();
            }

            if (snail.GetComponent<SnailSync>() == null)
            {
                snail.AddComponent<SnailSync>();
            }

            if (snail.GetComponent<PhotonView>() == null)
            {
                snail.AddComponent<PhotonView>();
            }

            if (snail.GetComponent<EnemyNavMeshAgent>() == null)
            {
                snail.AddComponent<EnemyNavMeshAgent>();
            }

            if (snail.GetComponent<NavMeshAgent>() == null)
            {
                snail.AddComponent<NavMeshAgent>();
            }

            // If SnailSync requires initialization (e.g., setting speed), you can do that here:
            var snailSync = snail.GetComponent<SnailSync>();
            snailSync.speed = 1f; // example, adjust as needed

        }
    }
}
