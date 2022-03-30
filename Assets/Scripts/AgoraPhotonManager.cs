using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using agora_gaming_rtc;
using UnityEngine.Events;

public class AgoraPhotonManager : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    //the appID of your agora project. Get it here: https://console.agora.io/
    public string appID;
    [Space]

    public AgoraEngine engine;
    public static AgoraPhotonManager local;
    public List<uint> syncedPlayers = new List<uint>();

    //player with id joins/leaves call
    [HideInInspector] public IdEvent OnPlayerSynced;
    [HideInInspector] public IdEvent OnPlayerUnSynced;

    //the photon actorNumber and agora uid
    uint id;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            id = (uint)PhotonNetwork.LocalPlayer.ActorNumber;
            local = this;
            JoinCall(PhotonNetwork.CurrentRoom.Name, id);
        }
    }

    //set the id of this player
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] IDdata = info.photonView.InstantiationData;
        id = (uint)(int)(IDdata[0]);
    }

    public void LeaveCall()
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

    //add new player to syncedPlayers
    private void OnUserJoined(uint uid, int elapsed)
    {
        if (!photonView.IsMine) return;

        syncedPlayers.Add(uid);

        OnPlayerSynced?.Invoke(uid);
    }

    //remove player from syncedplayers
    private void OnUserLeft(uint uid, USER_OFFLINE_REASON reason)
    {
        if (!photonView.IsMine) return;

        Debug.Log("onUserLeft: uid = " + uid + " reason = " + reason);

        syncedPlayers.Remove(uid);

        OnPlayerUnSynced?.Invoke(uid);
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

