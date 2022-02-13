using UnityEngine;
using Photon.Realtime;

namespace Photon.Pun
{
    public class JoinManager : MonoBehaviourPunCallbacks
    {
        public string RoomScene = "Room";
        bool isConnecting = false;

        private void Awake()
        {
            isConnecting = false;
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public void Connect()
        {
            isConnecting = true;

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else 
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = "1";
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 5});
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogError("Disconnected");

            isConnecting = false;
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

            // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");

                // #Critical
                // Load the Room Level. 
                PhotonNetwork.LoadLevel(RoomScene);
            }
        }
    }
}
