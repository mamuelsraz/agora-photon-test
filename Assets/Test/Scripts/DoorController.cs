using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorController : MonoBehaviour
{
    private void Update()
    {
        if (RoomManager.gameState.doorOpen == true)
        {
            transform.position = Vector3.Lerp(transform.position, Vector3.up * 3, 0.4f);
        }
        else transform.position = Vector3.Lerp(transform.position, Vector3.zero, 0.4f);
    }
}
