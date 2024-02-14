using Grpc.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DareFightingICE.Grpc.Proto;

public class GrpcServer : Singleton<GrpcServer>
{
    private Server server;
    private GrpcPlayer[] players;
    public GrpcServer() {
        this.players = new GrpcPlayer[] { new GrpcPlayer(true), new GrpcPlayer(false) };
    }
    public void StartGrpcServer()
    {
        int port = 50051;
        server = new Server
        {
            Services = { Service.BindService(new ServiceImpl()) },
            Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
        };
        server.Start();
        Debug.Log("Server started, listening on " + port);
    }
    public void StopGrpcServer()
    {
        if (server != null)
        {
            server.ShutdownTask.Wait();
        }
    }
    public GrpcPlayer GetPlayer(bool playerNumber)
    {
        return this.players[playerNumber ? 0 : 1];
    }
    public GrpcPlayer GetPlayerWithUniqueId(string UUID)
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
