using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class DontDestroy : MonoBehaviour
{

    void Start()
    {
        DontDestroyOnLoad(this);

        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);

        Debug.Log(Microphone.devices.Length);
        Debug.Log(Application.internetReachability.ToString());
    }

    private void Update()
    {
       
    }
}
