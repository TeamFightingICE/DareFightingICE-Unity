using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.Json;
using System.Data;
using System.Runtime.Remoting.Channels;
using DareFightingICE.Grpc.Proto;
using Google.Protobuf;

public class SocketServer : Singleton<SocketServer>, IServer
{
    private Socket server;
    private readonly SocketPlayer[] players;
    private Thread processingThread;
    public bool IsOpen { get; set; }

    public SocketServer() {
        this.players = new SocketPlayer[] { new(true), new(false) };
        this.IsOpen = false;
    }

    void OnApplicationQuit() {
        if (this.IsOpen) {
            StopServer();
            this.IsOpen = false;
            Debug.Log("Socket server stopped");
        }
    }

    public void StartServer() {
        if (this.server == null)
        {
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            int port = FlagSetting.Instance.port;
            IPEndPoint localEndPoint = new(ipAddr, port);
            server = new(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            try {
                server.Bind(localEndPoint);
                server.Listen(10);
                this.IsOpen = true;
                Debug.Log("Socket server started, listening on " + port);

                processingThread = new(MainProcess);
                processingThread.Start();
            } catch (Exception e) {
                Debug.Log("Socket server failed to start: " + e.Message);
            }
        }
    }

    public void StopServer()
    {
        if (server != null)
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].Close();
            }
            processingThread.Abort();
            server.Close();
            server = null;
            this.IsOpen = false;
        }
    }

    public IPlayer GetPlayer(bool playerNumber)
    {
        return this.players[playerNumber ? 0 : 1];
    }

    void MainProcess()
    {
        while (true)
        {
            Socket client = server.Accept();
            byte[] data = new byte[1];
            client.Receive(data);
            if (data[0] == 1)
            {
                byte[] byteData = RecvData(client);
                InitializeRequest request = InitializeRequest.Parser.ParseFrom(byteData);
                players[request.PlayerNumber ? 0 : 1].InitializeSocket(client, request);
            }
            else if (data[0] == 2)
            {
                OnRunGame(client);
            }
        }
    }

    void OnRunGame(Socket client)
    {
        RunGameRequest request = RunGameRequest.Parser.ParseFrom(RecvData(client));

        GrpcStatusCode statusCode;
        string responseMessage;

        if (!FlagSetting.Instance.autoMode)
        {
            statusCode = GrpcStatusCode.Failed;
            responseMessage = "The game does not enable auto mode.";
        }
        else if (!FlagSetting.Instance.autoModeReady)
        {
            statusCode = GrpcStatusCode.Failed;
            responseMessage = "The game is not ready for running the game.";
        }
        else
        {
            DataManager.Instance.GameData = new GameData(
                new string[] { request.Character1, request.Character2 },
                new string[] { request.Player1, request.Player2 },
                request.GameNumber
            );
            DataManager.Instance.RunFlag = true;

            statusCode = GrpcStatusCode.Success;
            responseMessage = "Success";
        }

        RunGameResponse response = new()
        {
            StatusCode = statusCode,
            ResponseMessage = responseMessage
        };
        SendData(client, response.ToByteArray());

        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    public static byte[] RecvData(Socket client, int n = -1)
    {
        if (n == -1)
        {
            byte[] headerData = new byte[4];
            client.Receive(headerData);
            n = BitConverter.ToInt32(headerData, 0);
        }

        byte[] byteData = new byte[n];

        if (n == 0)
        {
            return byteData;
        }

        client.Receive(byteData);
        return byteData;
    }

    public static void SendData(Socket client, byte[] byteData, bool withHeader = true)
    {
        try
        {
            if (byteData == null)
            {
                client.Send(new byte[] { 0, 0, 0, 0 });  // Null Data
                return;
            }

            if (withHeader)
            {
                int dataLength = byteData.Length;
                byte[] lengthAsBytes = BitConverter.GetBytes(dataLength);
                byte[] fixedLengthAsBytes = new byte[4];
                Array.Copy(lengthAsBytes, fixedLengthAsBytes, lengthAsBytes.Length);

                client.Send(fixedLengthAsBytes);
            }

            client.Send(byteData);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
