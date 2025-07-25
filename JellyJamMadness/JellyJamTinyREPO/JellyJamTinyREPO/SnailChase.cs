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

    public float moveForce = 10f;
    public float maxSpeed = 5f;

        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        FindClosestPlayer();
        }

        void FixedUpdate()
        {
            if (targetPlayer == null) return;

            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            Vector3 force = direction * moveForce;

            // Only add force if not already going too fast
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(force, ForceMode.Acceleration);
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
                Debug.Log("KILL PLAYER");
        var pv = PhotonView.Find(playerViewID);
        if (pv != null && pv.gameObject.CompareTag("Player"))
        {
            var health = pv.gameObject.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.Death(); 
            }
        }
    }
}
