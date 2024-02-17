using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GrpcAutoController : MonoBehaviour
{
    void Start()
    {
        FlagSetting.Instance.grpcAuto = true;
        GameSetting.Instance.SetRunWithGrpcAuto(true);
    }

    void Update()
    {
        if (GrpcServer.Instance.RunFlag) {
            GrpcServer.Instance.RunFlag = false;

            var gameData = GrpcServer.Instance.GameData;
            var p1ControlType = LocalAIUtil.IsAIExist(gameData.AiNames[0]) ? ControlType.AI : ControlType.GRPC;
            var p2ControlType = LocalAIUtil.IsAIExist(gameData.AiNames[1]) ? ControlType.AI : ControlType.GRPC;

            GameDataManager.Instance.SetGameData(GrpcServer.Instance.GameData);
            GameSetting.Instance.SetAIName(gameData.AiNames[0], gameData.AiNames[1]);
            GameSetting.Instance.SetCharacterControlType(p1ControlType, p2ControlType);
            SceneManager.LoadScene("Gameplay");
        }
    }

    public void Cancel() {
        FlagSetting.Instance.grpcAuto = false;
        GameSetting.Instance.SetRunWithGrpcAuto(false);
        SceneManager.LoadScene("Launch");
    }
}
