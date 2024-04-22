using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

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
    public bool isReplay;
    public TextMeshProUGUI ReplayFileName;
    public int ReplayFileNumber = 0;
    public List<string> ReplayFilesNames;
    public List<string> ReplayFilesPaths;
    public Button grpcAutoBtn;

    void Start()
    {
        LoadReplayFiles();
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

    public void Replay() 
    {
        SceneManager.LoadScene("Replay");
    }
    public void LoadReplayFiles() 
    {
        var path = Application.persistentDataPath;
        var folder = "log/replay";
        string Filepath = Path.Combine(path, folder);
        if(Directory.Exists(Filepath)) 
        {
            
            DirectoryInfo DInfo = new DirectoryInfo(Filepath);
            foreach(var file in DInfo.GetFiles("*.dat")) 
            {
                ReplayFilesNames.Add(file.Name);
                ReplayFilesPaths.Add(file.FullName);
            }
            if(ReplayFilesNames.Count > 0) 
            {
                isReplay = true;
                ReplayFileName.text = ReplayFilesNames[0];
                GameSetting.Instance.ReplayFilePath = ReplayFilesPaths[0];
            }
            

        }
        else 
        {
            isReplay = false;
        }
    }
    
}
