using DareFightingICE.Grpc.Proto;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GrpcPlayer
{
    public UniqueId PlayerUUID {  get; set; }
    public bool IsCancelled { get; set; }
    public PlayerGameState CurrentState { get; set; }

    public bool PlayerNumber { get; set; }
    private string playerName;
    private bool isBlind;

    private bool isControl;
    private FrameData frameData;
    private AudioData audioData;
    private ScreenData screenData;
    // private FrameData nonDelayFrameData;

    private bool notifyCompleted;
    private IServerStreamWriter<PlayerGameState> responseStream;
    private ServerCallContext serverCallContext;

    public GrpcPlayer(bool playerNumber)
    {
        this.PlayerUUID = new UniqueId();
        this.IsCancelled = true;

        this.PlayerNumber = playerNumber;
        this.notifyCompleted = false;

        this.isControl = false;
    }
    public void InitializeRPC(InitializeRequest request)
    {
        this.playerName = request.PlayerName;
        this.isBlind = request.IsBlind;
        this.IsCancelled = false;
    }
    public async Task ParticipateRPC(IServerStreamWriter<PlayerGameState> responseStream, ServerCallContext context) {
        context.CancellationToken.Register(() => { this.OnCancel(); }, false);

        this.notifyCompleted = false;
        this.responseStream = responseStream;
        this.serverCallContext = context;

        await Task.Run(() => {
            while (!context.CancellationToken.IsCancellationRequested && GrpcServer.Instance.IsOpen && !this.notifyCompleted)
            {
                
            }
        });

        this.OnCancel();
    }
    public void OnCancel()
    {
        this.IsCancelled = true;
        this.responseStream = null;
        this.serverCallContext = null;
    }
    public bool IsGameStarted
    {
        get
        {
            return this.frameData != null && !this.frameData.EmptyFlag && this.frameData.CurrentFrameNumber > 0;
        }
    }
    public void OnInput(PlayerInput request)
    {
        if (this.IsGameStarted)
        {
            Key key = GrpcUtil.FromGrpcKey(request.InputKey);
            InputManager.Instance.SetInput(PlayerNumber, key);
        }
    }
    public void SetInformation(bool isControl, FrameData frameData, AudioData audioData, ScreenData screenData)
    {
        this.isControl = isControl;
        this.frameData = new FrameData(frameData);
        this.audioData = new AudioData(audioData);
        this.screenData = new ScreenData(screenData);
    }
    public async void OnInitialize(GameData gameData)
    {
        if (this.IsCancelled) return;

        this.CurrentState = new PlayerGameState
        {
            StateFlag = GrpcFlag.Initialize,
            GameData = gameData.ToProto()
        };
        await this.responseStream.WriteAsync(this.CurrentState);
    }
    public async void OnGameUpdate()
    {
        if (!this.IsGameStarted || this.IsCancelled) return;

        if (!isBlind)
        {
            frameData.RemoveVisualData();
        }
        this.CurrentState = new PlayerGameState
        {
            StateFlag = GrpcFlag.Processing,
            IsControl = isControl,
            FrameData = frameData.ToProto(),
            AudioData = audioData.ToProto(),
            ScreenData = screenData.ToProto()
        };
        await this.responseStream.WriteAsync(this.CurrentState);
    }
    public async void OnRoundEnd(RoundResult roundResult)
    {
        if (this.IsCancelled) return;

        bool isGameEnd = roundResult.CurrentRound >= GameSetting.Instance.RoundLimit;
        this.CurrentState = new PlayerGameState
        {
            StateFlag = isGameEnd ? GrpcFlag.GameEnd : GrpcFlag.RoundEnd,
            RoundResult = roundResult.ToProto()
        };
        await this.responseStream.WriteAsync(this.CurrentState);

        if (isGameEnd)
        {
            this.notifyCompleted = true;
        }
    }
}
