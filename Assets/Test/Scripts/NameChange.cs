using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameChange : MonoBehaviour, IPunInstantiateMagicCallback
{
	public void OnPhotonInstantiate(Photon.Pun.PhotonMessageInfo info)
	{
		// Example... 
		Debug.Log("Is this mine?... " + info.Sender.IsLocal.ToString());
		object[] data = info.photonView.InstantiationData;
		gameObject.name = data[0] as string;
	}
}
