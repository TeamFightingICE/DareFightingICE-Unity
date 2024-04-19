using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReplayData 
{
   public List<CharacterData> Player1Data;
   public List<CharacterData> Player2Data;

   public ReplayData() 
   {
    this.Player1Data = new List<CharacterData>();
    this.Player2Data = new List<CharacterData>();
   }
   public ReplayData(ReplayData other)
    { 
        this.Player1Data = other.Player1Data;
        this.Player2Data = other.Player2Data;
    }



}
