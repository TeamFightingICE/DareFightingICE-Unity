using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameData
{
    public CharacterData[] CharacterData { get; set; }

    public float CurrentFrameNumber { get; set; }

    public int CurrentRound { get; set; }

    public List<AttackData> ProjectileData { get; set; }

    public bool EmptyFlag { get; set; }

    public bool[] Front { get; set; }
    
}
