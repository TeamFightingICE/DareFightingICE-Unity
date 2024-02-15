using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GrpcAutoController : MonoBehaviour
{
    private GrpcServer server;

    void Start()
    {
        server = GrpcServer.Instance;
    }

    void Update()
    {
        if (server.RunFlag) {
            server.RunFlag = false;
            GameDataManager.Instance.SetGameData(server.GameData);
            GameSetting.Instance.SetCharacterData(ControlType.GRPC, ControlType.GRPC);
            SceneManager.LoadScene("Gameplay");
        }
    }

    public void Cancel() {
        SceneManager.LoadScene("Launch");
    }
}
