using DareFightingICE.Grpc.Proto;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

public class GrpcPlayer : IPlayer
{
    public UniqueId PlayerUUID {  get; set; }
    public bool IsCancelled { get; set; }
    public bool PlayerNumber { get; set; }
    private string PlayerName { get; set; }
    private bool blind;

    private bool isControl;
    private FrameData frameData;
    private AudioData audioData;
    private ScreenData screenData;
    private FrameData nonDelayFrameData;
    private Key input;

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
        this.frameData = new FrameData();
        this.audioData = new AudioData();
        this.screenData = new ScreenData();
        this.nonDelayFrameData = new FrameData();
        this.input = new Key();
    }

    public void InitializeRPC(InitializeRequest request)
    {
        this.PlayerName = request.PlayerName;
        this.blind = request.IsBlind;
        this.IsCancelled = false;
    }

    public async Task ParticipateRPC(IServerStreamWriter<PlayerGameState> responseStream, ServerCallContext context) {
        context.CancellationToken.Register(() => { this.Close(); }, false);

        this.notifyCompleted = false;
        this.responseStream = responseStream;
        this.serverCallContext = context;

        await Task.Run(() => {
            while (!context.CancellationToken.IsCancellationRequested && GrpcServer.Instance.IsOpen && !this.notifyCompleted)
            {
                
            }
        });
    }
    
    public void OnInput(PlayerInput request)
    {
        if (this.IsGameStarted)
        {
            this.input = GrpcUtil.FromGrpcKey(request.InputKey);
        }
    }

    public bool IsGameStarted
    {
        get => this.frameData != null && !this.frameData.EmptyFlag && this.frameData.CurrentFrameNumber >= 0;
    }

    public bool IsBlind()
    {
        return blind;
    }

    public void Initialize(GameData gameData, bool playerNumber)
    {
        if (this.IsCancelled) return;

        var newState = new PlayerGameState
        {
            StateFlag = GrpcFlag.Initialize,
            GameData = gameData.ToProto()
        };
        this.responseStream.WriteAsync(newState).Wait();
    }

    public void GetNonDelayFrameData(FrameData frameData)
    {
        this.nonDelayFrameData = frameData;
    }

    public void GetInformation(FrameData frameData, bool isControl)
    {
        this.frameData = frameData;
        this.isControl = isControl;
    }

    public void GetScreenData(ScreenData screenData)
    {
        this.screenData = screenData;
    }

    public void GetAudioData(AudioData audioData)
    {
        this.audioData = audioData;
    }

    public void Processing()
    {
        if (!this.IsGameStarted || this.IsCancelled) return;

        var newState = new PlayerGameState
        {
            StateFlag = GrpcFlag.Processing,
            IsControl = isControl,
            FrameData = frameData.ToProto(),
            AudioData = audioData.ToProto(),
            ScreenData = screenData.ToProto(),
            NonDelayFrameData = nonDelayFrameData.ToProto(),
        };
        this.responseStream.WriteAsync(newState).Wait();
    }

    public Key Input()
    {
        return this.input;
    }

    public void RoundEnd(RoundResult roundResult)
    {
        if (this.IsCancelled) return;

        bool isGameEnd = roundResult.CurrentRound >= GameSetting.Instance.RoundLimit;
        var newState = new PlayerGameState
        {
            StateFlag = isGameEnd ? GrpcFlag.GameEnd : GrpcFlag.RoundEnd,
            RoundResult = roundResult.ToProto(),
        };
        this.responseStream.WriteAsync(newState).Wait();
    }
    public void Close()
    {
        this.notifyCompleted = true;
        this.IsCancelled = true;
        this.responseStream = null;
        this.serverCallContext = null;
    }
}
