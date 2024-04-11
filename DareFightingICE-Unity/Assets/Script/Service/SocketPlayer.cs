using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;

public class SocketPlayer : IAIInterface
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
    private JsonSerializerOptions serializerOptions;

    public SocketPlayer(bool playerNumber)
    {
        this.IsCancelled = true;

        this.PlayerNumber = playerNumber;
        this.notifyCompleted = false;

        this.isControl = false;
        this.frameData = new FrameData();
        this.audioData = new AudioData();
        this.screenData = new ScreenData();
        this.nonDelayFrameData = new FrameData();
        this.input = new Key();

        this.serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };
    }
    
    public void SetSocketClient(Socket socket)
    {
        this.socketClient = socket;
        this.IsCancelled = false;
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

        string gameDataJsonStr = JsonSerializer.Serialize(gameData.ToProto(), serializerOptions);
        this.socketClient.Send(new byte[] { 1 });  // 1: Initialize
        SendDataToAI(gameDataJsonStr);  // GameData
        this.socketClient.Send(new byte[] { playerNumber ? (byte)1 : (byte)0 });  // PlayerNumber
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

    private void WaitingForAIInput()
    {
        byte[] headerData = new byte[4];
        this.socketClient.Receive(headerData);
        int dataLength = BitConverter.ToInt32(headerData, 0);
        byte[] byteData = new byte[dataLength];
        this.socketClient.Receive(byteData);
        string keyJsonStr = Encoding.UTF8.GetString(byteData);
        this.input = JsonSerializer.Deserialize<Key>(keyJsonStr);
    }

    public void Processing()
    {
        if (!this.IsGameStarted || this.IsCancelled) return;
        
        string frameDataJsonStr = JsonSerializer.Serialize(frameData.ToProto(), serializerOptions);
        string audioDataJsonStr = JsonSerializer.Serialize(audioData.ToSocket(), serializerOptions);
        string screenDataJsonStr = JsonSerializer.Serialize(screenData.ToSocket(), serializerOptions);

        this.socketClient.Send(new byte[] { 2 });  // 2: Processing
        this.socketClient.Send(new byte[] { isControl ? (byte)1 : (byte)0 });  // isControl
        SendDataToAI(frameDataJsonStr);
        SendDataToAI(audioDataJsonStr);
        SendDataToAI(screenDataJsonStr);
        
        WaitingForAIInput();
    }

    public void SendDataToAI(string jsonString)
    {
        try
        {
            byte[] byteData = Encoding.UTF8.GetBytes(jsonString);
            int dataLength = byteData.Length;
            byte[] lengthAsBytes = BitConverter.GetBytes(dataLength);
            byte[] fixedLengthAsBytes = new byte[4];
            Array.Copy(lengthAsBytes, fixedLengthAsBytes, lengthAsBytes.Length);

            this.socketClient.Send(fixedLengthAsBytes);
            this.socketClient.Send(byteData);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
            this.IsCancelled = true;
        }
    }

    public Key Input()
    {
        return this.input;
    }

    public void RoundEnd(RoundResult roundResult)
    {
        if (this.IsCancelled) return;

        bool isGameEnd = roundResult.CurrentRound >= GameSetting.Instance.RoundLimit;

        string roundResultJsonStr = JsonSerializer.Serialize(roundResult.ToProto(), serializerOptions);
        this.socketClient.Send(new byte[] { isGameEnd ? (byte)4 : (byte)3 });  // 3: RoundEnd, 4: GameEnd
        SendDataToAI(roundResultJsonStr);
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
