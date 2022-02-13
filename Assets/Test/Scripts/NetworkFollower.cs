using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkFollower : MonoBehaviourPunCallbacks, IPunObservable
{
    public string targetName;
    Transform target;

    private void Start()
    {
        if (photonView.IsMine)
        {
            //Camera.main.gameObject.SetActive(false)
            target = GameObject.Find(targetName).transform;
            if (target == null)
            {
                Debug.LogError("Couldn't find target");
            }
        }
    }

    private void Update()
    {
        if (photonView.IsMine && target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
        }
        else
        {
            // Network player, receive data
        }
    }

    #endregion
}
