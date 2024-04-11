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
        FlagSetting.Instance.ResetData();
        MotionManager.Instance.LoadMotion(zenMotion, garnetMotion, ludMotion);
        MotionManager.Instance.LoadMotionData();
        
        Platform runningPlatform = PlatformUtil.GetRunningPlatform();
        if (FlagSetting.Instance.grpc && (runningPlatform == Platform.Windows || runningPlatform == Platform.Linux))
        {
            GrpcServer.Instance.StartGrpcServer();
        }
        else
        {
            SocketServer.Instance.StartServer();
            FlagSetting.Instance.grpc = false;
            FlagSetting.Instance.socket = true;
        }
        
        string[] args = Environment.GetCommandLineArgs();
        FlagSetting.Instance.LoadArgs(args);
        
        if (FlagSetting.Instance.grpcAuto && ServiceUtils.IsGrpcOrSocketOpen())
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
