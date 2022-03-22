using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListUI : MonoBehaviourPunCallbacks
{
    public GameObject placeItem;
    public Transform parent;
    List<GameObject> items;
    List<int> ids;

    private void Start()
    {
        items = new List<GameObject>();
        ids = new List<int>();

        CallManager.instance.OnSynced.AddListener(PlaceItem);
    }

    void PlaceItem(int id)
    {
        GameObject instance = Instantiate(placeItem, parent);
        items.Add(instance);
        ids.Add(id);

        TextMeshProUGUI text = instance.GetComponentInChildren<TextMeshProUGUI>();
        text.text = $"{PhotonNetwork.CurrentRoom.GetPlayer(id).ActorNumber} {PhotonNetwork.CurrentRoom.GetPlayer(id).NickName}";

        Button button = instance.GetComponentInChildren<Button>();
        ToggleMute muteScript = instance.GetComponentInChildren<ToggleMute>();
        muteScript.id = id;
        button.onClick.AddListener(muteScript.MuteToggle);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"player {otherPlayer.NickName} left the room");
        int i = otherPlayer.ActorNumber;
        int index = ids.IndexOf(i);

        Destroy(items[index]);
        items.RemoveAt(index);
        ids.RemoveAt(index);
    }
}
