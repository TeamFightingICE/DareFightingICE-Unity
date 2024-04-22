using DareFightingICE.Grpc.Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public bool PlayerNumber { get; set; }
    public int Hp { get; set; }
    public int Energy { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }
    public int Top { get; set; }
    public int Bottom { get; set; }
    public int SpeedX { get; set; }
    public int SpeedY { get; set; }
    public State State { get; set; }
    public Action Action { get; set; }
    public bool Front { get; set; }
    public bool Control { get; set; }
    public AttackData AttackData { get; set; }
    public int RemainingFrame { get; set; }
    public bool HitConfirm { get; set; }
    public int GraphicSizeX { get; set; }
    public int GraphicSizeY { get; set; }
    public int GraphicAdjustX { get; set; }
    public int HitCount { get; set; }
    public int LastHitFrame { get; set; }

    public CharacterData()
    {
        PlayerNumber = false;
        Hp = 0;
        Energy = 0;
        X = 0;
        Y = 0;
        Left = 0;
        Right = 0;
        Top = 0;
        Bottom = 0;
        SpeedX = 0;
        SpeedY = 0;
        State = State.Stand;
        Action = Action.STAND;
        Front = false;
        Control = false;
        AttackData = new AttackData();
        RemainingFrame = 0;
        HitConfirm = false;
        GraphicSizeX = 0;
        GraphicSizeY = 0;
        GraphicAdjustX = 0;
        HitCount = 0;
        LastHitFrame = 0;
    }

    public CharacterData(CharacterData other)
    {
        PlayerNumber = other.PlayerNumber;
        Hp = other.Hp;
        Energy = other.Energy;
        X = other.X;
        Y = other.Y;
        Left = other.Left;
        Right = other.Right;
        Top = other.Top;
        Bottom = other.Bottom;
        SpeedX = other.SpeedX;
        SpeedY = other.SpeedY;
        State = other.State;
        Action = other.Action;
        Front = other.Front;
        Control = other.Control;
        AttackData = new AttackData(other.AttackData);
        RemainingFrame = other.RemainingFrame;
        HitConfirm = other.HitConfirm;
        GraphicSizeX = other.GraphicSizeX;
        GraphicSizeY = other.GraphicSizeY;
        GraphicAdjustX = other.GraphicAdjustX;
        HitCount = other.HitCount;
        LastHitFrame = other.LastHitFrame;
    }

    public GrpcCharacterData ToProto()
    {
        return new GrpcCharacterData
        {
            PlayerNumber = PlayerNumber,
            Hp = Hp,
            Energy = Energy,
            X = X,
            Y = Y,
            Left = Left,
            Right = Right,
            Top = Top,
            Bottom = Bottom,
            SpeedX = SpeedX,
            SpeedY = SpeedY,
            State = (GrpcState) State,
            Action = (GrpcAction) Action,
            Front = Front,
            Control = Control,
            AttackData = AttackData.ToProto(),
            RemainingFrame = RemainingFrame,
            HitConfirm = HitConfirm,
            GraphicSizeX = GraphicSizeX,
            GraphicSizeY = GraphicSizeY,
            GraphicAdjustX = GraphicAdjustX,
            HitCount = HitCount,
            LastHitFrame = LastHitFrame
        };
    }

}