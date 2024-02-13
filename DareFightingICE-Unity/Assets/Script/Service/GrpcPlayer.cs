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
    public bool IsCancelled { get; set; }
    public PlayerGameState CurrentState { get; set; }

    private bool playerNumber;
    private string playerName;
    private bool isBlind;

    private bool isControl;
    private FrameData frameData;
    private AudioData audioData;
    private ScreenData screenData;
    private FrameData nonDelayFrameData;

    private bool waitFlag;

    public GrpcPlayer()
    {
        this.PlayerUUID = new UniqueId();
        this.IsCancelled = true;

        this.waitFlag = false;

        this.isControl = false;
    }
    public void InitializeRPC(InitializeRequest request)
    {
        this.playerName = request.PlayerName;
        this.isBlind = request.IsBlind;
    }
    public void onCancel()
    {
        this.IsCancelled = true;
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
        this.CurrentState = state;
    }
    public void onGameUpdate()
    {
        PlayerGameState state = new PlayerGameState();
        state.StateFlag = GrpcFlag.Processing;
        this.CurrentState = state;
    }
    public void onRoundEnd()
    {
        PlayerGameState state = new PlayerGameState();
        state.StateFlag = GrpcFlag.RoundEnd;
        this.CurrentState = state;
    }
}
