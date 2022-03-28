using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using agora_gaming_rtc;
using UnityEngine.Events;

public class AgoraPhotonManager : MonoBehaviourPunCallbacks
{
    public string appID;
    [Space]

    public AgoraEngine engine;
    public static AgoraPhotonManager local;
    public List<uint> syncedPlayers = new List<uint>();

    [HideInInspector] public IdEvent OnPlayerSynced;

    uint id;

    private void Awake()
    {
        id = (uint)PhotonNetwork.LocalPlayer.ActorNumber;

        if (photonView.IsMine)
        {
            local = this;
            JoinCall(PhotonNetwork.CurrentRoom.Name, id);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LeaveCall();
        }
    }

    private void LeaveCall()
    {

        if (!photonView.IsMine) return;

        if (!ReferenceEquals(engine, null))
        {
            engine.Leave(); // leave channel
            engine.UnloadEngine(); // delete engine
            engine = null; // delete app
        }

        syncedPlayers.Remove(id);
        PhotonNetwork.LeaveRoom();
    }

    private void JoinCall(string room, uint id)
    {
        // create app if nonexistent
        if (ReferenceEquals(engine, null))
        {
            engine = new AgoraEngine();
            engine.LoadEngine(appID);
        }

        engine.Join(room, id);

        engine.mRtcEngine.OnUserJoined = OnUserJoined;
        engine.mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        engine.mRtcEngine.OnUserOffline = OnUserLeft;
    }

    private void OnJoinChannelSuccess(string channel, uint uid, int elapsed)
    {
        OnUserJoined(uid, elapsed);
    }

    private void OnUserJoined(uint uid, int elapsed)
    {
        if (!photonView.IsMine) return;

        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        syncedPlayers.Add(uid);

        OnPlayerSynced?.Invoke(uid);
    }

    private void OnUserLeft(uint uid, USER_OFFLINE_REASON reason)
    {
        if (!photonView.IsMine) return;

        Debug.Log("onUserLeft: uid = " + uid + " reason = " + reason);

        syncedPlayers.Remove(uid);
    }

    void OnApplicationQuit()
    {
        LeaveCall();
    }
}

[System.Serializable]
public class IdEvent : UnityEvent<uint>
{
}

