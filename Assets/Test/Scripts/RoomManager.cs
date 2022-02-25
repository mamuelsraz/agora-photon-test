using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using ExitGames.Client.Photon;

namespace Photon.Pun
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        static public RoomManager Instance;
        static public GameState gameState;
        public CallManager call;

        public string PlayerPrefabName;

        private void Awake()
        {
            PhotonPeer.RegisterType(typeof(GameState), (byte)'G', GameState.SerializeGameState, GameState.DeserializeGameState);
        }

        private void Start()
        {
            Instance = this;

            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Lobby");

                return;
            }

            if (PlayerManager.LocalPlayerInstance == null)
            {
                SpawnPlayer();
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }

            if (photonView.IsMine && PhotonNetwork.IsMasterClient)
            {
                gameState = new GameState();
                gameState.bannerMessage = "========THIS IS THIS ROOM'S BANNER========";
                gameState.startTime = DateTime.Now;
            }
        }

        void Update()
        {
            // "back" button of phone equals "Escape". quit app if that's pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LeaveRoom();
            }
        }

        [PunRPC]
        public void GetGameState(GameState state)
        {
            gameState = state;
            Debug.LogError(state.bannerMessage);
            Debug.LogError(state.startTime);
        }

        [PunRPC]
        public void ChangedMaster(GameState state)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            gameState = state;
            state.startTime = DateTime.Now;
            Debug.LogError("I am the new master!");
            Debug.LogError(state.bannerMessage);
            Debug.LogError(state.startTime);
            photonView.RPC("GetGameState", RpcTarget.Others, state);
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Lobby");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            photonView.RPC("GetGameState", newPlayer, gameState);
        }

        public void LeaveRoom()
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerListOthers.Length > 0)
            {
                Player p = PhotonNetwork.PlayerListOthers[0];
                photonView.RPC("ChangedMaster", p, gameState);
            }
            PhotonNetwork.LeaveRoom();
            call.LeaveCall();
        }

        void SpawnPlayer()
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            int id = PhotonNetwork.LocalPlayer.ActorNumber;
            object[] dataName = new object[]{
                    (object)id
                };

            GameObject instance = PhotonNetwork.Instantiate(PlayerPrefabName, Vector3.zero, Quaternion.identity, 0, data: dataName);

            //call.JoinCall(PhotonNetwork.CurrentRoom.Name, (uint)PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }
}

public class GameState
{
    public DateTime startTime;
    public string bannerMessage;

    public static byte[] SerializeGameState(object customobject)
    {
        GameState gs = (GameState)customobject;

        short strLenght = (short)System.Text.Encoding.UTF8.GetBytes(gs.bannerMessage).Length;

        MemoryStream ms = new MemoryStream(8 + 2 + strLenght); //datetime + string lenght + string

        Debug.Log(strLenght);

        ms.Write(BitConverter.GetBytes(gs.startTime.Ticks), 0, 8);
        ms.Write(BitConverter.GetBytes(strLenght), 0, 2);
        ms.Write(System.Text.Encoding.UTF8.GetBytes(gs.bannerMessage), 0, strLenght);

        return ms.ToArray();
    }

    public static object DeserializeGameState(byte[] bytes)
    {
        GameState gs = new GameState();
        gs.startTime = new DateTime(BitConverter.ToInt64(bytes, 0));

        int strLenght = (int)BitConverter.ToInt16(bytes, 8);

        gs.bannerMessage = System.Text.Encoding.UTF8.GetString(bytes, 4 + 4 + 2, strLenght);

        return gs;
    }
}
