using Grpc.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRPCServer : Singleton<GRPCServer>
{
    private Server server;
    private GrpcPlayer[] players;
    public GRPCServer() {
        this.players = new GrpcPlayer[] { new GrpcPlayer(), new GrpcPlayer() };
    }
    public void StartGrpcServer()
    {
        server = new Server();
        server.Start();
        Debug.Log("gRPC server started.");
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
