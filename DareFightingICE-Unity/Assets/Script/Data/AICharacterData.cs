
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public class AICharacterData : MonoBehaviour
{
    private bool playerNumber;

    private int hp;

    private int energy;

    private float x;

    private float y;

  //  private int left;

   // private int right;

   // private int top;

  //  private int bottom;

    private float speedX;

    private float speedY;

    private State state;

    private Action action;

    private bool front;

    // need to check what is this for
    private bool control;

    //private AttackData attackData;

    private float remainingFrame;

   // private bool hitConfirm;

   // private int graphicSizeX;

 //   private int graphicSizeY;

  //  private int graphicAdjustX;

  //  private int hitCount;

  //  private int lastHitFrame;
    public ZenCharacterController zenCharacter;
    public InterfaceDisplay Interface;

    // private Deque<Key> inputCommands;

    //  private Deque<Key> processedCommands;


    void Update()
    {
        playerNumber = zenCharacter.PlayerNumber;
        energy = zenCharacter.Energy;
        x = zenCharacter.GetComponent<Transform>().position.x;   
        y = zenCharacter.GetComponent<Transform>().position.y;
  
        speedX = zenCharacter.GetComponent<Rigidbody2D>().velocity.x;
        speedY = zenCharacter.GetComponent<Rigidbody2D>().velocity.y;
        state = zenCharacter.state;
        // Add action information in ZenCharacterController
        action = new Action();
        front = zenCharacter.IsFront;
        // need to check what is this for
        control = false;
        remainingFrame = Interface.currentFrame;
    }


}
