using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using agora_gaming_rtc;
using agora_utilities;
using UnityEngine.UI;
using UnityEngine.Android;

public class CallManager : MonoBehaviour
{
    //NEKOUKAT!!
    public string AppID;

    static VideoManager app = null;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif

    void Awake()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
		permissionList.Add(Permission.Microphone);
#endif
    }

    private void Start()
    {
        CheckPermissions();
    }

    void Update()
    {
#if (UNITY_2018_3_OR_NEWER)
        CheckPermissions();
#endif
    }

    /// <summary>
    ///   Checks for platform dependent permissions.
    /// </summary>
    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER)
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
            app = new VideoManager(); // create app
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

        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find(uid.ToString());

        GameObject obj = go.transform.Find("Quad").gameObject;
        Debug.Log(uid);
        obj.name = uid.ToString();
        //obj.AddComponent<RawImage>();
        VideoSurface videoSurface = obj.AddComponent<VideoSurface>();

        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
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
