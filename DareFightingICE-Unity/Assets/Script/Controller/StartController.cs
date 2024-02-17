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
    public TMP_Text RepeatCountText;
    public Button playBtn;
    private int[] _repeatCount = new[] { 1, 3, 5, 10, 30, 50, 100 };

    // Current control types for each player
    private ControlType p1CurrentControl;
    private ControlType p2CurrentControl;

    public Button p1ControlBtn;
    public Button p2ControlBtn;

    private int CurrentRepeatCountIdx = 0;

    void Start()
    {
        p1CurrentControl = ControlType.KEYBOARD;
        p2CurrentControl = ControlType.KEYBOARD;
    }

    void Update()
    {
        playBtn.interactable = CheckCondition();
        UpdateControlTexts();
    }

    public bool CheckCondition()
    {
        if (p1CurrentControl == ControlType.GRPC && GrpcServer.Instance.GetPlayer(true).IsCancelled)
        {
            return false;
        }
        else if (p2CurrentControl == ControlType.GRPC && GrpcServer.Instance.GetPlayer(false).IsCancelled)
        {
            return false;
        }
        return true;
    }
    
    // Called to cycle through control types for each player
    public void SelectControl(int player, int offset)
    {
        if (player == 1)
        {
            p1CurrentControl = GetNextControlType(p1CurrentControl, offset);
            p1Control.text = p1CurrentControl.ToString();
        }
        else if (player == 2)
        {
            p2CurrentControl = GetNextControlType(p2CurrentControl, offset);
            p2Control.text = p2CurrentControl.ToString();
        }

        UpdateControlTexts();
    }
    
    public void SelectRepeatCount(int offset) 
    {
        int n_repeatCount = _repeatCount.Length;
        CurrentRepeatCountIdx = (CurrentRepeatCountIdx + offset + n_repeatCount) % n_repeatCount;
        RepeatCountText.text = _repeatCount[CurrentRepeatCountIdx].ToString();
        GameSetting.Instance.GameRepeatCount = _repeatCount[CurrentRepeatCountIdx];
    }

    public void StartGame()
    {
        GameData _gameData = new GameData();
        GameDataManager.Instance.SetGameData(_gameData);
        GameSetting.Instance.SetAIName("MctsAi23i", "MctsAi23i");
        GameSetting.Instance.SetCharacterControlType(p1CurrentControl, p2CurrentControl);
        SceneManager.LoadScene("StartingGamePlay");
    }

    // Utility method to get the next control type, cycling through the enum
    private ControlType GetNextControlType(ControlType currentType, int offset)
    {
        int n_controlType = Enum.GetValues(typeof(ControlType)).Length;
        int nextIndex = ((int)currentType + offset + n_controlType) % n_controlType;
        if ((ControlType)nextIndex == ControlType.GRPC && !GrpcServer.Instance.IsOpen)
        {
            return GetNextControlType((ControlType)nextIndex, offset);
        }
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