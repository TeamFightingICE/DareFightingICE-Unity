using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using DareFightingICE.Grpc.Proto;

public class GrpcServer : Singleton<GrpcServer>, IServer
{
    private Server server;
    private readonly GrpcPlayer[] players;
    public bool IsOpen { get; set; }
    public GrpcServer() {
        this.players = new GrpcPlayer[] { new(true), new(false) };
        this.IsOpen = false;
    }
    void OnApplicationQuit() {
        if (this.IsOpen) {
            StopServer();
            this.IsOpen = false;
            Debug.Log("gRPC server stopped");
        }
    }
    public void StartServer()
    {
        if (this.server == null) {
            int port = FlagSetting.Instance.port;
            server = new Server
            {
                Services = { Service.BindService(new GrpcServiceImpl()) },
                Ports = { new ServerPort("127.0.0.1", port, ServerCredentials.Insecure) }
            };
            
            try {
                server.Start();
                this.IsOpen = true;
                Debug.Log("gRPC server started, listening on " + port);
            } catch (Exception e) {
                Debug.Log("gRPC server failed to start: " + e.Message);
            }
        }
    }
    public void StopServer()
    {
        this.IsOpen = false;
        server?.ShutdownAsync().Wait();
    }
    public IPlayer GetPlayer(bool playerNumber)
    {
        return this.players[playerNumber ? 0 : 1];
    }
    public IPlayer GetPlayerWithUniqueId(string UUID)
    {
        for (int i = 0; i < this.players.Length; i++)
        {
            if (this.players[i].PlayerUUID.ToString().Equals(UUID))
            {
                return this.players[i];
            }
        }
        return null;
    }
}
