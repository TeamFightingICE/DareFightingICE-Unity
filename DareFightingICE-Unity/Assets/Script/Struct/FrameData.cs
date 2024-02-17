using DareFightingICE.Grpc.Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class FrameData
{
    public CharacterData[] CharacterData { get; set; }

    public int CurrentFrameNumber { get; set; }

    public int CurrentRound { get; set; }

    public List<AttackData> ProjectileData { get; set; }

    public bool EmptyFlag { get; set; }

    public bool[] Front { get; set; }

    public FrameData()
    {
        this.CharacterData = new CharacterData[] { null, null };
        this.CurrentFrameNumber = -1;
        this.CurrentRound = -1;
        this.ProjectileData = new List<AttackData>();
        this.EmptyFlag = true;
        this.Front = new bool[2];
    }

    public FrameData(FrameData other)
    {
        this.CharacterData = new CharacterData[2] {
            other.CharacterData[0] != null ? new CharacterData(other.CharacterData[0]) : null,
            other.CharacterData[1] != null ? new CharacterData(other.CharacterData[1]) : null
        };
        this.CurrentFrameNumber = other.CurrentFrameNumber;
        this.CurrentRound = other.CurrentRound;
        this.ProjectileData = new List<AttackData>();
        foreach (var item in other.ProjectileData)
        {
            this.ProjectileData.Add(new AttackData(item));
        }
        this.EmptyFlag = other.EmptyFlag;
        this.Front = new bool[2] { other.Front[0], other.Front[1] };
    }


    public int RemainingFrameNumber {
        get
        {
            return GameSetting.Instance.FrameLimit - this.CurrentFrameNumber;
        }
    }

    public CharacterData GetCharacterData(bool playerNumber)
    {
        return this.CharacterData[playerNumber ? 0 : 1];
    }

    public bool IsFront(bool playerNumber)
    {
        return this.Front[playerNumber ? 0 : 1];
    }

    public void RemoveVisualData()
    {
        this.CharacterData = new CharacterData[2];
        this.CurrentRound = -1;
        this.ProjectileData = new List<AttackData>();
    }

    public GrpcFrameData ToProto()
    {
        GrpcFrameData frameData = new()
        {
            CurrentFrameNumber = CurrentFrameNumber,
            CurrentRound = CurrentRound,
            EmptyFlag = EmptyFlag,
            Front = { Front },
        };
        frameData.CharacterData.Add(new GrpcCharacterData());
        frameData.CharacterData.Add(new GrpcCharacterData());
        if (this.CharacterData[0] != null)
        {
            frameData.CharacterData[0] = this.CharacterData[0].ToProto();
        }
        if (this.CharacterData[1] != null)
        {
            frameData.CharacterData[1] = this.CharacterData[1].ToProto();
        }
        foreach (var projectile in this.ProjectileData)
        {
            frameData.ProjectileData.Add(projectile.ToProto());
        }
        return frameData;
    }
}
