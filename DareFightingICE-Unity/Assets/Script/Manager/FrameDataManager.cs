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

    public float currentFrameNumber;

    public int currentRound;
    
    public GameObject[] projectileData;

    public bool emptyFlag;

    public bool[] front = new bool[2];

    private Action action = new Action();
    
    public void SetupFrameData(GameObject character1,GameObject character2,InterfaceDisplay frameInfo)
    {
        character[0] = character1;
        character[1] = character2;
        rb[0] = character1.GetComponent<Rigidbody2D>();
        rb[1] = character2.GetComponent<Rigidbody2D>();
        _controllers[0] = character1.GetComponent<ZenCharacterController>();
        _controllers[1] = character2.GetComponent<ZenCharacterController>();
        _interfaceDisplay = frameInfo;
    }
    
    public void UpdateCharacterData()
    {
        CharacterData _data1 = new CharacterData(_controllers[0].PlayerNumber, _controllers[0].Hp, _controllers[0].Energy,character[0].transform.position.x,character[0].transform.position.y,rb[0].velocity.x,rb[0].velocity.y,_controllers[0].state,action,_controllers[0].IsFront,true,0);
        characterData[0] = _data1;
        front[0] = _controllers[0].IsFront;
        
        CharacterData _data2 = new CharacterData(_controllers[1].PlayerNumber, _controllers[1].Hp, _controllers[1].Energy,character[1].transform.position.x,character[1].transform.position.y,rb[1].velocity.x,rb[1].velocity.y,_controllers[1].state,action,_controllers[1].IsFront,true,0);
        characterData[1] = _data2;
        front[1] = _controllers[1].IsFront;
    }
    
    public void ProcessFrameData()
    {
        UpdateCharacterData();
        currentFrameNumber = _interfaceDisplay.currentFrame;
        currentRound = 0;
        
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
