using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LaunchController : MonoBehaviour
{
    [SerializeField] private TMP_InputField p1Hp;
    [SerializeField] private TMP_InputField p2Hp;
    [SerializeField] private TMP_InputField roundLimit;
    [SerializeField] private TMP_InputField frameLimit;
    [SerializeField] private Toggle slowFlag;
    [SerializeField] private Toggle debugFlag;
    [SerializeField] private Toggle frameFlag;
    [SerializeField] private Toggle muteFlag;
    public Button grpcAutoBtn;

    void Start()
    {
        
    }
    void Update() {
        grpcAutoBtn.interactable = ServiceUtils.IsServerOpen();
    }

    public void Launch()
    {
        SceneManager.LoadScene("Start");
    }

    public void GrpcAuto() {
        if (!ServiceUtils.IsServerOpen()) return;

        SceneManager.LoadScene("GrpcAuto");
    }
    
}
