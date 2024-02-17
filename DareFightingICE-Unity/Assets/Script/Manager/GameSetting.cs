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
    public int FrameDelay { get; set; } = 15;
    public int P1HP { get; set; } = 400;
    public int P2HP { get; set; } = 400;
    public int RoundLimit { get; set; } = 3;
    public int FrameLimit { get; set; } = 3600;
    public int GameRepeatCount { get; set; } = 1;
    public ControlType P1ControlType { get; set; }
    public ControlType P2ControlType { get; set; }
    public bool IsRunWithGrpcAuto { get; set; }

    public void SetData(int p1Hp, int p2Hp, int roundLimit, int frameLimit)
    {
        this.P1HP = p1Hp;
        this.P2HP = p2Hp;
        this.RoundLimit = roundLimit;
        this.FrameLimit = frameLimit;
    }

    public void SetCharacterData(ControlType p1ControlType, ControlType p2ControlType)
    {
        this.P1ControlType = p1ControlType;
        this.P2ControlType = p2ControlType;
    }
    public void SetRunWithGrpcAuto(bool isRunWithGrpcAuto)
    {
        this.IsRunWithGrpcAuto = isRunWithGrpcAuto;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetData()
    {
        
    }
}
