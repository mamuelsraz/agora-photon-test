using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

namespace Photon.Pun
{
    public class JoinManager : MonoBehaviourPunCallbacks, IConnectionCallbacks
    {
        //simple implementation of joining the client to a photon server

        public TMP_InputField nameInput;
        public TextMeshProUGUI text;
        public TextMeshProUGUI buttonText;
        public Button button;

        public string RoomScene = "Room";

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public void Connect()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.GameVersion = "1";
                text.text = "Connecting to photon...";
                button.interactable = false;
                PhotonNetwork.ConnectUsingSettings();
            }
            else 
            {
                text.text = "Connecting to a random room...";
                button.interactable = false;
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnConnected()
        {
            text.text = "Connected to Photon";
            buttonText.text = "Join random Room";
            button.interactable = true;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            button.interactable = true;

            Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 20});
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            button.interactable = true;
            Debug.LogError("Disconnected");
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.NickName = nameInput.text;


            Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

            // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the Room Scene");

                PhotonNetwork.LoadLevel(RoomScene);
            }
        }
    }
}
