using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using agora_gaming_rtc;
using agora_utilities;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.Events;

public class CallManager : MonoBehaviour
{
    public static CallManager instance;
    public static CallEngine app = null;

    //NEKOUKAT!!
    public string AppID;
    List<uint> notYetSynced;
    public SyncEvent OnSynced;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
		permissionList.Add(Permission.Microphone);
#endif

        notYetSynced = new List<uint>();
    }

    private void Start()
    {
        CheckPermissions();
    }

    void Update()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        CheckPermissions();
#endif
    }

    /// <summary>
    ///   Checks for platform dependent permissions.
    /// </summary>
    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach(string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {                 
				Permission.RequestUserPermission(permission);
			}
        }
#endif
    }

    public void JoinCall(string roomName, uint id)
    {
        // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new CallEngine(); // create app
            app.loadEngine(AppID); // load engine
        }

        // join channel
        app.join(roomName, id);

        app.mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        app.mRtcEngine.OnUserJoined = OnUserJoined;
        app.mRtcEngine.OnUserOffline = OnUserOffline;
    }

    private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
    }

    private void OnUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        // this is called in main thread

        notYetSynced.Add(uid);

        TryDisplayPlayerCam(uid);
    }

    public void TryDisplayPlayerCam(uint uid)
    {
        // find a game object to render video stream from 'uid'
        if (PlayerManager.PlayerList.ContainsKey((int)uid) && PlayerManager.PlayerList[(int)uid].playerController != null && notYetSynced.Contains(uid))
        {
            OnSynced.Invoke((int)uid);

            GameObject player = PlayerManager.PlayerList[(int)uid].playerController.gameObject;
            GameObject quad = player.transform.Find("Quad").gameObject;
            Debug.Log(uid);
            quad.name = uid.ToString();

            VideoSurface videoSurface = quad.AddComponent<VideoSurface>();

            if (!ReferenceEquals(videoSurface, null))
            {
                // configure videoSurface
                videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
                videoSurface.SetForUser(uid);
                videoSurface.SetEnable(true);
            }

            notYetSynced.Remove(uid);
        }
        else
        {
            Debug.LogWarning("Player was not found (likely not instanciated yet)");
        }
    }

    private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
    }

    void OnApplicationPause(bool paused)
    {
        if (!ReferenceEquals(app, null))
        {
            app.EnableVideo(paused);
        }
    }

    void OnApplicationQuit()
    {
        if (!ReferenceEquals(app, null))
        {
            LeaveCall();
        }
    }

    public void LeaveCall()
    {
        if (!ReferenceEquals(app, null))
        {
            app.leave(); // leave channel
            app.unloadEngine(); // delete engine
            app = null; // delete app
        }
    }
}

[System.Serializable]
public class SyncEvent : UnityEvent<int>
{
}
