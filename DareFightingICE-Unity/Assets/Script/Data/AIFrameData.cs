using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFrameData : MonoBehaviour
{
    [SerializeField] private AICharacterData[] characterData;

    [SerializeField] private float currentFrameNumber;

    [SerializeField] private int currentRound;

    public InterfaceDisplay interfaceDisplay;
    public FightingController controller;

   // private Deque<AttackData> projectileData;

    private bool emptyFlag;

    private bool[] front;
    void Update()
    {
        characterData[0] = controller.character[0].GetComponent<AICharacterData>();
        characterData[1] = controller.character[1].GetComponent<AICharacterData>();
        currentFrameNumber = interfaceDisplay.currentFrame;
        currentRound = 0;
        front[0] = controller.character[0].GetComponent<ZenCharacterController>().IsFront;
        front[1] = controller.character[1].GetComponent<ZenCharacterController>().IsFront;
    }
}
