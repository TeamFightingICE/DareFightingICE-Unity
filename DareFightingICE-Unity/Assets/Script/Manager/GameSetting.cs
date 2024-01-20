using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Stand,
    Crouch,
    Air,
    Down,
}
public class GameSetting : Singleton<GameSetting>
{
    public int p1Hp;
    public int p2Hp;
    public int roundLimit;
    public int frameLimit;

    public void Setdata(int hp1,int hp2,int rLimit,int fLimit)
    {
        p1Hp = hp1;
        p2Hp = hp2;
        roundLimit = rLimit;
        frameLimit = fLimit;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetData()
    {
        
    }
}
