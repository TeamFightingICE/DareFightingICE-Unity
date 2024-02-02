
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterData : MonoBehaviour
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
    public CharacterController Character;
    public InterfaceDisplay Interface;

    // private Deque<Key> inputCommands;

    //  private Deque<Key> processedCommands;


    void Update()
    {
        playerNumber = Character.PlayerNumber;
        energy = Character.Energy;
        x = Character.GetComponent<Transform>().position.x;   
        y = Character.GetComponent<Transform>().position.y;
  
        speedX = Character.GetComponent<Rigidbody2D>().velocity.x;
        speedY = Character.GetComponent<Rigidbody2D>().velocity.y;
        state = Character.state;
        // Add action information in CharacterController
        action = new Action();
        front = Character.IsFront;
        // need to check what is this for
        control = false;
        remainingFrame = Interface.currentFrame;
    }


}
