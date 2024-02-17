using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public int CurrentGame { get; set; } = 1;
    public int CurrentRound { get; set; } = 1;
    public List<RoundResult> RoundResults { get; set; } = new List<RoundResult>();
}
