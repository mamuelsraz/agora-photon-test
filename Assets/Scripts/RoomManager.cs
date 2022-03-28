using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public string prefabName;
    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Lobby");

            return;
        }

        if (AgoraPhotonManager.local == null)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity, 0);
    }
}
