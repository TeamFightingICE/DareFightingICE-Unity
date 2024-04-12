using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Google.Protobuf;
using DareFightingICE.Grpc.Proto;
using UnityEngine;

public class SocketPlayer : IPlayer
{
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
    private Socket socketClient;

    public SocketPlayer(bool playerNumber)
    {
        this.IsCancelled = true;

        this.PlayerNumber = playerNumber;
        this.notifyCompleted = false;

        this.isControl = false;
        this.input = new Key();
    }
    
    public void InitializeSocket(Socket socket, InitializeRequest request)
    {
        this.socketClient = socket;
        this.IsCancelled = false;

        this.PlayerName = request.PlayerName;
        this.blind = request.IsBlind;
    }

    public bool IsGameStarted
    {
        get => this.frameData != null && !this.frameData.EmptyFlag && this.frameData.CurrentFrameNumber >= 0;
    }

    public bool IsBlind()
    {
        return blind;
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

    public Key Input()
    {
        return this.input;
    }

    public void Initialize(GameData gameData, bool playerNumber)
    {
        if (this.IsCancelled) return;

        var newState = new PlayerGameState
        {
            StateFlag = GrpcFlag.Initialize,
            GameData = gameData.ToProto()
        };

        this.socketClient.Send(new byte[] { 1 });  // 1: Processing
        SocketServer.SendData(this.socketClient, newState.ToByteArray());
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
        };

        if (screenData != null)
        {
            newState.ScreenData = screenData.ToProto();
        }

        if (nonDelayFrameData != null)
        {
            newState.NonDelayFrameData = nonDelayFrameData.ToProto();
        }

        this.socketClient.Send(new byte[] { 1 });  // 1: Processing
        SocketServer.SendData(this.socketClient, newState.ToByteArray());
        
        WaitingForAIInput();
    }
    
    private void WaitingForAIInput()
    {
        byte[] byteData = SocketServer.RecvData(this.socketClient);
        GrpcKey inputKey = GrpcKey.Parser.ParseFrom(byteData);
        
        this.input = new Key
        {
            A = inputKey.A,
            B = inputKey.B,
            C = inputKey.C,
            U = inputKey.U,
            D = inputKey.D,
            L = inputKey.L,
            R = inputKey.R,
        };
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

        this.socketClient.Send(new byte[] { 1 });  // 1: Processing
        SocketServer.SendData(this.socketClient, newState.ToByteArray());
    }

    public void Close()
    {
        if (this.socketClient != null) {
            this.socketClient.Send(new byte[] { 0 });  // 0: Close
            this.socketClient.Close();
        }

        this.notifyCompleted = true;
        this.IsCancelled = true;
        this.socketClient = null;
    }
}
