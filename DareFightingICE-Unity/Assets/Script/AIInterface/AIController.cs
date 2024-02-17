using System;
using UnityEngine;


public class AIController : MonoBehaviour
{
    public ZenCharacterController characterController;

    private IAIInterface ai;
    private GrpcPlayer grpcPlayer;

    private GameData gameData;
    private FrameData frameData;
    private AudioData audioData;
    private ScreenData screenData;

    private bool isPlayerOne;
    
    public void Awake()
    {

    }

    public void Initialize(GameData gameData, bool isPlayerOne)
    {
        this.gameData = gameData;
        this.isPlayerOne = isPlayerOne;
        this.grpcPlayer = GrpcServer.Instance.GetPlayer(isPlayerOne);
        this.grpcPlayer?.OnInitialize(gameData);
        this.ai?.Initialize(gameData, isPlayerOne);
    }
    
    public void GetInformation(FrameData frameData)
    {
        this.frameData = frameData;
        this.ai?.GetInformation(frameData);
    }

    public void GetAudioData(AudioData audioData)
    {
        this.audioData = audioData;
        this.ai?.GetAudioData(audioData);
    }

    public void GetScreenData(ScreenData screenData)
    {
        this.screenData = screenData;
        this.ai?.GetScreenData(screenData);
    }
    
    public void Processing()
    {
        this.grpcPlayer?.SetInformation(true, frameData, audioData, screenData);
        this.grpcPlayer?.OnGameUpdate();
        this.ai?.Processing();
    }

    public Key Input()
    {
        if (ai != null) {
            return this.ai.Input();
        }
        return InputManager.Instance.GetInput(isPlayerOne);
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
        GetInformation(FrameDataManager.Instance.GetFrameData());
        GetAudioData(AudioDataManager.Instance.GetAudioData());
        GetScreenData(new ScreenData());
        Processing();
    }

    
    void ApplyInputToCharacter(Key input)
    {
        // Convert AI input into character actions
        // This is where you'll control the character based on AI decisions
    }
}