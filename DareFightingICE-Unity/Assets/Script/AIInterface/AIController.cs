using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AIController : MonoBehaviour
{
    public ZenCharacterController characterController;
    private IAIInterface ai;
    private LinkedList<FrameData> frameDatas;
    
    public void Awake()
    {
        this.frameDatas = new LinkedList<FrameData>();
    }

    public void Initialize(GameData gameData, bool isPlayerOne)
    {
        this.ai = GameSetting.Instance.GetControlType(isPlayerOne) switch
        {
            ControlType.AI => LocalAIUtil.GetAIInterface(GameSetting.Instance.GetAIName(isPlayerOne)),
            ControlType.GRPC => GrpcServer.Instance.GetPlayer(isPlayerOne),
            _ => new Sandbox(),
        };
        this.ai?.Initialize(gameData, isPlayerOne);
    }
    public void InitRound() {
        this.Clear();
    }

    public void SetFrameData(FrameData frameData) {
        this.frameDatas.AddLast(frameData ?? new FrameData());

        while (frameDatas.Count > GameSetting.Instance.FrameDelay) {
            frameDatas.RemoveFirst();
        }
    }

    public void SetAudioData(AudioData audioData)
    {
        this.ai?.GetAudioData(audioData);
    }

    public void SetScreenData(ScreenData screenData)
    {
        this.ai?.GetScreenData(screenData);
    }
    
    public void Processing()
    {
        FrameData frameData;
        if (frameDatas.Count > 0) {
            frameData = frameDatas.First.Value;
            frameDatas.RemoveFirst();
        } else {
            frameData = new FrameData();
        }

        this.ai?.GetInformation(frameData);
        this.ai?.Processing();
    }

    public Key Input()
    {
        return this.ai?.Input();
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

    public void Close()
    {
        this.ai?.Close();
    }

    public void RoundEnd(RoundResult roundResult)
    {
        this.ai?.RoundEnd(roundResult);
    }
    
    void Update()
    {
        SetFrameData(FrameDataManager.Instance.GetFrameData());
        SetAudioData(AudioDataManager.Instance.GetAudioData());
        SetScreenData(new ScreenData());
        Processing();
    }
    
}