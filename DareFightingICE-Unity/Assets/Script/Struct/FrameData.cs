using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameData : MonoBehaviour
{
    [SerializeField] private CharacterData[] characterData;

    [SerializeField] private float currentFrameNumber;

    [SerializeField] private int currentRound;

    public InterfaceDisplay interfaceDisplay;
    public FightingController controller;

   // private Deque<AttackData> projectileData;

    private bool emptyFlag;

    private bool[] front;
    void Update()
    {
        characterData[0] = controller.character[0].GetComponent<CharacterData>();
        characterData[1] = controller.character[1].GetComponent<CharacterData>();
        currentFrameNumber = interfaceDisplay.currentFrame;
        currentRound = 0;
        front[0] = controller.character[0].GetComponent<CharacterController>().IsFront;
        front[1] = controller.character[1].GetComponent<CharacterController>().IsFront;
    }
}
