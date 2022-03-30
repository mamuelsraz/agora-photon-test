## Agora + photon implementation
# How to use
-spawn the AgoraPhotonManager using PhotonNetwork.Instantiate
-use OnPlayerSynced and OnPlayerUnSynced events to know when someone joined the agora+photon call
-AgoraPhotonManager.local.syncedPlayers shows you the ids of players in call
-ids of clients are the same in photon (PhotonPlayer.ActorNumber) and agora (uid)