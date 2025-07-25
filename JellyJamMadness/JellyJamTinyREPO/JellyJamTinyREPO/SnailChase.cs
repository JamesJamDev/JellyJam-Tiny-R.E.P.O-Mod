using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PhotonView))]
public class SnailChase : MonoBehaviourPun
{
    private NavMeshAgent navAgent;
    private Transform targetPlayer;

    private float checkTargetInterval = 0.5f;
    private float checkTimer;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = true;  // optional, depends on your needs
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            FindClosestPlayer();
            checkTimer = checkTargetInterval;
        }

        if (targetPlayer != null && IsAgentValid())
        {
            navAgent.SetDestination(targetPlayer.position);
        }
    }

    private void FindClosestPlayer()
    {
        float closestDist = float.MaxValue;
        Transform closest = null;

        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = player.transform;
            }
        }
        targetPlayer = closest;
    }

    private bool IsAgentValid()
    {
        if (navAgent == null || !navAgent.enabled)
            return false;

        NavMeshHit hit;
        return NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!photonView.IsMine) return;

        if (other.collider.CompareTag("Player"))
        {
            photonView.RPC(nameof(KillPlayerRPC), RpcTarget.All, other.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    private void KillPlayerRPC(int playerViewID)
    {
        var pv = PhotonView.Find(playerViewID);
        if (pv != null && pv.gameObject.CompareTag("Player"))
        {
            var health = pv.gameObject.GetComponent<PlayerHealth>();
            if (health != null)
            {
                Debug.Log("KILL PLAYER");
                health.Death(); 
            }
        }
    }
}
