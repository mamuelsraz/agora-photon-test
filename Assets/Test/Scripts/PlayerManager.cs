using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public static PlayerManager LocalPlayerInstance;
    public static Dictionary<PlayerType, string> PlayerTypeDirectories = new Dictionary<PlayerType, string>()
    {
        { PlayerType.PC, "Player" },
        { PlayerType.VR, "VRPlayer"}
    };

    public static Dictionary<int, PlayerManager> PlayerList = new Dictionary<int, PlayerManager>();

    public GameObject playerController;
    public int callID;
    PlayerType playerType;

    private void Awake()
    {
        PhotonPeer.RegisterType(typeof(SendPlayer), (byte)'H', SendPlayer.SerializeSendPlayer, SendPlayer.DeserializeSendPlayer);

        if (photonView.IsMine)
        {
            LocalPlayerInstance = this;
            playerType = PlayerType.PC;
            if (Application.platform == RuntimePlatform.Android) playerType = PlayerType.VR;
        }
    }

    private void Update()
    {

    }

    void SpawnPlayer()
    {
        string path = PlayerTypeDirectories[playerType];

        playerController = PhotonNetwork.Instantiate(path, Vector3.zero, Quaternion.identity);
    }

    public void OnPhotonInstantiate(Photon.Pun.PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        callID = (int)(data[0]);

        if (photonView.IsMine)
        {
            SpawnPlayer();
        }

        PlayerList.Add(callID, this);
    }

    private void OnDestroy()
    {
        PlayerList.Remove(callID);
    }

    [PunRPC]
    public void SendThroughNetwork(SendPlayer sendPlayer)
    {
        Debug.LogError($"Recieved: {sendPlayer.ID} {sendPlayer.playerType} | message {sendPlayer.message}");
    }
}

public class SendPlayer 
{
    public int ID;
    public PlayerType playerType;
    public string message;

    public SendPlayer()
    {

    }

    public SendPlayer(int ID, PlayerType playerType, string message = "")
    {
        this.ID = ID;
        this.playerType = playerType;
        this.message = message;
    }

    public static byte[] SerializeSendPlayer(object customobject)
    {
        SendPlayer pl = (SendPlayer)customobject;

        short strLenght = (short)System.Text.Encoding.UTF8.GetBytes(pl.message).Length;

        MemoryStream ms = new MemoryStream(4 + 4 + 2 + strLenght); //ID + PlayerType + string lenght + string

        ms.Write(BitConverter.GetBytes(pl.ID), 0, 4);
        ms.Write(BitConverter.GetBytes((int)pl.playerType), 0, 4);
        ms.Write(BitConverter.GetBytes(strLenght), 0, 2);
        ms.Write(System.Text.Encoding.UTF8.GetBytes(pl.message), 0, strLenght);

        return ms.ToArray();
    }

    public static object DeserializeSendPlayer(byte[] bytes)
    {
        SendPlayer pl = new SendPlayer();
        pl.ID = BitConverter.ToInt32(bytes, 0);
        pl.playerType = (PlayerType)BitConverter.ToInt32(bytes, 4);

        int strLenght = (int)BitConverter.ToInt16(bytes, 4 + 4);

        pl.message = System.Text.Encoding.UTF8.GetString(bytes, 4 + 4 + 2, strLenght);

        return pl;
    }
}

public enum PlayerType
{
    PC,
    VR
}

//https://answers.unity.com/questions/430090/photon-pun-type-serialization-error-on-rpc.html