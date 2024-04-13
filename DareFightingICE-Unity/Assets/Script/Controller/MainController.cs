using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    [SerializeField] private TextAsset zenMotion;
    [SerializeField] private TextAsset ludMotion;
    [SerializeField] private TextAsset garnetMotion;
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        string[] args = Environment.GetCommandLineArgs();
        FlagSetting.Instance.LoadArgs(args);

        FlagSetting.Instance.ResetData();
        MotionManager.Instance.LoadMotion(zenMotion, garnetMotion, ludMotion);
        MotionManager.Instance.LoadMotionData();
        
        if (FlagSetting.Instance.useSocket)
        {
            SocketServer.Instance.StartServer();
        }
        else if (FlagSetting.Instance.useGrpc)
        {
            GrpcServer.Instance.StartServer();
        }
        else
        {
            Debug.Log("No server started");
        }
        
        if (FlagSetting.Instance.autoMode && ServiceUtils.IsServerOpen())
        {
            SceneManager.LoadScene("GrpcAuto");
        }
        else
        {
            SceneManager.LoadScene("Launch");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
