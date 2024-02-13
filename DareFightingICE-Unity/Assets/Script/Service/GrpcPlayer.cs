using DareFightingICE.Grpc.Proto;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEditor.IMGUI.Controls;
using UnityEngine.UIElements;

public class GrpcPlayer
{
    public UniqueId PlayerUUID {  get; set; }
    private bool isCancelled;

    private bool playerNumber;
    private string playerName;
    private bool isBlind;

    private bool isControl;
    private FrameData frameData;
    private AudioData audioData;
    private ScreenData screenData;
    private FrameData nonDelayFrameData;

    private IServerStreamWriter<PlayerGameState> responseStream;
    private ServerCallContext serverCallContext;
    private bool waitFlag;

    public GrpcPlayer()
    {
        this.PlayerUUID = new UniqueId();
        this.isCancelled = true;

        this.waitFlag = false;

        this.isControl = false;
    }

    public void InitializeRPC(InitializeRequest request)
    {
        this.playerName = request.PlayerName;
        this.isBlind = request.IsBlind;
    }
    public void ParticipateRPC(IServerStreamWriter<PlayerGameState> responseStream, ServerCallContext serverCallContext)
    {
        if (!this.isCancelled)
        {
            this.onCancel();
        }

        this.isCancelled = false;
        this.responseStream = responseStream;
        this.serverCallContext = serverCallContext;
    }
    public void onCancel()
    {
        this.isCancelled = true;
        this.responseStream = null;
        this.serverCallContext = null;
    }
    public bool IsGameStarted()
    {
        return true;
    }
    public void onInput(PlayerInput request)
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
    public void onInitialize(GameData gameData)
    {
        PlayerGameState state = new PlayerGameState();
        state.StateFlag = GrpcFlag.Initialize;
        this.onNext(state);
    }
    public void onGameUpdate()
    {
        PlayerGameState state = new PlayerGameState();
        state.StateFlag = GrpcFlag.Processing;
        this.onNext(state);
    }
    public void onRoundEnd()
    {
        PlayerGameState state = new PlayerGameState();
        state.StateFlag = GrpcFlag.RoundEnd;
        this.onNext(state);
    }
    public async void onNext(PlayerGameState state)
    {
        if (this.serverCallContext.CancellationToken.IsCancellationRequested)
        {
            this.onCancel();
            return;
        }
        await this.responseStream.WriteAsync(state);
    }
}
