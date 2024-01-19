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
public class GameSetting : Singleton<FlagSetting>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetData()
    {
        Application.targetFrameRate = 60;
    }
}
