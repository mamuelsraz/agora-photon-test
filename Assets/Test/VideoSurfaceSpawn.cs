using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using Photon.Pun;

public class VideoSurfaceSpawn : MonoBehaviourPunCallbacks
{
    //uses the OnPlayerSynced and OnPlayerUnSynced events to spawn videosurfaces

    public AgoraPhotonManager agoraManager;
    public GameObject prefab;
    Transform holder;

    List<uint> indexes;
    List<GameObject> surfaces;

    private void Start()
    {
        if (photonView.IsMine)
        {
            indexes = new List<uint>();
            surfaces = new List<GameObject>();

            agoraManager.OnPlayerSynced.AddListener(OnJoinedPlayer);
            agoraManager.OnPlayerUnSynced.AddListener(OnLeftPlayer);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && photonView.IsMine)
        {
            AgoraPhotonManager.local.LeaveCall();
        }
    }

    void OnJoinedPlayer(uint id)
    {
        if (!photonView.IsMine) return;

        holder = GameObject.Find("holder").transform;
        VideoSurface vid = Instantiate(prefab, holder).GetComponent<VideoSurface>();
        vid.gameObject.name = id.ToString();

        vid.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
        if ((uint)PhotonNetwork.LocalPlayer.ActorNumber == id)
            vid.SetForUser(0);
        else
            vid.SetForUser(id);

        vid.SetEnable(true);

        surfaces.Add(vid.gameObject);
        indexes.Add(id);
    }

    void OnLeftPlayer(uint id)
    {
        if (!photonView.IsMine) return;

        if (indexes.Contains(id))
        {
            int index = indexes.IndexOf(id);
            indexes.RemoveAt(index);

            Destroy(surfaces[index]);
            surfaces.RemoveAt(index);
        }
    }
}
