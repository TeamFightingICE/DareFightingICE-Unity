using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameDataManager : Singleton<FrameDataManager>
{
    public GameObject[] character = new GameObject[2];
    private Rigidbody2D[] rb = new Rigidbody2D[2];
    private ZenCharacterController[] _controllers = new ZenCharacterController[2];
    private InterfaceDisplay _interfaceDisplay;
    
    public CharacterData[] characterData = new CharacterData[2];

    public int currentFrameNumber;

    public int currentRound;
    
    public GameObject[] projectileData;

    public bool emptyFlag;

    public bool[] front = new bool[2];

    private Action action = new Action();

    private int GetRemainingFrame()
    {
        return GameSetting.Instance.FrameLimit - currentFrameNumber;
    }
    
    public void SetupFrameData(GameObject character1, GameObject character2, InterfaceDisplay frameInfo)
    {
        character[0] = character1;
        character[1] = character2;
        rb[0] = character1.GetComponent<Rigidbody2D>();
        rb[1] = character2.GetComponent<Rigidbody2D>();
        _controllers[0] = character1.GetComponent<ZenCharacterController>();
        _controllers[1] = character2.GetComponent<ZenCharacterController>();
        _interfaceDisplay = frameInfo;
        currentFrameNumber = 0;
        currentRound = DataManager.Instance.CurrentRound;
    }
    
    public void UpdateCharacterData()
    {
        int remainingFrame = GetRemainingFrame();

        characterData[0] = new CharacterData
        {
            PlayerNumber = _controllers[0].PlayerNumber,
            Hp = _controllers[0].Hp,
            Energy = _controllers[0].Energy,
            XPos = character[0].transform.position.x,
            YPos = character[0].transform.position.y,
            XVelo = rb[0].velocity.x,
            YVelo = rb[0].velocity.y,
            State = _controllers[0].state,
            Action = action,
            IsFront = _controllers[0].IsFront,
            Control = true,
            RemainingFrame = remainingFrame
        };
        front[0] = _controllers[0].IsFront;

        characterData[1] = new CharacterData
        {
            PlayerNumber = _controllers[1].PlayerNumber,
            Hp = _controllers[1].Hp,
            Energy = _controllers[1].Energy,
            XPos = character[1].transform.position.x,
            YPos = character[1].transform.position.y,
            XVelo = rb[1].velocity.x,
            YVelo = rb[1].velocity.y,
            State = _controllers[1].state,
            Action = action,
            IsFront = _controllers[1].IsFront,
            Control = true,
            RemainingFrame = remainingFrame
        };
        front[1] = _controllers[1].IsFront;
    }
    
    public void ProcessFrameData()
    {
        UpdateCharacterData();
        currentFrameNumber = _interfaceDisplay.currentFrame;
    }

    public FrameData GetFrameData()
    {
        FrameData data = new FrameData
        {
            CharacterData = characterData,
            CurrentFrameNumber = currentFrameNumber,
            CurrentRound = currentRound,
            ProjectileData = new List<AttackData>(),
            EmptyFlag = false,
            Front = front
        };
        return data;
    }
}
