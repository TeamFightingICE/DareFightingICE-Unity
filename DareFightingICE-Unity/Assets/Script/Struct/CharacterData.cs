using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public bool PlayerNumber;
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
    
    public CharacterData(bool playerNumber, int energy, float xPos, float yPos, float xVelo, float yVelo, State state, Action action, bool isFront, bool control, float remainingFrame)
    {
        PlayerNumber = playerNumber;
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
}