using DareFightingICE.Grpc.Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterData
{
    public bool PlayerNumber;
    public int Hp;
    public int Energy;
    public float XPos;
    public float YPos;
    public float XVelo;
    public float YVelo;
    public State State;
    public Action Action;
    public bool IsFront;
    public bool Control;
    public int RemainingFrame;

    public CharacterData()
    {
        PlayerNumber = false;
        Hp = 0;
        Energy = 0;
        XPos = 0;
        YPos = 0;
        XVelo = 0;
        YVelo = 0;
        State = State.Stand;
        Action = Action.STAND;
        IsFront = false;
        Control = false;
        RemainingFrame = 0;
    }

    public CharacterData(CharacterData other)
    {
        PlayerNumber = other.PlayerNumber;
        Hp = other.Hp;
        Energy = other.Energy;
        XPos = other.XPos;
        YPos = other.YPos;
        XVelo = other.XVelo;
        YVelo = other.YVelo;
        State = other.State;
        Action = other.Action;
        IsFront = other.IsFront;
        Control = other.Control;
        RemainingFrame = other.RemainingFrame;
    }

    public GrpcCharacterData ToProto()
    {
        return new GrpcCharacterData
        {
            PlayerNumber = PlayerNumber,
            Hp = Hp,
            Energy = Energy,
            X = (int) XPos,
            Y = (int) YPos,
            SpeedX = (int) XVelo,
            SpeedY = (int) YVelo,
            State = (GrpcState) State,
            Action = (GrpcAction) Action,
            Front = IsFront,
            Control = Control,
            RemainingFrame = RemainingFrame,
        };
    }
}