using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ControlType
{
    KEYBOARD,
    AI,
    GRPC
}

public class StartController : MonoBehaviour
{
    public TMP_Text p1Control;
    public TMP_Text p2Control;
    public Button playBtn;
    private int[] _repeatCount = new[] { 1, 3, 5, 10, 100, 500, 1000 };

    // Current control types for each player
    private ControlType p1CurrentControl = ControlType.KEYBOARD;
    private ControlType p2CurrentControl = ControlType.KEYBOARD;

    void Start()
    {
        // Initial display of control types
        UpdateControlTexts();
    }

    void Update()
    {
        //CheckCondition();
    }

    public void CheckCondition()
    {
        if (false)
        {
            playBtn.interactable = false;
        }
        else
        {
            playBtn.interactable = true;
        }
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
        GameSetting.Instance.SetCharacterData(p1CurrentControl,p2CurrentControl);
        SceneManager.LoadScene("Gameplay");
    }

    // Utility method to get the next control type, cycling through the enum
    private ControlType GetNextControlType(ControlType currentType)
    {
        int nextIndex = ((int)currentType + 1) % System.Enum.GetValues(typeof(ControlType)).Length;
        return (ControlType)nextIndex;
    }

    // Update control text for both players
    private void UpdateControlTexts()
    {
        p1Control.text = $"{p1CurrentControl}";
        p2Control.text = $"{p2CurrentControl}";
    }
}