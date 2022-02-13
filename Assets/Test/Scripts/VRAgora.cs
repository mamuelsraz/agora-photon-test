using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if (UNITY_2018_3_OR_NEWER)
using UnityEngine.Android;
#endif
using agora_gaming_rtc;

public class VRAgora : MonoBehaviour
{
    // PLEASE KEEP THIS App ID IN SAFE PLACE
    // Get your own App ID at https://dashboard.agora.io/
    [SerializeField]
    private string appId = "AGORA-APP-ID-HERE";
    [SerializeField]
    private string roomName = "room1";

    private int numUsers = 0;
    private bool connected;

    private void Awake()
    {
        InitUI();
    }

    // Use this for initialization
    private ArrayList permissionList = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
#if (UNITY_2018_3_OR_NEWER)
        permissionList.Add(Permission.Microphone);
#endif
    }

    void InitUI()
    {
        GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(OnQuit);
    }

    private void CheckPermission()
    {
#if (UNITY_2018_3_OR_NEWER)
        foreach (string permission in permissionList)
        {
            if (Permission.HasUserAuthorizedPermission(permission))
            {
                if (!connected)
                    onJoinRoomClicked();
            }
            else
            {
                Permission.RequestUserPermission(permission);
            }
        }
# endif
    }

    // Update is called once per frame
    void Update()
    {
#if (UNITY_2018_3_OR_NEWER)
        CheckPermission();
#endif
    }

    private void onJoinRoomClicked()
    {
        if (!connected)
        {
            connected = true;
            loadEngine();
        }
        join(roomName);
        onSceneHelloVideoLoaded();
    }

    public void onLeaveButtonClicked()
    {

        if (connected)
        {
            leave();
            unloadEngine();
            connected = false;
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            if (IRtcEngine.QueryEngine() != null)
            {
                IRtcEngine.QueryEngine().DisableVideo();
            }
        }
        else
        {
            if (IRtcEngine.QueryEngine() != null)
            {
                IRtcEngine.QueryEngine().EnableVideo();
            }
        }
    }

    void OnApplicationQuit()
    {
        IRtcEngine.Destroy();
    }


    // load agora engine
    public void loadEngine()
    {
        // start sdk
        Debug.Log("initializeEngine");
        if (mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.getEngine(appId);

        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    // unload agora engine
    public void unloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }

    public void join(string channel)
    {
        Debug.Log("calling join (channel = " + channel + ")");
        if (mRtcEngine == null)
            return;

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
        mRtcEngine.OnUserJoined = onUserJoined;
        mRtcEngine.OnUserOffline = onUserOffline;

        // enable video
        mRtcEngine.EnableVideo();

        // allow camera output callback
        mRtcEngine.EnableVideoObserver();

        // join channel
        mRtcEngine.JoinChannel(channel, null, 0);


        Debug.Log("initializeEngine done");
    }

    public void leave()
    {
        Debug.Log("calling leave");

        if (mRtcEngine == null)
            return;

        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();

    }

    public string getSdkVersion()
    {
        return IRtcEngine.GetSdkVersion();
    }

    // accessing GameObject in Scnene1
    // set video transform delegate for statically created GameObject
    public void onSceneHelloVideoLoaded()
    {
        GameObject go = GameObject.Find("VideoSpawn");
        if (ReferenceEquals(go, null))
        {
            Debug.Log("BBBB: failed to find VideoQuad");
            return;
        }
    }

    // instance of agora engine
    public IRtcEngine mRtcEngine;

    // implement engine callbacks
    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void onUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid);
        // this is called in main thread

        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        numUsers++;
        PutUser(uid);
    }

    void PutUser(uint uid)
    {
        // create a GameObject and assigne to this new user
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        if (!ReferenceEquals(go, null))
        {
            go.name = uid.ToString();

            // configure videoSurface
            VideoSurface o = go.AddComponent<VideoSurface>();
            o.SetForUser(uid);
            o.SetEnable(true);

            // Adjust view transform
            var videoQuadPos = GameObject.Find("VideoSpawn").transform.position;
            go.transform.position = videoQuadPos + new Vector3(numUsers * 0.95f, 0, 0);
            go.transform.localScale = new Vector3(0.1f, 0.5f, 0.1f);
            go.transform.Rotate(-90.0f, 1.0f, 0.0f);

            AssignShader(go);
        }
    }

    void AssignShader(GameObject go)
    {
        Material material = Resources.Load<Material>("PlaneMaterial");
        MeshRenderer mesh = go.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            mesh.material = material;
        }
    }
    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid);
        // this is called in main thread
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Destroy(go);
        }
        numUsers--;
    }

    void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }


}
