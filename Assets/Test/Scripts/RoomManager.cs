using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Photon.Pun
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        static public RoomManager Instance;
        public CallManager call;

        private GameObject instance;

        [SerializeField]
        private GameObject playerPrefab;

        private void Start()
        {
            Instance = this;

            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Lobby");

                return;
            }

            if (ClientManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                string name = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
                object[] dataName = new object[]{
                    (object)name
                };

                GameObject instance = PhotonNetwork.Instantiate("Player", new Vector3(0f, 0f, 0f), Quaternion.identity, 0, data: dataName);
                call.JoinCall(PhotonNetwork.CurrentRoom.Name, (uint)PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
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

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Lobby");
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            call.LeaveCall();
        }
    }
}
