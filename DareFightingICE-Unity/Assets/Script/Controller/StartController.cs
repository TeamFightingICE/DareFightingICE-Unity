using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartController : MonoBehaviour
{
    public TMP_Text p1Control;
    public TMP_Text p2Control;
    public Button playBtn;
    private int[] _repeatCount = new[] { 1, 3, 5, 10, 100, 500, 1000 };

    // Current control types for each player
    private ControlType p1CurrentControl = ControlType.GRPC;
    private ControlType p2CurrentControl = ControlType.GRPC;

    void Start()
    {

    }

    void Update()
    {
        playBtn.interactable = CheckCondition();
        UpdateControlTexts();
    }

    public bool CheckCondition()
    {
        if (p1CurrentControl != ControlType.KEYBOARD && GrpcServer.Instance.GetPlayer(true).IsCancelled)
        {
            return false;
        }
        else if (p2CurrentControl != ControlType.KEYBOARD && GrpcServer.Instance.GetPlayer(false).IsCancelled)
        {
            return false;
        }
        return true;
    }
    
    // Called to cycle through control types for each player
    public void SelectControl(int player)
    {
        if (player == 1)
        {
            p1CurrentControl = GetNextControlType(p1CurrentControl);
            p1Control.text = p1CurrentControl.ToString();
        }
        else if (player == 2)
        {
            p2CurrentControl = GetNextControlType(p2CurrentControl);
            p2Control.text = p2CurrentControl.ToString();
        }

        UpdateControlTexts();
    }

    public void StartGame()
    {
        GameData _gameData = new GameData();
        GameDataManager.Instance.SetGameData(_gameData);
        GameSetting.Instance.SetCharacterData(p1CurrentControl, p2CurrentControl);
        SceneManager.LoadScene("Gameplay");
    }

    // Utility method to get the next control type, cycling through the enum
    private ControlType GetNextControlType(ControlType currentType)
    {
        int nextIndex = ((int)currentType + 1) % Enum.GetValues(typeof(ControlType)).Length;
        return (ControlType)nextIndex;
    }

    // Update control text for both players
    private void UpdateControlTexts()
    {
        p1Control.text = $"{ControlTypeUtil.GetString(p1CurrentControl)}";
        p2Control.text = $"{ControlTypeUtil.GetString(p2CurrentControl)}";

        if (p1CurrentControl == ControlType.GRPC)
        {
            p1Control.text += GrpcServer.Instance.GetPlayer(true).IsCancelled ? " (Disconnected)" : " (Connected)";
        }
        if (p2CurrentControl == ControlType.GRPC)
        {
            p2Control.text += GrpcServer.Instance.GetPlayer(false).IsCancelled ? " (Disconnected)" : " (Connected)";
        }
    }
    public void Back() {
        SceneManager.LoadScene("Launch");
    }
}