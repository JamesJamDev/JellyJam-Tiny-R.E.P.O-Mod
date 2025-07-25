using UnityEngine;
using Photon.Pun;

public class SnailSync : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public float speed = 1f;
    private Transform target;

    void Start()
    {
        if (photonView.IsMine)
        {
            target = Camera.main.transform; // or find Character root
            InvokeRepeating(nameof(SendPosition), 0f, 0.1f);
        }
    }

    void Update()
    {
        if (photonView.IsMine && target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
            transform.LookAt(target);
        }
    }

    void SendPosition()
    {
        photonView.RPC(nameof(UpdateSnailOnClients), RpcTarget.OthersBuffered, transform.position, transform.rotation);
    }

    [PunRPC]
    void UpdateSnailOnClients(Vector3 pos, Quaternion rot)
    {
        if (!photonView.IsMine)
        {
            transform.position = pos;
            transform.rotation = rot;
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) { }
}
