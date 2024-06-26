using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartController : MonoBehaviour
{
    public TMP_Text p1Control;
    public TMP_Text p2Control;
    public TMP_Text RepeatCountText;
    public Button playBtn;
    private int[] _repeatCount = new[] { 1, 3, 5, 10, 30, 50, 100 };

    private ControlType p1CurrentControl;
    private ControlType p2CurrentControl;

    public Button p1ControlBtn;
    public Button p2ControlBtn;

    private int CurrentRepeatCountIdx;

    void Start()
    {
        p1CurrentControl = ControlType.KEYBOARD;
        p2CurrentControl = ControlType.KEYBOARD;
        CurrentRepeatCountIdx = 0;
    }

    void Update()
    {
        playBtn.interactable = CheckCondition();
        UpdateControlTexts();
    }

    public bool CheckCondition()
    {
        if (p1CurrentControl == ControlType.EXTERNAL_AI && ServiceUtils.GetServerInstance().GetPlayer(true).IsCancelled)
        {
            return false;
        }
        else if (p2CurrentControl == ControlType.EXTERNAL_AI && ServiceUtils.GetServerInstance().GetPlayer(false).IsCancelled)
        {
            return false;
        }
        return true;
    }
    
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
    }

    public void StartGame()
    {
        GameData _gameData = new GameData();
        GameDataManager.Instance.SetGameData(_gameData);
        GameSetting.Instance.SetCharacterControlType(p1CurrentControl, p2CurrentControl);
        GameSetting.Instance.SetGameRepeatCount(_repeatCount[CurrentRepeatCountIdx]);
        SceneManager.LoadScene("StartingGamePlay");
    }

    private bool IsCancelled(bool isPlayerOne) {
        if (ServiceUtils.IsServerOpen() && ServiceUtils.GetServerInstance().GetPlayer(isPlayerOne) != null)
        {
            return ServiceUtils.GetServerInstance().GetPlayer(isPlayerOne).IsCancelled;
        }
        return true;
    }

    // Utility method to get the next control type, cycling through the enum
    private ControlType GetNextControlType(ControlType currentType, int offset)
    {
        int n_controlType = Enum.GetValues(typeof(ControlType)).Length;
        int nextIndex = ((int)currentType + offset + n_controlType) % n_controlType;
        return (ControlType)nextIndex;
    }

    // Update control text for both players
    private void UpdateControlTexts()
    {
        p1Control.text = $"{ControlTypeUtil.GetString(p1CurrentControl)}";
        p2Control.text = $"{ControlTypeUtil.GetString(p2CurrentControl)}";

        if (p1CurrentControl == ControlType.EXTERNAL_AI)
        {
            p1Control.text += IsCancelled(true) ? " (Disconnected)" : " (Connected)";
        }
        if (p2CurrentControl == ControlType.EXTERNAL_AI)
        {
            p2Control.text += IsCancelled(false) ? " (Disconnected)" : " (Connected)";
        }
    }
    public void Back() {
        SceneManager.LoadScene("Launch");
    }
}