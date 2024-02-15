using DareFightingICE.Grpc.Proto;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class GrpcPlayer
{
    public UniqueId PlayerUUID {  get; set; }
    public bool IsCancelled { get; set; }
    public PlayerGameState CurrentState { get; set; }

    private bool playerNumber;
    private string playerName;
    private bool isBlind;

    private bool isControl;
    private FrameData frameData;
    private AudioData audioData;
    private ScreenData screenData;
    // private FrameData nonDelayFrameData;

    private bool waitFlag;

    public GrpcPlayer(bool playerNumber)
    {
        this.PlayerUUID = new UniqueId();
        this.IsCancelled = true;

        this.playerNumber = playerNumber;
        this.waitFlag = false;

        this.isControl = false;
    }
    public void InitializeRPC(InitializeRequest request)
    {
        this.playerName = request.PlayerName;
        this.isBlind = request.IsBlind;
        this.IsCancelled = false;
    }
    public void onCancel()
    {
        this.IsCancelled = true;
    }
    public bool IsGameStarted()
    {
        return (this.frameData != null && !this.frameData.EmptyFlag && this.frameData.CurrentFrameNumber > 0);
    }
    public void OnInput(PlayerInput request)
    {
        if (this.IsGameStarted())
        {
            Key key = GrpcUtil.FromGrpcKey(request.InputKey);
            InputManager.Instance.SetInput(playerNumber, key);
        }

        if (this.waitFlag)
        {
            this.waitFlag = false;
        }
    }
    public void OnInitialize(GameData gameData)
    {
        this.CurrentState = new PlayerGameState
        {
            StateFlag = GrpcFlag.Initialize,
            GameData = gameData.ToProto()
        };
    }
    public void SetInformation(bool isControl, FrameData frameData, AudioData audioData, ScreenData screenData)
    {
        this.isControl = isControl;
        this.frameData = new FrameData(frameData);
        this.audioData = new AudioData(audioData);
        this.screenData = new ScreenData(screenData);
    }
    public void OnGameUpdate()
    {
        if (!isBlind)
        {
            frameData.RemoveVisualData();
        }
        PlayerGameState newState = new PlayerGameState
        {
            StateFlag = GrpcFlag.Processing,
            IsControl = isControl,
            FrameData = frameData.ToProto(),
        };
        this.CurrentState = newState;
        
    }
    public void OnRoundEnd(RoundResult roundResult)
    {
        this.CurrentState = new PlayerGameState
        {
            StateFlag = GrpcFlag.RoundEnd,
            RoundResult = roundResult.ToProto()
        };
    }
}
