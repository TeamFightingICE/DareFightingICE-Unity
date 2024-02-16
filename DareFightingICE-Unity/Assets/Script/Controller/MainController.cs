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
        GrpcServer.Instance.StartGrpcServer();

        string[] args = Environment.GetCommandLineArgs();
        FlagSetting.Instance.LoadArgs(args);
        
        if (FlagSetting.Instance.grpcAuto)
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
