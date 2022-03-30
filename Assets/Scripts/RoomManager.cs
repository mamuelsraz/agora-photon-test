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
        //the player will be conscious of its id thanks to this 
        int id = PhotonNetwork.LocalPlayer.ActorNumber;
        object[] IDdata = new object[]{(object)id};

        PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity, 0, data: IDdata);
    }
}
