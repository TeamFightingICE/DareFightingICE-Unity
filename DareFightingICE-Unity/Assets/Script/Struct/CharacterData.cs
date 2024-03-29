﻿using DareFightingICE.Grpc.Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterData
{
    public bool PlayerNumber { get; set; }
    public int Hp { get; set; }
    public int Energy { get; set; }
    public float XPos { get; set; }
    public float YPos { get; set; }
    public float XVelo { get; set; }
    public float YVelo { get; set; }
    public State State { get; set; }
    public Action Action { get; set; }
    public bool IsFront { get; set; }
    public bool Control { get; set; }
    public int RemainingFrame { get; set; }

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