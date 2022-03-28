using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using Photon.Pun;

public class VideoSurfaceSpawn : MonoBehaviourPunCallbacks
{
    public AgoraPhotonManager agoraManager;
    public GameObject prefab;
    VideoSurface vid;
    Transform holder;

    private void Start()
    {
        agoraManager.OnPlayerSynced.AddListener(OnJoinedPlayer);
    }

    void OnJoinedPlayer(uint id)
    {
        holder = GameObject.Find("holder").transform;
        vid = Instantiate(prefab, holder).GetComponent<VideoSurface>();

        vid.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
        //if (photonView.IsMine) vid.SetForUser(0);
        //else
            vid.SetForUser(id);
        vid.SetEnable(true);
    }
}
