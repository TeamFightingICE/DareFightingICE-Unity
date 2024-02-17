using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AIController : MonoBehaviour
{
    public ZenCharacterController characterController;
    private IAIInterface ai;
    private GrpcPlayer grpcPlayer;
    private LinkedList<FrameData> frameDatas;
    private AudioData audioData;
    private ScreenData screenData;
    private bool isPlayerOne;
    
    public void Awake()
    {
        this.frameDatas = new LinkedList<FrameData>();
    }

    public void Initialize(GameData gameData, bool isPlayerOne)
    {
        this.isPlayerOne = isPlayerOne;
        this.grpcPlayer = GrpcServer.Instance.GetPlayer(isPlayerOne);
        this.grpcPlayer?.OnInitialize(gameData);
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
        this.audioData = audioData;
        this.ai?.GetAudioData(audioData);
    }

    public void SetScreenData(ScreenData screenData)
    {
        this.screenData = screenData;
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

        this.grpcPlayer?.SetInformation(true, frameData, audioData, screenData);
        this.grpcPlayer?.OnGameUpdate();

        this.ai?.GetInformation(frameData);
        this.ai?.Processing();
    }

    public Key Input()
    {
        if (ai != null) {
            return this.ai.Input();
        }
        return InputManager.Instance.GetInput(isPlayerOne);
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
        this.grpcPlayer?.OnRoundEnd(roundResult);
        this.ai?.RoundEnd(roundResult);
    }
    
    void Update()
    {
        SetFrameData(FrameDataManager.Instance.GetFrameData());
        SetAudioData(AudioDataManager.Instance.GetAudioData());
        SetScreenData(new ScreenData());
        Processing();
    }

    
    void ApplyInputToCharacter(Key input)
    {
        // Convert AI input into character actions
        // This is where you'll control the character based on AI decisions
    }
}