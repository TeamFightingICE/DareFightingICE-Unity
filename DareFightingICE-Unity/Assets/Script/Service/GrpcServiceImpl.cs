using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DareFightingICE.Grpc.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UnityEngine.AI;

public class GrpcServiceImpl : Service.ServiceBase
{
    private readonly GrpcServer server;
    public GrpcServiceImpl()
    {
        this.server = GrpcServer.Instance;
    }
    public override Task<InitializeResponse> Initialize(InitializeRequest request, ServerCallContext context)
    {
        GrpcPlayer player = (GrpcPlayer)server.GetPlayer(request.PlayerNumber);
        player.InitializeRPC(request);
        InitializeResponse response = new InitializeResponse
        {
            PlayerUuid = player.PlayerUUID.ToString()
        };
        return Task.FromResult(response);
    }

    public override async Task Participate(ParticipateRequest request, IServerStreamWriter<PlayerGameState> responseStream, ServerCallContext context)
    {
        GrpcPlayer player = (GrpcPlayer)server.GetPlayerWithUniqueId(request.PlayerUuid);
        await player.ParticipateRPC(responseStream, context);    
    }

    public override Task<Empty> Input(PlayerInput request, ServerCallContext context)
    {
        GrpcPlayer player = (GrpcPlayer)server.GetPlayerWithUniqueId(request.PlayerUuid);
        player.OnInput(request);
        return Task.FromResult(new Empty());
    }

    public override Task<RunGameResponse> RunGame(RunGameRequest request, ServerCallContext context)
    {
        Debug.Log("GrpcServer: Incoming runGame request");

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

        return Task.FromResult(new RunGameResponse
        {
            StatusCode = statusCode,
            ResponseMessage = responseMessage
        });
    }
}
