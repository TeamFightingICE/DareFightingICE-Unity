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
public enum Action
{
    NEUTRAL, STAND, FORWARD_WALK, DASH, BACK_STEP, CROUCH, JUMP, FOR_JUMP, BACK_JUMP, AIR, STAND_GUARD, CROUCH_GUARD, AIR_GUARD, STAND_GUARD_RECOV, CROUCH_GUARD_RECOV, AIR_GUARD_RECOV, STAND_RECOV, CROUCH_RECOV, AIR_RECOV, CHANGE_DOWN, DOWN, RISE, LANDING, THROW_A, THROW_B, THROW_HIT, THROW_SUFFER, STAND_A, STAND_B, CROUCH_A, CROUCH_B, AIR_A, AIR_B, AIR_DA, AIR_DB, STAND_FA, STAND_FB, CROUCH_FA, CROUCH_FB, AIR_FA, AIR_FB, AIR_UA, AIR_UB, STAND_D_DF_FA, STAND_D_DF_FB, STAND_F_D_DFA, STAND_F_D_DFB, STAND_D_DB_BA, STAND_D_DB_BB, AIR_D_DF_FA, AIR_D_DF_FB, AIR_F_D_DFA, AIR_F_D_DFB, AIR_D_DB_BA, AIR_D_DB_BB, STAND_D_DF_FC,
}
/// <summary>
/// This singleton used to carry all setting from start and launch scene to gameplay
/// </summary>
public class GameSetting : Singleton<GameSetting>
{
    public int p1Hp;
    public int p2Hp;
    public int roundLimit;
    public int frameLimit;
    public ControlType p1Control;
    public ControlType p2Control;

    public void Setdata(int hp1,int hp2,int rLimit,int fLimit)
    {
        p1Hp = hp1;
        p2Hp = hp2;
        roundLimit = rLimit;
        frameLimit = fLimit;
    }

    public void SetCharacterData(ControlType p1,ControlType p2)
    {
        p1Control = p1;
        p2Control = p2;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetData()
    {
        
    }
}
