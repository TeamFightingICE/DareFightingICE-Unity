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

public class SocketServer : Singleton<SocketServer>
{
    private Socket server;
    private SocketPlayer[] players;
    private Thread processingThread;
    public bool IsOpen { get; set; }
    public SocketServer() {
        this.players = new SocketPlayer[] { new(true), new(false) };
        this.IsOpen = false;
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
    public void StopSocketServer()
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
    void OnApplicationQuit() {
        if (this.IsOpen) {
            StopSocketServer();
            this.IsOpen = false;
            Debug.Log("Socket server stopped");
        }
    }
    public SocketPlayer GetPlayer(bool playerNumber)
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
            if (data[0] == 0)
            {
                players[0].SetSocketClient(client);
                Debug.Log("Player 1 connected");
            }
            else if (data[0] == 1)
            {
                players[1].SetSocketClient(client);
                Debug.Log("Player 2 connected");
            }
            else if (data[0] == 2)
            {
                OnRunGame(client);
            }
        }
    }
    void OnRunGame(Socket client)
    {
        byte[] byteData = new byte[256];
        client.Receive(byteData);
        Debug.Log("Incoming runGame request");
        string requestJsonStr = Encoding.UTF8.GetString(byteData);
        SocketRunGameRequest request = JsonSerializer.Deserialize<SocketRunGameRequest>(
            requestJsonStr[..requestJsonStr.IndexOf('\0')],
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            }
        );

        int statusCode;
        string responseMessage;

        if (!FlagSetting.Instance.grpcAuto)
        {
            statusCode = 1;  // Failed
            responseMessage = "The game is not in gRPC auto mode.";
        }
        else if (!FlagSetting.Instance.grpcAutoReady)
        {
            statusCode = 1;  // Failed
            responseMessage = "The game is not ready for running the game.";
        }
        else
        {
            DataManager.Instance.GameData = new GameData(
                new string[] { request.Character_1, request.Character_2 },
                new string[] { request.Player_1, request.Player_2 },
                request.GameNumber
            );
            DataManager.Instance.RunFlag = true;

            statusCode = 0;  // Success
            responseMessage = "Success";
        }

        SocketRunGameResponse response = new SocketRunGameResponse
        {
            StatusCode = statusCode,
            ResponseMessage = responseMessage
        };
        string responseJsonStr = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        });
        byte[] responseByteData = Encoding.UTF8.GetBytes(responseJsonStr);
        client.Send(responseByteData);

        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }
}
