using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMute : MonoBehaviour
{
    public int id;

    bool mute;

    public void MuteToggle()
    {
        Debug.Log("muting user " + id);
        CallManager.app.mRtcEngine.MuteRemoteAudioStream((uint)id, mute);
        mute = !mute;
    }
}
