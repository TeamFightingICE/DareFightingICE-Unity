using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DareFightingICE.Grpc.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

public class ServiceImpl : Service.ServiceBase
{
    private GrpcServer server;
    public ServiceImpl()
    {
        this.server = GrpcServer.Instance;
    }
    public override Task<InitializeResponse> Initialize(InitializeRequest request, ServerCallContext context)
    {
        UnityEngine.Debug.Log("Incoming initialize request");
        GrpcPlayer player = server.GetPlayer(request.PlayerNumber);
        player.InitializeRPC(request);
        InitializeResponse response = new InitializeResponse();
        response.PlayerUuid = player.PlayerUUID.ToString();
        return Task.FromResult(response);
    }

    public override async Task Participate(ParticipateRequest request, IServerStreamWriter<PlayerGameState> responseStream, ServerCallContext context)
    {
        UnityEngine.Debug.Log("Incoming participate request");
        GrpcPlayer player = server.GetPlayerWithUniqueId(request.PlayerUuid);

        context.CancellationToken.Register(() => { player.onCancel(); }, false);

        while (!context.CancellationToken.IsCancellationRequested)
        {
            if (player.CurrentState != null)
            {
                await responseStream.WriteAsync(player.CurrentState);
                player.CurrentState = null;
            }
        }
    }

    public override Task<Empty> Input(PlayerInput request, ServerCallContext context)
    {
        GrpcPlayer player = server.GetPlayerWithUniqueId(request.PlayerUuid);
        player.OnInput(request);
        return Task.FromResult(new Empty());
    }
}
