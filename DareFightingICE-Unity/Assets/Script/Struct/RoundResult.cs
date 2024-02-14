using DareFightingICE.Grpc.Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundResult
{
    public int CurrentRound { get; set; }
    public int ElaspedFrame { get; set; }
    public int[] RemainingHPs { get; set; }

    public GrpcRoundResult ToProto()
    {
        GrpcRoundResult result = new GrpcRoundResult
        {
            CurrentRound = CurrentRound,
            ElapsedFrame = ElaspedFrame,
            RemainingHps = { RemainingHPs }
        };
        return result;
    }
}
