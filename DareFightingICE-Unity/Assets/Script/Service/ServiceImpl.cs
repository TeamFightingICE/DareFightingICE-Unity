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

        // Mock
        player.CurrentState = new PlayerGameState();
        player.CurrentState.StateFlag = GrpcFlag.Initialize;
        player.CurrentState.GameData = new GrpcGameData();
        player.CurrentState.GameData.MaxHps.Add(400);
        player.CurrentState.GameData.MaxHps.Add(400);
        player.CurrentState.GameData.MaxEnergies.Add(300);
        player.CurrentState.GameData.MaxEnergies.Add(300);
        player.CurrentState.GameData.CharacterNames.Add("ZEN");
        player.CurrentState.GameData.CharacterNames.Add("ZEN");
        player.CurrentState.GameData.AiNames.Add("KickAI");
        player.CurrentState.GameData.AiNames.Add("Keyboard");
        // END Mock

        while (!context.CancellationToken.IsCancellationRequested)
        {
            if (player.CurrentState != null)
            {
                await responseStream.WriteAsync(player.CurrentState);
                player.CurrentState = null;

                // Mock
                player.CurrentState = new PlayerGameState();
                player.CurrentState.StateFlag = GrpcFlag.Processing;
                //player.CurrentState.FrameData.Front.Add(true);
                //player.CurrentState.FrameData.Front.Add(false);
                player.CurrentState.IsControl = true;
                // END Mock
            }
        }
    }

    public override Task<Empty> Input(PlayerInput request, ServerCallContext context)
    {
        try
        {
            GrpcPlayer player = server.GetPlayerWithUniqueId(request.PlayerUuid);
            player.onInput(request);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }
        return Task.FromResult(new Empty());
    }
}
