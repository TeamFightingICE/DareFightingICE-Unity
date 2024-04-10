using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public ZenCharacterController characterController;
    private bool isPlayerOne;
    private IAIInterface ai;
    private LinkedList<FrameData> frameDatas;
    
    public void Awake()
    {
        this.frameDatas = new LinkedList<FrameData>();
    }

    private IAIInterface GetGrpcAI(bool isPlayerOne)
    {
        if (FlagSetting.Instance.grpc)
        {
            return GrpcServer.Instance.GetPlayer(isPlayerOne);
        }
        else if (FlagSetting.Instance.socket)
        {
            return SocketServer.Instance.GetPlayer(isPlayerOne);
        }
        else
        {
            return new Sandbox();
        }
    }

    public void Initialize(GameData gameData, bool isPlayerOne)
    {
        this.isPlayerOne = isPlayerOne;
        this.ai = GameSetting.Instance.GetControlType(isPlayerOne) switch
        {
            ControlType.LOCAL_AI => LocalAIUtil.GetAIInterface(GameSetting.Instance.GetAIName(isPlayerOne)),
            ControlType.GRPC => GetGrpcAI(isPlayerOne),
            _ => new Sandbox(),
        };
        this.ai.Initialize(new GameData(gameData), isPlayerOne);
    }
    public void InitRound() {
        this.Clear();
    }

    public void SetFrameData(FrameData frameData) {
        var newFrameData = frameData ?? new FrameData();
        this.frameDatas.AddLast(newFrameData);

        while (frameDatas.Count > GameSetting.Instance.FrameDelay) {
            frameDatas.RemoveFirst();
        }

        if (GameSetting.Instance.IsNonDelay[isPlayerOne ? 0 : 1]) {
            this.ai.GetNonDelayFrameData(new FrameData(newFrameData));
        }
    }

    public void SetAudioData(AudioData audioData)
    {
        this.ai.GetAudioData(new AudioData(audioData));
    }

    public void SetScreenData(ScreenData screenData)
    {
        if (GameSetting.Instance.IsBlind[isPlayerOne ? 0 : 1] || this.ai.IsBlind()) {
            screenData = new ScreenData();
        }
        this.ai.GetScreenData(screenData);
    }
    
    public void Processing()
    {
        this.ai.Processing();
    }

    public Key Input()
    {
        return this.ai.Input();
    }

    public void Clear() {
        if (this.frameDatas != null)
        {
            this.frameDatas.Clear();

            while (frameDatas.Count < GameSetting.Instance.FrameDelay) {
                frameDatas.AddLast(new FrameData());
            }
        }
    }

    public void RoundEnd(RoundResult roundResult)
    {
        this.ai.RoundEnd(roundResult);
    }
    
    void Update()
    {
        SetFrameData(FrameDataManager.Instance.GetFrameData());
        SetAudioData(AudioDataManager.Instance.GetAudioData());
        SetScreenData(ScreenDataManager.Instance.GetScreenData());

        FrameData frameData;
        if (frameDatas.Count > 0) {
            frameData = frameDatas.First.Value;
            frameDatas.RemoveFirst();
        } else {
            frameData = new FrameData();
        }

        bool isControl = frameDatas.Last.Value.GetCharacter(isPlayerOne).Control;
        if (GameSetting.Instance.IsBlind[isPlayerOne ? 0 : 1] || this.ai.IsBlind()) {
            frameData.RemoveVisualData();
        }
        this.ai.GetInformation(new FrameData(frameData), isControl);

        Processing();
    }

    public void Close()
    {
        if (!GameSetting.Instance.IsKeepConnection[isPlayerOne ? 0 : 1]) {
            this.ai.Close();
        }
    }
    
}