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
            GameDataManager.Instance.SetGameData(GrpcServer.Instance.GameData);
            GameSetting.Instance.SetCharacterData(ControlType.GRPC, ControlType.GRPC);
            SceneManager.LoadScene("Gameplay");
        }
    }

    public void Cancel() {
        FlagSetting.Instance.grpcAuto = false;
        GameSetting.Instance.SetRunWithGrpcAuto(false);
        SceneManager.LoadScene("Launch");
    }
}
