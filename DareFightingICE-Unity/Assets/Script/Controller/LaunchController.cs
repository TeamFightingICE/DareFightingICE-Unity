using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LaunchController : MonoBehaviour
{
    /// <summary>
    /// This script use to collect every input inside launch scene to GameSetting and FlagSetting
    /// </summary>
    [SerializeField] private TMP_InputField p1Hp;
    [SerializeField] private TMP_InputField p2Hp;
    [SerializeField] private TMP_InputField roundLimit;
    [SerializeField] private TMP_InputField frameLimit;
    [SerializeField] private Toggle slowFlag;
    [SerializeField] private Toggle debugFlag;
    [SerializeField] private Toggle frameFlag;
    [SerializeField] private Toggle muteFlag;

    [SerializeField] private TextAsset zenMotion;
    [SerializeField] private TextAsset ludMotion;
    [SerializeField] private TextAsset garnetMotion;
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        GameSetting.Instance.ResetData();
        FlagSetting.Instance.ResetData();
        MotionManager.Instance.LoadMotion(zenMotion,garnetMotion,ludMotion);
        GrpcServer.Instance.StartGrpcServer();
    }

    public void Launch()
    {
        
        GameSetting.Instance.Setdata(int.Parse(p1Hp.text),int.Parse(p2Hp.text),int.Parse(roundLimit.text),int.Parse(frameLimit.text));
        MotionManager.Instance.LoadMotionData();
        SceneManager.LoadScene("Start");
    }
    
}
