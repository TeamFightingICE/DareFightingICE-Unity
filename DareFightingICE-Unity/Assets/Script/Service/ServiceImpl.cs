using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DareFightingICE.Grpc.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

public class ServiceImpl : Service.ServiceBase
{
    private GRPCServer server;
    public ServiceImpl()
    {
        this.server = GRPCServer.Instance;
    }
    public override Task<InitializeResponse> Initialize(InitializeRequest request, ServerCallContext context)
    {
        GrpcPlayer player = server.GetPlayer(request.PlayerNumber);
        player.InitializeRPC(request);
        InitializeResponse response = new InitializeResponse();
        response.PlayerUuid = player.PlayerUUID.ToString();
        return Task.FromResult(response);
    }

    public override async Task Participate(ParticipateRequest request, IServerStreamWriter<PlayerGameState> responseStream, ServerCallContext context)
    {
        GrpcPlayer player = server.GetPlayerWithUniqueId(request.PlayerUuid);
        await Task.Run(() => { player.ParticipateRPC(responseStream, context); });
    }

    public override Task<Empty> Input(PlayerInput request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }
}
