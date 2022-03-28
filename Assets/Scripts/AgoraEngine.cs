using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using agora_utilities;

public class AgoraEngine
{
    public IRtcEngine mRtcEngine;
    private string token = "";

    public void LoadEngine(string appId)
    {
        // start sdk
        if (mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.GetEngine(appId);

        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    public void Join(string channel, uint id)
    {
        Debug.Log("calling join (channel = " + channel + " " + id + ")");

        if (mRtcEngine == null)
            return;

        mRtcEngine.OnWarning = (int warn, string msg) =>
        {
            Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
        };
        mRtcEngine.OnError = HandleError;

        // enable video
        mRtcEngine.EnableVideo();
        // allow camera output callback
        mRtcEngine.EnableVideoObserver();

        /*  This API Accepts AppID with token; by default omiting info and use 0 as the local user id */
        mRtcEngine.JoinChannelByKey(channelKey: token, channelName: channel, uid: id);
    }

    public string GetSdkVersion()
    {
        string ver = IRtcEngine.GetSdkVersion();
        return ver;
    }

    public void Leave()
    {
        if (mRtcEngine == null)
            return;

        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();
    }

    public void UnloadEngine()
    {
        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            mRtcEngine = null;
        }
    }

    public void EnableVideo(bool pauseVideo)
    {
        if (mRtcEngine != null)
        {
            if (!pauseVideo)
            {
                mRtcEngine.EnableVideo();
            }
            else
            {
                mRtcEngine.DisableVideo();
            }
        }
    }

    #region Error Handling
    private int LastError { get; set; }
    private void HandleError(int error, string msg)
    {
        if (error == LastError)
        {
            return;
        }

        /*
        if (string.IsNullOrEmpty(msg))
        {
            msg = string.Format("Error code:{0} msg:{1}", error, IRtcEngine.GetErrorDescription(error));
        }

        switch (error)
        {
            case 101:
                msg += "\nPlease make sure your AppId is valid and it does not require a certificate for this demo.";
                break;
        }*/

        Debug.LogError(msg);

        LastError = error;
    }

    #endregion
}
