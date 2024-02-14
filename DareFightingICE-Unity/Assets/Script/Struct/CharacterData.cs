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
    public float RemainingFrame;

    public CharacterData(bool playerNumber, int hp, int energy, float xPos, float yPos, float xVelo, float yVelo, State state, Action action, bool isFront, bool control, float remainingFrame)
    {
        PlayerNumber = playerNumber;
        Hp = hp;
        Energy = energy;
        XPos = xPos;
        YPos = yPos;
        XVelo = xVelo;
        YVelo = yVelo;
        State = state;
        Action = action;
        IsFront = isFront;
        Control = control;
        RemainingFrame = remainingFrame;
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
            RemainingFrame = (int) RemainingFrame,
        };
    }
}