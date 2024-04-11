using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GrpcAutoController : MonoBehaviour
{
    void Start()
    {
        FlagSetting.Instance.grpcAuto = true;
        FlagSetting.Instance.grpcAutoReady = true;
        DataManager.Instance.RunFlag = false;
        GameSetting.Instance.SetGameRepeatCount(1);
    }

    void Update()
    {
        if (DataManager.Instance.RunFlag) {
            DataManager.Instance.RunFlag = false;
            FlagSetting.Instance.grpcAutoReady = false;

            var gameData = DataManager.Instance.GameData;
            var p1ControlType = GetControlTypeByAIName(gameData.AiNames[0]);
            var p2ControlType = GetControlTypeByAIName(gameData.AiNames[1]);

            GameDataManager.Instance.SetGameData(gameData);
            GameSetting.Instance.SetAIName(gameData.AiNames[0], gameData.AiNames[1]);
            GameSetting.Instance.SetCharacterControlType(p1ControlType, p2ControlType);
            GameSetting.Instance.SetGameRepeatCount(gameData.RepeatCount);

            SceneManager.LoadScene("StartingGamePlay");
        }
    }

    private ControlType GetControlTypeByAIName(string aiName)
    {
        if (string.Equals(aiName, "Keyboard", StringComparison.OrdinalIgnoreCase))
        {
            return ControlType.KEYBOARD;
        }
        else if (LocalAIUtil.IsAIExist(aiName))
        {
            return ControlType.LOCAL_AI;
        }
        else
        {
            return ControlType.GRPC;
        }
    }

    public void Cancel() {
        FlagSetting.Instance.grpcAuto = false;
        FlagSetting.Instance.grpcAutoReady = false;

        SceneManager.LoadScene("Launch");
    }
}
